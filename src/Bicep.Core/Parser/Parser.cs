// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    public class Parser
    {
        private readonly TokenReader reader;

        private readonly ImmutableArray<Diagnostic> lexerDiagnostics;
        
        public Parser(string text)
        {
            // treating the lexer as an implementation detail of the parser
            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            this.lexerDiagnostics = lexer.GetDiagnostics();

            this.reader = new TokenReader(lexer.GetTokens());
        }

        public ProgramSyntax Program()
        {
            var declarationsOrTokens = new List<SyntaxBase>();
            
            while (!this.IsAtEnd())
            {
                // this produces either a declaration node, skipped tokens node or just a token
                var declarationOrToken = Declaration();
                declarationsOrTokens.Add(declarationOrToken);

                // if skipped node is returned above, the newline is not consumed
                // if newline token is returned, we must not expect another (could be a beginning of a declaration)
                if (declarationOrToken is IDeclarationSyntax)
                {
                    // declarations must be followed by a newline or the file must end
                    var newLine = this.WithRecoveryNullable(this.NewLineOrEof, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
                    if (newLine != null)
                    {
                        declarationsOrTokens.Add(newLine);
                    }
                }
            }

            var endOfFile = reader.Read();

            return new ProgramSyntax(declarationsOrTokens, endOfFile, this.lexerDiagnostics);
        }

        public SyntaxBase Declaration() =>
            this.WithRecovery(
                () =>
                {
                    var current = reader.Peek();

                    return current.Type switch
                    {
                        TokenType.Identifier => current.Text switch
                        {
                            LanguageConstants.ParameterKeyword => this.ParameterDeclaration(),
                            LanguageConstants.VariableKeyword => this.VariableDeclaration(),
                            LanguageConstants.ResourceKeyword => this.ResourceDeclaration(),
                            LanguageConstants.OutputKeyword => this.OutputDeclaration(),
                            LanguageConstants.ModuleKeyword => this.ModuleDeclaration(),
                            _ => throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                        },
                        TokenType.NewLine => this.NewLine(),

                        _ => throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                    };
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private static RecoveryFlags GetSuppressionFlag(SyntaxBase precedingNode)
        {
            // local function
            RecoveryFlags ConvertFlags(bool suppress) => suppress ? RecoveryFlags.SuppressDiagnostics : RecoveryFlags.None;

            /*
             * When we have an incomplete declarations like "param\n",
             * the keyword is parsed but all other properties are set to 0-length SkippedTriviaSyntax or MalformedIdentifierSyntax
             * to prevent stacking multiple parse errors on a 0-length span (which is technically correct but also confusing)
             * we will only leave the first parse error
             */
            switch (precedingNode)
            {
                case IdentifierSyntax identifier when identifier.IsValid == false:
                    return ConvertFlags(identifier.Span.Length == 0);

                case SkippedTriviaSyntax skipped:
                    return ConvertFlags(skipped.Span.Length == 0);

                default:
                    return RecoveryFlags.None;
            }
        }

        private SyntaxBase ParameterDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.ParameterKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(b => b.ExpectedParameterType()), GetSuppressionFlag(name), TokenType.Assignment, TokenType.LeftBrace, TokenType.NewLine);

            // TODO: Need a better way to choose the terminating token
            SyntaxBase? modifier = this.WithRecoveryNullable(
                () =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        // the parameter does not have a modifier
                        TokenType.NewLine => null,
                        TokenType.EndOfFile => null,

                        // default value is specified
                        TokenType.Assignment => this.ParameterDefaultValue(),

                        // modifier is specified
                        TokenType.LeftBrace => this.Object(),

                        _ => throw new ExpectedTokenException(current, b => b.ExpectedParameterContinuation())
                    };
                },
                GetSuppressionFlag(type),
                TokenType.NewLine);

            return new ParameterDeclarationSyntax(keyword, name, type, modifier);
        }

        private SyntaxBase ParameterDefaultValue()
        {
            var assignmentToken = this.Expect(TokenType.Assignment, b => b.ExpectedCharacter("="));
            SyntaxBase defaultValue = this.Expression(allowComplexLiterals: true);

            return new ParameterDefaultValueSyntax(assignmentToken, defaultValue);
        }

        private SyntaxBase VariableDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.VariableKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedVariableIdentifier(), TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(allowComplexLiterals: true), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new VariableDeclarationSyntax(keyword, name, assignment, value);
        }

        private SyntaxBase OutputDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.OutputKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedOutputIdentifier(), TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(b => b.ExpectedParameterType()), GetSuppressionFlag(name), TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(allowComplexLiterals: true), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new OutputDeclarationSyntax(keyword, name, type, assignment, value);
        }

        private SyntaxBase ResourceDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.ResourceKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedResourceIdentifier(), TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var type = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedResourceTypeString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.LeftBrace, TokenType.NewLine);
            var body = this.WithRecovery(this.Object, GetSuppressionFlag(assignment), TokenType.NewLine);

            return new ResourceDeclarationSyntax(keyword, name, type, assignment, body);
        }

        private SyntaxBase ModuleDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.ModuleKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedModuleIdentifier(), TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var path = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedModulePathString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(path), TokenType.LeftBrace, TokenType.NewLine);
            var body = this.WithRecovery(this.Object, GetSuppressionFlag(assignment), TokenType.NewLine);

            return new ModuleDeclarationSyntax(keyword, name, path, assignment, body);
        }

        private Token? NewLineOrEof()
        {
            if (reader.Peek().Type == TokenType.EndOfFile)
            {
                // don't actually consume the token
                return null;
            }

            return NewLine();
        }

        private Token NewLine()
        {
            return Expect(TokenType.NewLine, b => b.ExpectedNewLine());
        }

        public SyntaxBase Expression(bool allowComplexLiterals)
        {
            var candidate = this.BinaryExpression(allowComplexLiterals);

            if (this.Check(TokenType.Question))
            {
                var question = this.reader.Read();
                var trueExpression = this.Expression(allowComplexLiterals);
                var colon = this.Expect(TokenType.Colon, b => b.ExpectedCharacter(":"));
                var falseExpression = this.Expression(allowComplexLiterals);

                return new TernaryOperationSyntax(candidate, question, trueExpression, colon, falseExpression);
            }

            return candidate;
        }

        private SyntaxBase BinaryExpression(bool allowComplexLiterals, int precedence = 0)
        {
            var current = this.UnaryExpression(allowComplexLiterals);

            while (true)
            {
                // the current token does not necessarily have to be an operator token
                // it could also be the end of file or some other token that is actually valid in this place
                Token candidateOperatorToken = this.reader.Peek();

                int operatorPrecedence = GetOperatorPrecedence(candidateOperatorToken.Type);

                if (operatorPrecedence <= precedence)
                {
                    break;
                }

                this.reader.Read();

                SyntaxBase rightExpression = this.BinaryExpression(allowComplexLiterals, operatorPrecedence);
                current = new BinaryOperationSyntax(current, candidateOperatorToken, rightExpression);
            }

            return current;
        }

        private SyntaxBase UnaryExpression(bool allowComplexLiterals)
        {
            Token operatorToken = this.reader.Peek();

            if (Operators.TokenTypeToUnaryOperator.TryGetValue(operatorToken.Type, out var @operator))
            {
                this.reader.Read();

                var expression = this.MemberExpression(allowComplexLiterals);
                return new UnaryOperationSyntax(operatorToken, expression);
            }

            return this.MemberExpression(allowComplexLiterals);
        }

        private SyntaxBase MemberExpression(bool allowComplexLiterals)
        {
            var current = this.PrimaryExpression(allowComplexLiterals);

            while (true)
            {
                if (this.Check(TokenType.LeftSquare))
                {
                    // array indexer
                    Token openSquare = this.reader.Read();
                    SyntaxBase indexExpression = this.Expression(allowComplexLiterals);
                    Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                    current = new ArrayAccessSyntax(current, openSquare, indexExpression, closeSquare);

                    continue;
                }

                if (this.Check(TokenType.Dot))
                {
                    // dot operator
                    Token dot = this.reader.Read();
                    IdentifierSyntax identifier = this.Identifier(b => b.ExpectedFunctionOrPropertyName());

                    if (Check(TokenType.LeftParen))
                    {
                        var functionCall = FunctionCallAccess(identifier, allowComplexLiterals);

                        // gets instance function call
                        current = new InstanceFunctionCallSyntax(
                            current,
                            dot,
                            functionCall.Identifier,
                            functionCall.OpenParen,
                            functionCall.ArgumentNodes,
                            functionCall.CloseParen);
                    }
                    else
                    {
                        current = new PropertyAccessSyntax(current, dot, identifier);
                    }

                    continue;
                }

                break;
            }

            return current;
        }

        private SyntaxBase PrimaryExpression(bool allowComplexLiterals)
        {
            Token nextToken = this.reader.Peek();

            switch (nextToken.Type)
            {
                case TokenType.Number:
                case TokenType.NullKeyword:
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    return this.LiteralValue();

                case TokenType.StringComplete:
                case TokenType.StringLeftPiece:
                    return this.InterpolableString();

                case TokenType.LeftBrace when allowComplexLiterals:
                    return this.Object();

                case TokenType.LeftSquare when allowComplexLiterals:
                    return this.Array();

                case TokenType.LeftBrace:
                case TokenType.LeftSquare:
                    throw new ExpectedTokenException(nextToken, b => b.ComplexLiteralsNotAllowed());

                case TokenType.LeftParen:
                    return this.ParenthesizedExpression(allowComplexLiterals);

                case TokenType.Identifier:
                    return this.FunctionCallOrVariableAccess(allowComplexLiterals);

                default:
                    throw new ExpectedTokenException(nextToken, b => b.UnrecognizedExpression());
            }
        }

        private SyntaxBase ParenthesizedExpression(bool allowComplexLiterals)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));
            var expression = this.Expression(allowComplexLiterals);
            var closeParen = this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")"));

            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }

        private SyntaxBase FunctionCallOrVariableAccess(bool allowComplexLiterals)
        {
            var identifier = this.Identifier(b => b.ExpectedVariableOrFunctionName());

            if (Check(TokenType.LeftParen))
            {
                var functionCall = FunctionCallAccess(identifier, allowComplexLiterals);

                return new FunctionCallSyntax(
                    functionCall.Identifier,
                    functionCall.OpenParen,
                    functionCall.ArgumentNodes,
                    functionCall.CloseParen);
            }
            // returns variable access
            return new VariableAccessSyntax(identifier);
        }

        /// <summary>
        /// Method that gets a function call identifier, its arguments plus open and close parens
        /// </summary>
        private (IdentifierSyntax Identifier, Token OpenParen, IEnumerable<FunctionArgumentSyntax> ArgumentNodes, Token CloseParen) FunctionCallAccess(IdentifierSyntax functionName, bool allowComplexLiterals)
        {
           var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));

            var argumentNodes = FunctionCallArguments(allowComplexLiterals);

            var closeParen = this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")"));

            return (functionName, openParen, argumentNodes, closeParen);
        }

        /// <summary>
        /// Method that consumes and returns all arguments from an Instance of Function call.
        /// This method stops when a right paren is found without consuming it, a caller must
        /// consume the right paren token.
        /// </summary>
        /// <param name="allowComplexLiterals"></param>
        private IEnumerable<FunctionArgumentSyntax> FunctionCallArguments(bool allowComplexLiterals)
        {
            if (this.Check(TokenType.RightParen))
            {
                return ImmutableArray<FunctionArgumentSyntax>.Empty;
            }

            var arguments = new List<(SyntaxBase expression, Token? comma)>();

            while (true)
            {
                var expression = this.Expression(allowComplexLiterals);
                arguments.Add((expression, null));

                if (this.Check(TokenType.RightParen))
                {
                    // end of function call
                    // return the accumulated arguments without consuming right paren a caller must consume it
                    var functionArguments = new List<FunctionArgumentSyntax>(arguments.Count);
                    foreach (var argument in arguments)
                    {
                        functionArguments.Add(
                            new FunctionArgumentSyntax(argument.expression, argument.comma));
                    }
                    return functionArguments.ToImmutableArray();
                }

                var comma = this.Expect(TokenType.Comma, b => b.ExpectedCharacter(","));

                // update the tuple
                var lastArgument = arguments.Last();
                lastArgument.comma = comma;
                arguments[arguments.Count - 1] = lastArgument;
            }
        }

        private ImmutableArray<Token> NewLines()
        {
            var newLines = new List<Token>
            {
                this.NewLine()
            };

            while (Check(TokenType.NewLine))
            {
                newLines.Add(reader.Read());
            }

            return newLines.ToImmutableArray();
        }

        private Token Assignment()
        {
            return this.Expect(TokenType.Assignment, b => b.ExpectedCharacter("="));
        }

        private IdentifierSyntax Identifier(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var identifier = Expect(TokenType.Identifier, errorFunc);

            return new IdentifierSyntax(identifier);
        }

        private IdentifierSyntax IdentifierWithRecovery(DiagnosticBuilder.ErrorBuilderDelegate errorFunc, params TokenType[] terminatingTypes)
        {
            var identifierOrSkipped = this.WithRecovery(
                () => Identifier(errorFunc),
                RecoveryFlags.None,
                terminatingTypes);

            switch (identifierOrSkipped)
            {
                case IdentifierSyntax identifier:
                    return identifier;

                case SkippedTriviaSyntax skipped:
                    return new IdentifierSyntax(skipped);

                default:
                    throw new NotImplementedException($"Unexpected identifier syntax type '{identifierOrSkipped.GetType().Name}'");
            }
        }

        private TypeSyntax Type(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var identifier = Expect(TokenType.Identifier, errorFunc);

            return new TypeSyntax(identifier);
        }

        private NumericLiteralSyntax NumericLiteral()
        {
            var literal = Expect(TokenType.Number, b => b.ExpectedNumericLiteral());

            if (Int32.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
            {
                return new NumericLiteralSyntax(literal, value);
            }

            // TODO: Should probably be moved to type checking
            // integer is invalid (too long to fit in an int32)
            throw new ExpectedTokenException(literal, b => b.InvalidInteger());
        }

        private SyntaxBase InterpolableString()
        {
            var startToken = reader.Peek();
            var tokensOrSyntax = new List<SyntaxBase>();

            SyntaxBase? ProcessStringSegment(bool isFirstSegment)
            {
                // This local function will be called in a loop to consume string segments and expressions in interpolation holes.
                // Returning a non-null result will result in the caller terminating the loop and returning the given syntax tree for the string.

                var hadErrors = false;
                var isComplete = false;
                var currentType = reader.Peek().Type;

                // Depending on where we are in the loop, we need to look for different string tokens to orientate ourselves.
                // If we're handling the first segment, the final (only) segment will look like "'...'" and continuation will look like "'...${".
                // If we're handling later segments, the final segment will look like "}...'" and continuation will look like "}...${".
                var tokenStringEnd = isFirstSegment ? TokenType.StringComplete : TokenType.StringRightPiece;
                var tokenStringContinue = isFirstSegment ? TokenType.StringLeftPiece : TokenType.StringMiddlePiece;

                if (currentType == tokenStringEnd)
                {
                    // We're done - exit the loop.
                    tokensOrSyntax.Add(reader.Read());
                    isComplete = true;
                }
                else if (currentType == tokenStringContinue)
                {
                    tokensOrSyntax.Add(reader.Read());

                    // Look for an expression syntax inside the interpolation 'hole' (between "${" and "}").
                    // The lexer doesn't allow an expression contained inside an interpolation to span multiple lines, so we can safely use recovery to look for a NewLine character.
                    // We are also blocking complex literals (arrays and objects) from inside string interpolation
                    var interpExpression = WithRecovery(() => Expression(allowComplexLiterals: false), RecoveryFlags.None, TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine);
                    if (!Check(TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine))
                    {
                        // We may have successfully parsed the expression, but have not reached the end of the expression hole. Skip to the end of the hole.
                        var skippedSyntax = SynchronizeAndReturnTrivia(reader.Position, RecoveryFlags.None, b => b.UnexpectedTokensInInterpolation(), TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine);

                        // Things start to get hairy to build the string if we return an uneven number of tokens and expressions.
                        // Rather than trying to add two expression nodes, combine them.
                        var combined = new [] { interpExpression, skippedSyntax };
                        interpExpression = new SkippedTriviaSyntax(TextSpan.Between(combined.First(), combined.Last()), combined, Enumerable.Empty<Diagnostic>());
                    }

                    tokensOrSyntax.Add(interpExpression);

                    if (Check(TokenType.NewLine) || IsAtEnd())
                    {
                        // Terminate the loop with errors.
                        hadErrors = true;
                    }
                }
                else
                {
                    // Don't consume any tokens that aren't part of this syntax - allow synchronize to handle that safely.
                    var skippedSyntax = SynchronizeAndReturnTrivia(reader.Position, RecoveryFlags.None, b => b.UnexpectedTokensInInterpolation(), TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine);
                    tokensOrSyntax.Add(skippedSyntax);

                    // If we're able to match a continuation, we should keep going, even if the expression parsing fails.
                    // A bad expression will simply be added as a SkippedTriviaSyntax node.
                    //if (reader.Peek().Type == TokenType.NewLine || IsAtEnd())
                    if (!Check(TokenType.StringMiddlePiece, TokenType.StringRightPiece))
                    {
                        // Terminate the loop with errors.
                        hadErrors = true;
                    }
                }

                if (isComplete)
                {
                    var tokens = new List<Token>();
                    var expressions = new List<SyntaxBase>();
                    foreach (var element in tokensOrSyntax)
                    {
                        if (element is Token token)
                        {
                            tokens.Add(token);
                        }
                        else
                        {
                            expressions.Add(element);
                        }
                    }

                    // The lexer may return unterminated string tokens to allow lexing to continue over an interpolated string.
                    // We should catch that here and prevent parsing from succeeding.
                    var segments = Lexer.TryGetRawStringSegments(tokens);
                    if (segments != null)
                    {
                        return new StringSyntax(tokens, expressions, segments);
                    }

                    // Fall back to main error-handling, we can't safely return a string.
                    hadErrors = true;
                }

                if (hadErrors)
                {
                    // This error-handling is just for cases where we were completely unable to interpret the string.
                    var span = TextSpan.BetweenInclusiveAndExclusive(startToken, reader.Peek());
                    return new SkippedTriviaSyntax(span, tokensOrSyntax, Enumerable.Empty<Diagnostic>());
                }

                return null;
            }

            var isFirstSegment = true;
            while (true)
            {
                // Here we're actually parsing and returning the completed string
                var output = ProcessStringSegment(isFirstSegment);
                if (output != null)
                {
                    return output;
                }

                isFirstSegment = false;
            }
        }

        private SyntaxBase LiteralValue()
        {
            var current = reader.Peek();
            switch (current.Type)
            {
                case TokenType.TrueKeyword:
                    return new BooleanLiteralSyntax(reader.Read(), true);

                case TokenType.FalseKeyword:
                    return new BooleanLiteralSyntax(reader.Read(), false);

                case TokenType.Number:
                    return this.NumericLiteral();

                case TokenType.NullKeyword:
                    return new NullLiteralSyntax(reader.Read());

                default:
                    throw new ExpectedTokenException(current, b => b.InvalidType());
            }
        }

        private SyntaxBase Array()
        {
            var openBracket = Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));

            if (Check(TokenType.RightSquare))
            {
                // allow a close on the same line for an empty array
                var emptyCloseBracket = reader.Read();
                return new ArraySyntax(openBracket, Enumerable.Empty<Token>(), Enumerable.Empty<SyntaxBase>(), emptyCloseBracket);
            }

            var newLines = this.NewLines();

            var items = new List<SyntaxBase>();
            while (this.IsAtEnd() == false && this.reader.Peek().Type != TokenType.RightSquare)
            {
                var item = this.ArrayItem();
                items.Add(item);
            }

            var closeBracket = Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

            return new ArraySyntax(openBracket, newLines, items, closeBracket);
        }

        private SyntaxBase ArrayItem()
        {
            return this.WithRecovery(() =>
            {
                var value = this.Expression(allowComplexLiterals: true);
                var newLines = this.NewLines();

                return new ArrayItemSyntax(value, newLines);
            }, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
        }

        private ObjectSyntax Object()
        {
            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));

            if (Check(TokenType.RightBrace))
            {
                // allow a close on the same line for an empty object
                var emptyCloseBrace = reader.Read();
                return new ObjectSyntax(openBrace, ImmutableArray<SyntaxBase>.Empty, emptyCloseBrace);
            }

            var propertiesOrTokens = new List<SyntaxBase>();
            while (!this.IsAtEnd() && this.reader.Peek().Type != TokenType.RightBrace)
            {
                // this produces a property node, skipped tokens node, or just a newline token
                var propertyOrToken = this.ObjectProperty();
                propertiesOrTokens.Add(propertyOrToken);

                // if skipped tokens node is returned above, the newline is not consumed
                // if newline token is returned, we must not expect another (could be beginning of a new property)
                if (propertyOrToken is ObjectPropertySyntax)
                {
                    // properties must be followed by newlines
                    var newLine = this.WithRecoveryNullable(this.NewLineOrEof, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
                    if (newLine != null)
                    {
                        propertiesOrTokens.Add(newLine);
                    }
                }
            }

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new ObjectSyntax(openBrace, propertiesOrTokens, closeBrace);
        }

        private SyntaxBase ObjectProperty()
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                // TODO: Clean this up a bit
                var current = this.reader.Peek();
                if (current.Type == TokenType.NewLine)
                {
                    return this.NewLine();
                }

                var key = this.WithRecovery(
                    () => ThrowIfSkipped(
                        () =>
                            current.Type switch
                            {
                                TokenType.Identifier => this.Identifier(b => b.ExpectedPropertyName()),
                                TokenType.StringComplete => this.InterpolableString(),
                                TokenType.StringLeftPiece => this.InterpolableString(),
                                _ => throw new ExpectedTokenException(current, b => b.ExpectedPropertyName()),
                            }, b => b.ExpectedPropertyName()),
                    RecoveryFlags.None,
                    TokenType.Colon, TokenType.NewLine);

                var colon = this.WithRecovery(() => Expect(TokenType.Colon, b => b.ExpectedCharacter(":")), GetSuppressionFlag(key), TokenType.NewLine);
                var value = this.WithRecovery(() => Expression(allowComplexLiterals: true), GetSuppressionFlag(colon), TokenType.NewLine);

                return new ObjectPropertySyntax(key, colon, value);
            }, RecoveryFlags.None, TokenType.NewLine);
        }

        private SyntaxBase WithRecovery<TSyntax>(Func<TSyntax> syntaxFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
            where TSyntax : SyntaxBase
        {
            var startReaderPosition = reader.Position;
            try
            {
                return syntaxFunc();
            }
            catch (ExpectedTokenException exception)
            {
                return SynchronizeAndReturnTrivia(startReaderPosition, flags, _ => exception.Error, terminatingTypes);
            }
        }

        private SyntaxBase? WithRecoveryNullable<TSyntax>(Func<TSyntax> syntaxFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
            where TSyntax : SyntaxBase?
        {
            var startReaderPosition = reader.Position;
            try
            {
                return syntaxFunc();
            }
            catch (ExpectedTokenException exception)
            {
                return SynchronizeAndReturnTrivia(startReaderPosition, flags, _ => exception.Error, terminatingTypes);
            }
        }

        private SyntaxBase ThrowIfSkipped(Func<SyntaxBase> syntaxFunc, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var startToken = reader.Peek();
            var syntax = syntaxFunc();

            if (syntax.IsSkipped)
            {
                throw new ExpectedTokenException(startToken, errorFunc);
            }

            return syntax;
        }

        private void Synchronize(bool consumeTerminator, params TokenType[] expectedTypes)
        {
            while (!IsAtEnd())
            {
                if (consumeTerminator ? Match(expectedTypes) : Check(expectedTypes))
                {
                    return;
                }

                reader.Read();
            }
        }

        private SkippedTriviaSyntax SynchronizeAndReturnTrivia(int startReaderPosition, RecoveryFlags flags, DiagnosticBuilder.ErrorBuilderDelegate errorFunc, params TokenType[] expectedTypes)
        {
            var startToken = reader.AtPosition(startReaderPosition);

            // Generally we don't want the error span to include the terminating token, so synchronize with and without if required.
            // The skipped trivia returned should always include the full span
            Synchronize(false, expectedTypes);
            var skippedTokens = reader.Slice(startReaderPosition, reader.Position - startReaderPosition);
            var skippedSpan = TextSpan.SafeBetween(skippedTokens, startToken.Span.Position);
            var errorSpan = skippedSpan;

            if (flags.HasFlag(RecoveryFlags.ConsumeTerminator))
            {
                Synchronize(true, expectedTypes);

                skippedTokens = reader.Slice(startReaderPosition, reader.Position - startReaderPosition);
                skippedSpan = TextSpan.SafeBetween(skippedTokens, startToken.Span.Position);
            }

            var errors = flags.HasFlag(RecoveryFlags.SuppressDiagnostics)
                ? ImmutableArray<ErrorDiagnostic>.Empty
                : ImmutableArray.Create(errorFunc(DiagnosticBuilder.ForPosition(errorSpan)));

            return new SkippedTriviaSyntax(skippedSpan, skippedTokens, errors);
        }

        private bool IsAtEnd()
        {
            return reader.IsAtEnd() || reader.Peek().Type == TokenType.EndOfFile;
        }

        private Token Expect(TokenType type, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            if (this.Check(type))
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            throw new ExpectedTokenException(this.reader.Peek(), errorFunc);
        }

        private Token ExpectKeyword(string expectedKeyword)
        {
            if (this.Check(TokenType.Identifier) && reader.Peek().Text == expectedKeyword)
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            throw new ExpectedTokenException(this.reader.Peek(), b => b.ExpectedKeyword(expectedKeyword));
        }

        private bool Match(params TokenType[] types)
        {
            if (Check(types))
            {
                reader.Read();
                return true;
            }

            return false;
        }

        private bool Check(params TokenType[] types)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return types.Contains(reader.Peek().Type);
        }

        private static int GetOperatorPrecedence(TokenType tokenType)
        {
            // the absolute values are not important here
            switch (tokenType)
            {
                case TokenType.Modulo:
                case TokenType.Asterisk:
                case TokenType.Slash:
                    return 100;

                case TokenType.Plus:
                case TokenType.Minus:
                    return 90;

                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                    return 80;

                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.EqualsInsensitive:
                case TokenType.NotEqualsInsensitive:
                    return 70;

                // if we add bitwise operators in the future, they should go here

                case TokenType.LogicalAnd:
                    return 50;

                case TokenType.LogicalOr:
                    return 40;

                default:
                    return -1;
            }
        }
    }
}
