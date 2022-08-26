// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Bicep.Core.Parsing
{
    public abstract class BaseParser
    {
        protected readonly TokenReader reader;

        protected readonly ImmutableArray<IDiagnostic> lexerDiagnostics;

        protected BaseParser(string text)
        {
            // treating the lexer as an implementation detail of the parser
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            this.lexerDiagnostics = diagnosticWriter.GetDiagnostics().ToImmutableArray();

            this.reader = new TokenReader(lexer.GetTokens());
        }

        private static bool CheckKeyword(Token? token, string keyword) => token?.Type == TokenType.Identifier && token.Text == keyword;

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

                case TokenType.DoubleQuestion:
                    return 30;

                default:
                    return -1;
            }
        }

        protected static RecoveryFlags GetSuppressionFlag(SyntaxBase precedingNode)
        {
            // local function
            static RecoveryFlags ConvertFlags(bool suppress) => suppress ? RecoveryFlags.SuppressDiagnostics : RecoveryFlags.None;

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

        private static RecoveryFlags GetSuppressionFlag(SyntaxBase precedingNode, bool predicate)
        {
            var initial = GetSuppressionFlag(precedingNode);
            if (initial == RecoveryFlags.SuppressDiagnostics)
            {
                // we already made a decision to suppress diagnostics
                // the predicate is irrelevant
                return RecoveryFlags.SuppressDiagnostics;
            }

            Debug.Assert(initial == RecoveryFlags.None, "initial == RecoveryFlags.None");

            // predicate must hold to preserve diagnostics
            return predicate ? RecoveryFlags.None : RecoveryFlags.SuppressDiagnostics;
        }

        private static bool HasExpressionFlag(ExpressionFlags flags, ExpressionFlags check)
        {
            // Use this instead of Enum.HasFlag which boxes the enum and allocates.
            return (flags & check) == check;
        }

        private static ExpressionFlags WithExpressionFlag(ExpressionFlags flags, ExpressionFlags set)
        {
            return flags | set;
        }

        private static ExpressionFlags WithoutExpressionFlag(ExpressionFlags flags, ExpressionFlags unset)
        {
            return flags & ~unset;
        }

        public SyntaxBase Expression(ExpressionFlags expressionFlags)
        {
            var candidate = this.BinaryExpression(expressionFlags);

            if (this.Check(TokenType.Question))
            {
                var question = this.reader.Read();
                var trueExpression = this.WithRecovery(
                    () => this.Expression(expressionFlags),
                    RecoveryFlags.None,
                    TokenType.Colon,
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine);
                var colon = this.WithRecovery(
                    () => this.Expect(TokenType.Colon, b => b.ExpectedCharacter(":")),
                    GetSuppressionFlag(trueExpression),
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine);
                var falseExpression = this.WithRecovery(
                    () => this.Expression(expressionFlags),
                    GetSuppressionFlag(colon),
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine);

                return new TernaryOperationSyntax(candidate, question, trueExpression, colon, falseExpression);
            }

            return candidate;
        }

        public abstract ProgramSyntax Program();

        protected abstract SyntaxBase Declaration();

        private SyntaxBase Array()
        {
            var openBracket = Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightSquare,
                parseChildElement: () => ArrayItem());

            var closeBracket = Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

            return new ArraySyntax(openBracket, itemsOrTokens, closeBracket);
        }

        private SyntaxBase ArrayItem()
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                var value = this.Expression(ExpressionFlags.AllowComplexLiterals);

                return new ArrayItemSyntax(value);
            }, RecoveryFlags.None, TokenType.NewLine, TokenType.RightSquare);
        }

        protected Token Assignment()
        {
            return this.Expect(TokenType.Assignment, b => b.ExpectedCharacter("="));
        }

        private SyntaxBase BinaryExpression(ExpressionFlags expressionFlags, int precedence = 0)
        {
            var current = this.UnaryExpression(expressionFlags);

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

                SyntaxBase rightExpression = this.WithRecovery(
                    () => this.BinaryExpression(expressionFlags, operatorPrecedence),
                    RecoveryFlags.None,
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine);

                current = new BinaryOperationSyntax(current, candidateOperatorToken, rightExpression);
            }

            return current;
        }

        protected bool Check(Token? token, params TokenType[] types)
        {
            if (token is null)
            {
                return false;
            }

            return types.Contains(token.Type);
        }

        protected bool Check(params TokenType[] types)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return types.Contains(reader.Peek().Type);
        }

        private bool CheckKeyword(string keyword) => !this.IsAtEnd() && CheckKeyword(this.reader.Peek(), keyword);

        protected Token Expect(TokenType type, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            if (this.Check(type))
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            throw new ExpectedTokenException(this.reader.Peek(), errorFunc);
        }

        protected Token ExpectKeyword(string expectedKeyword)
        {
            return GetOptionalKeyword(expectedKeyword) ??
                throw new ExpectedTokenException(this.reader.Peek(), b => b.ExpectedKeyword(expectedKeyword));
        }

        private SyntaxBase ForBody(ExpressionFlags expressionFlags, bool isResourceOrModuleContext)
        {
            if (!isResourceOrModuleContext)
            {
                // we're not parsing a resource or module body, which means we can have any expression at this point
                return this.Expression(WithExpressionFlag(expressionFlags, ExpressionFlags.AllowComplexLiterals));
            }

            // we're parsing a resource or module body
            // at this point, we can have either an object literal or a condition
            var current = this.reader.Peek();
            return current.Type switch
            {
                TokenType.LeftBrace => this.Object(expressionFlags),
                TokenType.Identifier when current.Text == LanguageConstants.IfKeyword => this.IfCondition(expressionFlags, insideForExpression: true),

                _ => throw new ExpectedTokenException(current, b => b.ExpectBodyStartOrIf())
            };
        }

        protected ForSyntax ForExpression(ExpressionFlags expressionFlags, bool isResourceOrModuleContext)
        {
            var openBracket = this.Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));
            var forKeyword = this.ExpectKeyword(LanguageConstants.ForKeyword);
            SyntaxBase variableSection = this.reader.Peek().Type switch
            {
                TokenType.Identifier => new LocalVariableSyntax(this.Identifier(b => b.ExpectedLoopVariableIdentifier())),
                TokenType.LeftParen => this.ForVariableBlock(),
                _ => this.SkipEmpty(b => b.ExpectedLoopItemIdentifierOrVariableBlockStart())
            };

            var inKeyword = this.WithRecovery(() => this.ExpectKeyword(LanguageConstants.InKeyword), variableSection.HasParseErrors() ? RecoveryFlags.SuppressDiagnostics : RecoveryFlags.None, TokenType.RightSquare, TokenType.NewLine);
            var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(inKeyword), TokenType.Colon, TokenType.RightSquare, TokenType.NewLine);
            var colon = this.WithRecovery(() => this.Expect(TokenType.Colon, b => b.ExpectedCharacter(":")), GetSuppressionFlag(expression), TokenType.RightSquare, TokenType.NewLine);
            var body = this.WithRecovery(
                () => this.ForBody(expressionFlags, isResourceOrModuleContext),
                GetSuppressionFlag(colon),
                TokenType.RightSquare, TokenType.NewLine);
            var closeBracket = this.WithRecovery(() => this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]")), GetSuppressionFlag(body), TokenType.RightSquare, TokenType.NewLine);

            return new(openBracket, forKeyword, variableSection, inKeyword, expression, colon, body, closeBracket);
        }

        private SyntaxBase ForVariableBlock()
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(ExpressionFlags.None);

            var variableBlock = GetVariableBlock(openParen, expressionsOrCommas, closeParen);

            if (variableBlock.Arguments.Length != 2 && !variableBlock.HasParseErrors())
            {
                return Skip(variableBlock.AsEnumerable(), x => x.ExpectedLoopVariableBlockWith2Elements(variableBlock.Arguments.Length));
            }

            return variableBlock;
        }

        private SyntaxBase FunctionArgument(ExpressionFlags expressionFlags)
        {
            var expression = this.WithRecovery<SyntaxBase>(() =>
            {
                return this.Expression(expressionFlags);
            }, RecoveryFlags.None, TokenType.NewLine, TokenType.Comma, TokenType.RightParen);

            // always return a function argument syntax, even if we have skipped trivia
            // this simplifies calculations done to show argument completions and signature help
            return new FunctionArgumentSyntax(expression);
        }

        /// <summary>
        /// Method that gets a function call identifier, its arguments plus open and close parens
        /// </summary>
        protected (IdentifierSyntax Identifier, Token OpenParen, IEnumerable<SyntaxBase> ArgumentNodes, Token CloseParen) FunctionCallAccess(IdentifierSyntax functionName, ExpressionFlags expressionFlags)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));

            var itemsOrTokens = HandleFunctionElements(
                closingTokenType: TokenType.RightParen,
                parseChildElement: () => FunctionArgument(expressionFlags));

            var closeParen = this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")"));

            return (functionName, openParen, itemsOrTokens, closeParen);
        }

        private SyntaxBase FunctionCallOrVariableAccess(ExpressionFlags expressionFlags)
        {
            var identifier = this.Identifier(b => b.ExpectedVariableOrFunctionName());

            if (Check(TokenType.LeftParen))
            {
                var functionCall = FunctionCallAccess(identifier, expressionFlags);

                return new FunctionCallSyntax(
                    functionCall.Identifier,
                    functionCall.OpenParen,
                    functionCall.ArgumentNodes,
                    functionCall.CloseParen);
            }

            if (Check(TokenType.Arrow))
            {
                var arrow = this.Expect(TokenType.Arrow, b => b.ExpectedCharacter("=>"));
                var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);

                return new LambdaSyntax(new LocalVariableSyntax(identifier), arrow, expression);
            }

            // returns variable access
            return new VariableAccessSyntax(identifier);
        }

        protected Token? GetOptionalKeyword(string expectedKeyword)
        {
            if (this.CheckKeyword(expectedKeyword))
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            return null;
        }

        private ParenthesizedExpressionSyntax GetParenthesizedExpressionSyntax(Token openParen, ImmutableArray<SyntaxBase> expressionsOrCommas, SyntaxBase closeParen)
        {
            var bodyExpression = expressionsOrCommas.Length switch {
                0 => SkipEmpty(openParen.Span.GetEndPosition(), x => x.ParenthesesMustHaveExactlyOneItem()),
                1 when expressionsOrCommas[0] is Token token => Skip(token.AsEnumerable()),
                1 => expressionsOrCommas[0],
                _ => Skip(expressionsOrCommas, x => x.ParenthesesMustHaveExactlyOneItem()),
            };

            return new ParenthesizedExpressionSyntax(openParen, bodyExpression, closeParen);
        }

        private VariableBlockSyntax GetVariableBlock(Token openParen, ImmutableArray<SyntaxBase> expressionsOrCommas, SyntaxBase closeParen)
        {
            var rewritten = expressionsOrCommas.Select(item => item switch {
                VariableAccessSyntax varAccess => new LocalVariableSyntax(varAccess.Name),
                Token { Type: TokenType.Comma } => item,
                SkippedTriviaSyntax => item,
                _ => new SkippedTriviaSyntax(item.Span, item.AsEnumerable(), Enumerable.Empty<IDiagnostic>()),
            });

            return new VariableBlockSyntax(openParen, rewritten, closeParen);
        }

        private IEnumerable<SyntaxBase> HandleArrayOrObjectElements(TokenType closingTokenType, Func<SyntaxBase> parseChildElement)
        {
            if (Check(closingTokenType))
            {
                // always allow a close on the same line
                return ImmutableArray<SyntaxBase>.Empty;
            }

            var itemsOrTokens = new List<SyntaxBase>();

            itemsOrTokens.AddRange(NewLines());

            var expectElement = true;
            while (!this.IsAtEnd() && this.reader.Peek().Type != closingTokenType)
            {
                if (!expectElement)
                {
                    // every element should be separated by AT MOST one set of new lines, or one comma
                    // we don't want to allow mixing and matching, and we want to insert dummy elements between commas
                    if (Check(TokenType.NewLine))
                    {
                        itemsOrTokens.AddRange(NewLines());
                    }
                    else if (Check(TokenType.Comma))
                    {
                        itemsOrTokens.Add(reader.Read());
                        if (Check(TokenType.NewLine))
                        {
                            // this may be a common mistake for anyone converting from single- to multi-line
                            // array or object declarations. special-case the diagnostics to reduce confusion.
                            itemsOrTokens.Add(SkipEmpty(x => x.UnexpectedNewLineAfterCommaSeparator()));
                            itemsOrTokens.Add(NewLine());
                        }
                    }
                    else
                    {
                        itemsOrTokens.Add(SkipEmpty(x => x.ExpectedNewLineOrCommaSeparator()));
                    }

                    expectElement = true;
                    continue;
                }

                var itemOrToken = parseChildElement();
                itemsOrTokens.Add(itemOrToken);
                expectElement = false;
            }

            return itemsOrTokens;
        }

        private IEnumerable<SyntaxBase> HandleFunctionElements(TokenType closingTokenType, Func<SyntaxBase> parseChildElement)
        {
            if (Check(closingTokenType))
            {
                // always allow a close on the same line
                return ImmutableArray<SyntaxBase>.Empty;
            }

            var itemsOrTokens = new List<SyntaxBase>();

            itemsOrTokens.AddRange(NewLines());

            var expectElement = true;
            while (!this.IsAtEnd() && this.reader.Peek().Type != closingTokenType)
            {
                if (!expectElement)
                {
                    // every element should be separated by AT MOST one set of new lines, or one comma
                    // we don't want to allow mixing and matching, and we want to insert dummy elements between commas
                    if (Check(TokenType.NewLine))
                    {
                        if (!Check(this.reader.PeekAhead(), closingTokenType))
                        {
                            itemsOrTokens.Add(SkipEmpty(x => x.ExpectedCommaSeparator()));
                        }

                        itemsOrTokens.AddRange(NewLines());
                    }
                    else if (Check(TokenType.Comma))
                    {
                        itemsOrTokens.Add(reader.Read());
                        if (Check(TokenType.NewLine))
                        {
                            // newlines are optional after commas
                            itemsOrTokens.AddRange(NewLines());
                        }
                        if (Check(closingTokenType))
                        {
                            // trailing commas not supported - try to parse a child element before we exit the while loop,
                            // to give a chance to raise diagnostics, and generate a placeholder 'skipped' element to help pick the correct function overload.
                            var skippedItem = parseChildElement();
                            itemsOrTokens.Add(skippedItem);
                        }
                    }
                    else
                    {
                        itemsOrTokens.Add(SkipEmpty(x => x.ExpectedNewLineOrCommaSeparator()));
                    }

                    expectElement = true;
                    continue;
                }

                var itemOrToken = parseChildElement();
                itemsOrTokens.Add(itemOrToken);
                expectElement = false;
            }

            return itemsOrTokens;
        }

        protected IdentifierSyntax Identifier(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var identifier = Expect(TokenType.Identifier, errorFunc);

            return new IdentifierSyntax(identifier);
        }

        protected IdentifierSyntax IdentifierOrSkip(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            if (this.Check(TokenType.Identifier))
            {
                var identifier = Expect(TokenType.Identifier, errorFunc);
                return new IdentifierSyntax(identifier);
            }

            var skipped = SkipEmpty(errorFunc);
            return new IdentifierSyntax(skipped);
        }

        protected IdentifierSyntax IdentifierWithRecovery(DiagnosticBuilder.ErrorBuilderDelegate errorFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
        {
            var identifierOrSkipped = this.WithRecovery(
                () => Identifier(errorFunc),
                flags,
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

        protected SyntaxBase IfCondition(ExpressionFlags expressionFlags, bool insideForExpression)
        {
            var keyword = this.ExpectKeyword(LanguageConstants.IfKeyword);

            // when inside a for-expression, we must include ] as a recovery terminator
            // otherwise, the ] character may get consumed by recovery
            // then, the for-expression parsing will produce an "expected ] character" diagnostic, which is confusing
            var conditionExpression = this.WithRecovery(
                () => this.ParenthesizedExpression(WithoutExpressionFlag(expressionFlags, ExpressionFlags.AllowResourceDeclarations)),
                RecoveryFlags.None,
                insideForExpression ? new[] { TokenType.RightSquare, TokenType.LeftBrace, TokenType.NewLine } : new[] { TokenType.LeftBrace, TokenType.NewLine });
            var body = this.WithRecovery(
                () => this.Object(expressionFlags),
                GetSuppressionFlag(conditionExpression, conditionExpression is ParenthesizedExpressionSyntax { CloseParen: not SkippedTriviaSyntax }),
                insideForExpression ? new[] { TokenType.RightSquare, TokenType.NewLine } : new[] { TokenType.NewLine });
            return new IfConditionSyntax(keyword, conditionExpression, body);
        }

        protected SyntaxBase InterpolableString()
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
                    var interpExpression = WithRecovery(() => Expression(ExpressionFlags.None), RecoveryFlags.None, TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine);
                    if (!Check(TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine))
                    {
                        // We may have successfully parsed the expression, but have not reached the end of the expression hole. Skip to the end of the hole.
                        var skippedSyntax = SynchronizeAndReturnTrivia(reader.Position, RecoveryFlags.None, b => b.UnexpectedTokensInInterpolation(), TokenType.StringMiddlePiece, TokenType.StringRightPiece, TokenType.NewLine);

                        // Things start to get hairy to build the string if we return an uneven number of tokens and expressions.
                        // Rather than trying to add two expression nodes, combine them.
                        var combined = new[] { interpExpression, skippedSyntax };
                        interpExpression = new SkippedTriviaSyntax(TextSpan.Between(combined.First(), combined.Last()), combined, Enumerable.Empty<IDiagnostic>());
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
                    return new SkippedTriviaSyntax(span, tokensOrSyntax, Enumerable.Empty<IDiagnostic>());
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

        protected bool IsAtEnd()
        {
            return reader.IsAtEnd() || reader.Peek().Type == TokenType.EndOfFile;
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

                case TokenType.Integer:
                    return this.NumericLiteral();

                case TokenType.NullKeyword:
                    return new NullLiteralSyntax(reader.Read());

                default:
                    throw new ExpectedTokenException(current, b => b.InvalidType());
            }
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

        private SyntaxBase MemberExpression(ExpressionFlags expressionFlags)
        {
            var current = this.PrimaryExpression(expressionFlags);

            while (true)
            {
                if (this.Check(TokenType.LeftSquare))
                {
                    // array indexer
                    Token openSquare = this.reader.Read();

                    if (this.Check(TokenType.RightSquare))
                    {
                        // empty indexer - we are allowing this special case in the parser to help with completions
                        SyntaxBase skipped = SkipEmpty(b => b.EmptyIndexerNotAllowed());
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                        current = new ArrayAccessSyntax(current, openSquare, skipped, closeSquare);
                    }
                    else
                    {
                        SyntaxBase indexExpression = this.Expression(expressionFlags);
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                        current = new ArrayAccessSyntax(current, openSquare, indexExpression, closeSquare);
                    }

                    continue;
                }

                if (this.Check(TokenType.Dot))
                {
                    // dot operator
                    Token dot = this.reader.Read();

                    IdentifierSyntax identifier = this.IdentifierOrSkip(b => b.ExpectedFunctionOrPropertyName());

                    if (Check(TokenType.LeftParen))
                    {
                        var functionCall = FunctionCallAccess(identifier, expressionFlags);

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

                if (this.Check(TokenType.DoubleColon))
                {
                    var doubleColon = this.reader.Read();
                    var identifier = this.IdentifierOrSkip(b => b.ExpectedFunctionOrPropertyName());
                    current = new ResourceAccessSyntax(current, doubleColon, identifier);

                    continue;
                }

                break;
            }

            return current;
        }

        private SyntaxBase MultilineString()
        {
            var token = reader.Read();
            var stringValue = Lexer.TryGetMultilineStringValue(token);

            if (stringValue is null)
            {
                return new SkippedTriviaSyntax(token.Span, token.AsEnumerable(), Enumerable.Empty<IDiagnostic>());
            }

            return new StringSyntax(token.AsEnumerable(), Enumerable.Empty<SyntaxBase>(), stringValue.AsEnumerable());
        }

        protected Token NewLine()
        {
            return Expect(TokenType.NewLine, b => b.ExpectedNewLine());
        }

        protected Token? NewLineOrEof()
        {
            if (reader.Peek().Type == TokenType.EndOfFile)
            {
                // don't actually consume the token
                return null;
            }

            return NewLine();
        }

        private IEnumerable<Token> NewLines()
        {
            while (Check(TokenType.NewLine))
            {
                yield return this.NewLine();
            }
        }

        private IntegerLiteralSyntax NumericLiteral()
        {
            var literal = Expect(TokenType.Integer, b => b.ExpectedNumericLiteral());

            if (ulong.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out ulong value))
            {
                return new IntegerLiteralSyntax(literal, value);
            }

            // TODO: Should probably be moved to type checking
            // integer is invalid (too long to fit in an int32)
            throw new ExpectedTokenException(literal, b => b.InvalidInteger());
        }

        protected ObjectSyntax Object(ExpressionFlags expressionFlags)
        {
            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightBrace,
                parseChildElement: () => ObjectProperty(expressionFlags));

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new ObjectSyntax(openBrace, itemsOrTokens, closeBrace);
        }

        private SyntaxBase ObjectProperty(ExpressionFlags expressionFlags)
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                var current = this.reader.Peek();

                // Nested resource declarations may be allowed - but we need lookahead to avoid
                // treating 'resource' as a reserved property name.
                if (HasExpressionFlag(expressionFlags, ExpressionFlags.AllowResourceDeclarations) &&
                    CheckKeyword(LanguageConstants.ResourceKeyword) &&

                    // You are here: |resource <name> ...
                    //
                    // If we see a non-identifier then it's not a resource declaration,
                    // fall back to the property parser.
                    Check(this.reader.PeekAhead(), TokenType.Identifier))
                {
                    return this.Declaration();
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
                    TokenType.Colon, TokenType.NewLine, TokenType.RightBrace);

                var colon = this.WithRecovery(() => Expect(TokenType.Colon, b => b.ExpectedCharacter(":")), GetSuppressionFlag(key), TokenType.NewLine, TokenType.RightBrace);
                var value = this.WithRecovery(() => Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(colon), TokenType.NewLine, TokenType.RightBrace);

                return new ObjectPropertySyntax(key, colon, value);
            }, RecoveryFlags.None, TokenType.NewLine, TokenType.RightBrace);
        }

        private SyntaxBase ParenthesizedExpression(ExpressionFlags expressionFlags)
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(expressionFlags);

            return GetParenthesizedExpressionSyntax(openParen, expressionsOrCommas, closeParen);
        }

        private (Token openParen, ImmutableArray<SyntaxBase> expressionsOrCommas, SyntaxBase closeParen) ParenthesizedExpressionList(ExpressionFlags expressionFlags)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));
            var expressionsOrCommas = new List<SyntaxBase>();
            while (!this.Check(TokenType.RightParen))
            {
                var expression = this.WithRecovery(
                    () => this.Expression(expressionFlags),
                    RecoveryFlags.None,
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine,
                    TokenType.Comma);
                expressionsOrCommas.Add(expression);

                if (this.Check(TokenType.Comma))
                {
                    var comma = this.Expect(TokenType.Comma, b => b.ExpectedCharacter(","));
                    expressionsOrCommas.Add(comma);
                }
                else
                {
                    break;
                }
            }

            var closeParen = this.WithRecovery(
                () => this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")")),
                expressionsOrCommas.Any() ? GetSuppressionFlag(expressionsOrCommas.Last()) : RecoveryFlags.None,
                TokenType.StringRightPiece,
                TokenType.RightBrace,
                TokenType.RightSquare,
                TokenType.NewLine);

            return (openParen, expressionsOrCommas.ToImmutableArray(), closeParen);
        }

        private SyntaxBase ParenthesizedExpressionOrLambda(ExpressionFlags expressionFlags)
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(expressionFlags);

            if (Check(TokenType.Arrow))
            {
                var arrow = this.Expect(TokenType.Arrow, b => b.ExpectedCharacter("=>"));
                var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);
                var variableBlock = GetVariableBlock(openParen, expressionsOrCommas, closeParen);

                return new LambdaSyntax(variableBlock, arrow, expression);
            }

            return GetParenthesizedExpressionSyntax(openParen, expressionsOrCommas, closeParen);
        }

        private SyntaxBase PrimaryExpression(ExpressionFlags expressionFlags)
        {
            Token nextToken = this.reader.Peek();

            switch (nextToken.Type)
            {
                case TokenType.Integer:
                case TokenType.NullKeyword:
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    return this.LiteralValue();

                case TokenType.StringComplete:
                case TokenType.StringLeftPiece:
                    return this.InterpolableString();

                case TokenType.MultilineString:
                    return this.MultilineString();

                case TokenType.LeftBrace when HasExpressionFlag(expressionFlags, ExpressionFlags.AllowComplexLiterals):
                    return this.Object(expressionFlags);

                case TokenType.LeftSquare when HasExpressionFlag(expressionFlags, ExpressionFlags.AllowComplexLiterals):
                    return CheckKeyword(this.reader.PeekAhead(), LanguageConstants.ForKeyword)
                        ? this.ForExpression(expressionFlags, isResourceOrModuleContext: false)
                        : this.Array();

                case TokenType.LeftBrace:
                case TokenType.LeftSquare:
                    throw new ExpectedTokenException(nextToken, b => b.ComplexLiteralsNotAllowed());

                case TokenType.LeftParen:
                    return this.ParenthesizedExpressionOrLambda(expressionFlags);

                case TokenType.Identifier:
                    return this.FunctionCallOrVariableAccess(expressionFlags);

                default:
                    throw new ExpectedTokenException(nextToken, b => b.UnrecognizedExpression());
            }
        }

        private SkippedTriviaSyntax Skip(IEnumerable<SyntaxBase> syntax, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var syntaxArray = syntax.ToImmutableArray();
            var span = TextSpan.Between(syntaxArray.First(), syntaxArray.Last());
            return new SkippedTriviaSyntax(span, syntaxArray, errorFunc(DiagnosticBuilder.ForPosition(span)).AsEnumerable());
        }

        private SkippedTriviaSyntax Skip(IEnumerable<SyntaxBase> syntax)
        {
            var syntaxArray = syntax.ToImmutableArray();
            var span = TextSpan.Between(syntaxArray.First(), syntaxArray.Last());
            return new SkippedTriviaSyntax(span, syntaxArray, Enumerable.Empty<Diagnostic>());
        }

        protected SkippedTriviaSyntax SkipEmpty()
            => SkipEmpty(this.reader.Peek().Span.Position, null);

        private SkippedTriviaSyntax SkipEmpty(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
            => SkipEmpty(this.reader.Peek().Span.Position, errorFunc);

        private SkippedTriviaSyntax SkipEmpty(int position, DiagnosticBuilder.ErrorBuilderDelegate? errorFunc)
        {
            var span = new TextSpan(position, 0);
            var diagnostics = errorFunc is null ? Enumerable.Empty<IDiagnostic>() : errorFunc(DiagnosticBuilder.ForPosition(span)).AsEnumerable();
            return new SkippedTriviaSyntax(span, ImmutableArray<SyntaxBase>.Empty, diagnostics);
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

        protected SyntaxBase ThrowIfSkipped(Func<SyntaxBase> syntaxFunc, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var startToken = reader.Peek();
            var syntax = syntaxFunc();

            if (syntax.IsSkipped)
            {
                throw new ExpectedTokenException(startToken, errorFunc);
            }

            return syntax;
        }

        protected TypeSyntax Type(DiagnosticBuilder.ErrorBuilderDelegate errorFunc, bool allowOptionalResourceType)
        {
            if (GetOptionalKeyword(LanguageConstants.ResourceKeyword) is {} resourceKeyword)
            {
                var type = this.WithRecoveryNullable(
                    () =>
                    {
                        // The resource type is optional for an output
                        if (allowOptionalResourceType && !this.Check(this.reader.Peek(), TokenType.StringComplete, TokenType.StringLeftPiece))
                        {
                            return null;
                        }
                        else
                        {
                            return ThrowIfSkipped(this.InterpolableString, b => b.ExpectedResourceTypeString());
                        }
                    },
                    RecoveryFlags.None,
                    TokenType.Assignment, TokenType.NewLine);
                return new ResourceTypeSyntax(resourceKeyword, type);
            }

            var identifier = Expect(TokenType.Identifier, errorFunc);
            return new SimpleTypeSyntax(identifier);
        }

        private SyntaxBase UnaryExpression(ExpressionFlags expressionFlags)
        {
            Token operatorToken = this.reader.Peek();

            if (Operators.TokenTypeToUnaryOperator.TryGetValue(operatorToken.Type, out var @operator))
            {
                this.reader.Read();

                var expression = this.WithRecovery(
                    () => this.MemberExpression(expressionFlags),
                    RecoveryFlags.None,
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine);

                return new UnaryOperationSyntax(operatorToken, expression);
            }

            return this.MemberExpression(expressionFlags);
        }

        protected SyntaxBase WithRecovery<TSyntax>(Func<TSyntax> syntaxFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
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

        protected SyntaxBase? WithRecoveryNullable<TSyntax>(Func<TSyntax> syntaxFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
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
    }
}
