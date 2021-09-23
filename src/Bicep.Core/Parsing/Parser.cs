// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class Parser
    {
        private readonly TokenReader reader;

        private readonly ImmutableArray<IDiagnostic> lexerDiagnostics;

        public Parser(string text)
        {
            // treating the lexer as an implementation detail of the parser
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            this.lexerDiagnostics = diagnosticWriter.GetDiagnostics().ToImmutableArray();

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
                if (declarationOrToken is ITopLevelDeclarationSyntax)
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
                    List<SyntaxBase> leadingNodes = new();

                    // Parse decorators before the declaration.
                    while (this.Check(TokenType.At))
                    {
                        leadingNodes.Add(this.Decorator());

                        // A decorators must followed by a newline.
                        leadingNodes.Add(this.WithRecovery(this.NewLine, RecoveryFlags.ConsumeTerminator, TokenType.NewLine));


                        while (this.Check(TokenType.NewLine))
                        {
                            // In case there are skipped trivial syntaxes after a decorator, we need to consume
                            // all the newlines after them.
                            leadingNodes.Add(this.NewLine());
                        }
                    }

                    Token current = reader.Peek();

                    return current.Type switch
                    {
                        TokenType.Identifier => current.Text switch
                        {
                            LanguageConstants.TargetScopeKeyword => this.TargetScope(leadingNodes),
                            LanguageConstants.ParameterKeyword => this.ParameterDeclaration(leadingNodes),
                            LanguageConstants.VariableKeyword => this.VariableDeclaration(leadingNodes),
                            LanguageConstants.ResourceKeyword => this.ResourceDeclaration(leadingNodes),
                            LanguageConstants.OutputKeyword => this.OutputDeclaration(leadingNodes),
                            LanguageConstants.ModuleKeyword => this.ModuleDeclaration(leadingNodes),
                            LanguageConstants.ImportKeyword => this.ImportDeclaration(leadingNodes),
                            _ => leadingNodes.Count > 0
                                ? new MissingDeclarationSyntax(leadingNodes)
                                : throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                        },
                        TokenType.NewLine => this.NewLine(),

                        _ => leadingNodes.Count > 0
                            ? new MissingDeclarationSyntax(leadingNodes)
                            : throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                    };
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private static RecoveryFlags GetSuppressionFlag(SyntaxBase precedingNode)
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

        private SyntaxBase TargetScope(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.TargetScopeKeyword);
            var assignment = this.WithRecovery(this.Assignment, RecoveryFlags.None, TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new TargetScopeSyntax(leadingNodes, keyword, assignment, value);
        }

        private SyntaxBase Decorator()
        {
            Token at = this.Expect(TokenType.At, b => b.ExpectedCharacter("@"));
            SyntaxBase expression = this.WithRecovery(() =>
            {
                SyntaxBase current;
                IdentifierSyntax identifier = this.Identifier(b => b.ExpectedNamespaceOrDecoratorName());

                if (Check(TokenType.LeftParen))
                {
                    var functionCall = FunctionCallAccess(identifier, ExpressionFlags.AllowComplexLiterals);

                    current = new FunctionCallSyntax(
                        functionCall.Identifier,
                        functionCall.OpenParen,
                        functionCall.ArgumentNodes,
                        functionCall.CloseParen);
                }
                else
                {
                    current = new VariableAccessSyntax(identifier);
                }


                while (this.Check(TokenType.Dot))
                {
                    Token dot = this.reader.Read();
                    identifier = this.IdentifierOrSkip(b => b.ExpectedFunctionOrPropertyName());

                    if (Check(TokenType.LeftParen))
                    {
                        var functionCall = FunctionCallAccess(identifier, ExpressionFlags.AllowComplexLiterals);

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
                }

                return current;
            },
            RecoveryFlags.None,
            TokenType.NewLine);

            return new DecoratorSyntax(at, expression);
        }

        private SyntaxBase ParameterDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ParameterKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
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

                        _ => throw new ExpectedTokenException(current, b => b.ExpectedParameterContinuation())
                    };
                },
                GetSuppressionFlag(type),
                TokenType.NewLine);

            return new ParameterDeclarationSyntax(leadingNodes, keyword, name, type, modifier);
        }

        private SyntaxBase ParameterDefaultValue()
        {
            var assignmentToken = this.Expect(TokenType.Assignment, b => b.ExpectedCharacter("="));
            SyntaxBase defaultValue = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new ParameterDefaultValueSyntax(assignmentToken, defaultValue);
        }

        private SyntaxBase VariableDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.VariableKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedVariableIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new VariableDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }

        private SyntaxBase OutputDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.OutputKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedOutputIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(b => b.ExpectedOutputType()), GetSuppressionFlag(name), TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new OutputDeclarationSyntax(leadingNodes, keyword, name, type, assignment, value);
        }

        private SyntaxBase ResourceDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ResourceKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedResourceIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var type = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedResourceTypeString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var existingKeyword = GetOptionalKeyword(LanguageConstants.ExistingKeyword);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.LeftBrace, TokenType.NewLine);

            var value = this.WithRecovery(() =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        TokenType.Identifier when current.Text == LanguageConstants.IfKeyword => this.IfCondition(ExpressionFlags.AllowResourceDeclarations | ExpressionFlags.AllowComplexLiterals, insideForExpression: false),
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowResourceDeclarations | ExpressionFlags.AllowComplexLiterals),
                        TokenType.LeftSquare => this.ForExpression(ExpressionFlags.AllowResourceDeclarations | ExpressionFlags.AllowComplexLiterals, isResourceOrModuleContext: true),
                        _ => throw new ExpectedTokenException(current, b => b.ExpectBodyStartOrIfOrLoopStart())
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return new ResourceDeclarationSyntax(leadingNodes, keyword, name, type, existingKeyword, assignment, value);
        }

        private SyntaxBase ModuleDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ModuleKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedModuleIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var path = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedModulePathString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(path), TokenType.LeftBrace, TokenType.NewLine);
            var value = this.WithRecovery(() =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        TokenType.Identifier when current.Text == LanguageConstants.IfKeyword => this.IfCondition(ExpressionFlags.AllowComplexLiterals, insideForExpression: false),
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowComplexLiterals),
                        TokenType.LeftSquare => this.ForExpression(ExpressionFlags.AllowComplexLiterals, isResourceOrModuleContext: true),
                        _ => throw new ExpectedTokenException(current, b => b.ExpectBodyStartOrIfOrLoopStart())
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return new ModuleDeclarationSyntax(leadingNodes, keyword, name, path, assignment, value);
        }

        private ImportDeclarationSyntax ImportDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ImportKeyword);
            var aliasName = this.IdentifierWithRecovery(b => b.ExpectedImportAliasName(), RecoveryFlags.None, TokenType.NewLine);
            var fromKeyword = this.WithRecovery(() => this.ExpectKeyword(LanguageConstants.FromKeyword), GetSuppressionFlag(aliasName), TokenType.NewLine);
            var providerName = this.IdentifierWithRecovery(b => b.ExpectedImportProviderName(), GetSuppressionFlag(fromKeyword), TokenType.NewLine);
            var config = this.WithRecoveryNullable(
                () =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        // no config is supplied
                        TokenType.NewLine => null,
                        TokenType.EndOfFile => null,

                        // we have config!
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowComplexLiterals),
                        _ => throw new ExpectedTokenException(current, b => b.ExpectedCharacter("{")),
                    };
                },
                GetSuppressionFlag(providerName),
                TokenType.NewLine);

            return new(leadingNodes, keyword, aliasName, fromKeyword, providerName, config);
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
                    return this.ParenthesizedExpression(expressionFlags);

                case TokenType.Identifier:
                    return this.FunctionCallOrVariableAccess(expressionFlags);

                default:
                    throw new ExpectedTokenException(nextToken, b => b.UnrecognizedExpression());
            }
        }

        private SyntaxBase ParenthesizedExpression(ExpressionFlags expressionFlags)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));
            var expression = this.WithRecovery(
                () => this.Expression(expressionFlags),
                RecoveryFlags.None,
                TokenType.StringRightPiece,
                TokenType.RightBrace,
                TokenType.RightParen,
                TokenType.RightSquare,
                TokenType.NewLine);
            var closeParen = this.WithRecovery(
                () => this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")")),
                GetSuppressionFlag(expression),
                TokenType.StringRightPiece,
                TokenType.RightBrace,
                TokenType.RightSquare,
                TokenType.NewLine);

            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
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
            // returns variable access
            return new VariableAccessSyntax(identifier);
        }

        /// <summary>
        /// Method that gets a function call identifier, its arguments plus open and close parens
        /// </summary>
        private (IdentifierSyntax Identifier, Token OpenParen, IEnumerable<FunctionArgumentSyntax> ArgumentNodes, Token CloseParen) FunctionCallAccess(IdentifierSyntax functionName, ExpressionFlags expressionFlags)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));

            var argumentNodes = FunctionCallArguments(expressionFlags);

            var closeParen = this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")"));

            return (functionName, openParen, argumentNodes, closeParen);
        }

        /// <summary>
        /// Method that consumes and returns all arguments from an Instance of Function call.
        /// This method stops when a right paren is found without consuming it, a caller must
        /// consume the right paren token.
        /// </summary>
        /// <param name="allowComplexLiterals"></param>
        private IEnumerable<FunctionArgumentSyntax> FunctionCallArguments(ExpressionFlags expressionFlags)
        {
            SkippedTriviaSyntax CreateDummyArgument(Token current) =>
                new SkippedTriviaSyntax(current.ToZeroLengthSpan(), ImmutableArray<SyntaxBase>.Empty, DiagnosticBuilder.ForPosition(current.ToZeroLengthSpan()).UnrecognizedExpression().AsEnumerable());

            if (this.Check(TokenType.RightParen))
            {
                return ImmutableArray<FunctionArgumentSyntax>.Empty;
            }

            var arguments = new List<(SyntaxBase expression, Token? comma)>();

            while (true)
            {
                var current = this.reader.Peek();
                switch (current.Type)
                {
                    case TokenType.Comma:
                        this.reader.Read();
                        if (arguments.Any() && arguments[^1].comma == null)
                        {
                            // we have consumed an expression which is expecting a comma
                            // add the comma to the expression (it's a value type)
                            var lastArgument = arguments[^1];
                            lastArgument.comma = current;
                            arguments[^1] = lastArgument;
                        }
                        else
                        {
                            // there aren't any arguments or the expression already has a comma following it
                            // add an empty expression with the comma
                            arguments.Add((CreateDummyArgument(current), current));
                        }

                        break;

                    case TokenType.RightParen:
                        // end of function call

                        if (arguments.Any() && arguments.Last().comma != null)
                        {
                            // we have a trailing comma without an argument
                            // we need to allow it so signature help doesn't get interrupted
                            // by the user typing a comma without specifying a function argument

                            // insert a dummy argument
                            arguments.Add((CreateDummyArgument(current), null));
                        }

                        // return the accumulated arguments without consuming the right paren (the caller must consume it)
                        var functionArguments = new List<FunctionArgumentSyntax>(arguments.Count);
                        foreach (var (argumentExpression, comma) in arguments)
                        {
                            functionArguments.Add(new FunctionArgumentSyntax(argumentExpression, comma));
                        }
                        return functionArguments.ToImmutableArray();

                    default:
                        // not a comma or )
                        // it should be an expression
                        if (arguments.Any() && arguments[^1].comma == null)
                        {
                            // we are expecting a comma after the previous expression
                            throw new ExpectedTokenException(current, b => b.ExpectedCharacter(","));
                        }

                        var expression = this.Expression(expressionFlags);
                        arguments.Add((expression, null));

                        break;
                }
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

        private IdentifierSyntax IdentifierOrSkip(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            if (this.Check(TokenType.Identifier))
            {
                var identifier = Expect(TokenType.Identifier, errorFunc);
                return new IdentifierSyntax(identifier);
            }

            var skipped = SkipEmpty(errorFunc);
            return new IdentifierSyntax(skipped);
        }

        private IdentifierSyntax IdentifierWithRecovery(DiagnosticBuilder.ErrorBuilderDelegate errorFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
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

        private SkippedTriviaSyntax SkipEmpty(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var span = new TextSpan(this.reader.Peek().Span.Position, 0);
            return new SkippedTriviaSyntax(span, ImmutableArray<SyntaxBase>.Empty, errorFunc(DiagnosticBuilder.ForPosition(span)).AsEnumerable());
        }

        private TypeSyntax Type(DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
        {
            var identifier = Expect(TokenType.Identifier, errorFunc);

            return new TypeSyntax(identifier);
        }

        private IntegerLiteralSyntax NumericLiteral()
        {
            var literal = Expect(TokenType.Integer, b => b.ExpectedNumericLiteral());

            if (long.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out long value))
            {
                return new IntegerLiteralSyntax(literal, value);
            }

            // TODO: Should probably be moved to type checking
            // integer is invalid (too long to fit in an int32)
            throw new ExpectedTokenException(literal, b => b.InvalidInteger());
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

        private ForSyntax ForExpression(ExpressionFlags expressionFlags, bool isResourceOrModuleContext)
        {
            var openBracket = this.Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));
            var forKeyword = this.ExpectKeyword(LanguageConstants.ForKeyword);
            var variableSection = this.reader.Peek().Type switch
            {
                TokenType.Identifier => (SyntaxBase)new LocalVariableSyntax(this.Identifier(b => b.ExpectedLoopVariableIdentifier())),
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

        private ForVariableBlockSyntax ForVariableBlock()
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));
            var itemVariable = new LocalVariableSyntax(this.IdentifierWithRecovery(b => b.ExpectedLoopVariableIdentifier(), RecoveryFlags.None, TokenType.Comma, TokenType.RightParen, TokenType.NewLine));
            var comma = this.WithRecovery(() => this.Expect(TokenType.Comma, b => b.ExpectedCharacter(",")), GetSuppressionFlag(itemVariable.Name), TokenType.Identifier, TokenType.RightParen, TokenType.NewLine);
            var indexVariable = new LocalVariableSyntax(this.IdentifierWithRecovery(b => b.ExpectedLoopIndexIdentifier(), GetSuppressionFlag(comma), TokenType.RightParen, TokenType.NewLine));
            var closeParen = this.WithRecovery(() => this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")")), GetSuppressionFlag(indexVariable.Name), TokenType.NewLine);

            return new(openParen, itemVariable, comma, indexVariable, closeParen);
        }

        private SyntaxBase ForBody(ExpressionFlags expressionFlags, bool isResourceOrModuleContext)
        {
            if(!isResourceOrModuleContext)
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

        private SyntaxBase Array()
        {
            var openBracket = Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));

            if (Check(TokenType.RightSquare))
            {
                // allow a close on the same line for an empty array
                var emptyCloseBracket = reader.Read();
                return new ArraySyntax(openBracket, ImmutableArray<SyntaxBase>.Empty, emptyCloseBracket);
            }

            var itemsOrTokens = new List<SyntaxBase>();

            while (!this.IsAtEnd() && this.reader.Peek().Type != TokenType.RightSquare)
            {
                // this produces an item node, skipped tokens node, or just a newline token
                var itemOrToken = this.ArrayItem();
                itemsOrTokens.Add(itemOrToken);

                // if skipped tokens node is returned above, the newline is not consumed
                // if newline token is returned, we must not expect another (could be beginning of a new item)
                if (itemOrToken is ArrayItemSyntax)
                {
                    if (Check(TokenType.Comma))
                    {
                        var token = this.reader.Read();
                        var skippedSyntax = new SkippedTriviaSyntax(
                            token.Span,
                            token.AsEnumerable(),
                            DiagnosticBuilder.ForPosition(token.Span).UnexpectedCommaSeparator().AsEnumerable()
                        );
                        itemsOrTokens.Add(skippedSyntax);
                    }

                    // items must be followed by newlines
                    var newLine = this.WithRecoveryNullable(this.NewLineOrEof, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
                    if (newLine != null)
                    {
                        itemsOrTokens.Add(newLine);
                    }
                }
            }

            var closeBracket = Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

            return new ArraySyntax(openBracket, itemsOrTokens, closeBracket);
        }

        private SyntaxBase ArrayItem()
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                var current = this.reader.Peek();

                if (current.Type == TokenType.NewLine)
                {
                    return this.NewLine();
                }

                var value = this.Expression(ExpressionFlags.AllowComplexLiterals);
                return new ArrayItemSyntax(value);
            }, RecoveryFlags.None, TokenType.NewLine);
        }

        private ObjectSyntax Object(ExpressionFlags expressionFlags)
        {
            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));

            if (Check(TokenType.RightBrace))
            {
                // allow a close on the same line for an empty object
                var emptyCloseBrace = reader.Read();
                return new ObjectSyntax(openBrace, ImmutableArray<SyntaxBase>.Empty, emptyCloseBrace);
            }

            var propertiesOrResourcesTokens = new List<SyntaxBase>();
            while (!this.IsAtEnd() && this.reader.Peek().Type != TokenType.RightBrace)
            {
                // this produces a property node, skipped tokens node, or just a newline token
                var propertyOrResourceOrToken = this.ObjectProperty(expressionFlags);
                propertiesOrResourcesTokens.Add(propertyOrResourceOrToken);

                // if skipped tokens node is returned above, the newline is not consumed
                // if newline token is returned, we must not expect another (could be beginning of a new property)
                if (propertyOrResourceOrToken is ObjectPropertySyntax)
                {
                    if (Check(TokenType.Comma))
                    {
                        var token = this.reader.Read();
                        var skippedSyntax = new SkippedTriviaSyntax(
                            token.Span,
                            token.AsEnumerable(),
                            DiagnosticBuilder.ForPosition(token.Span).UnexpectedCommaSeparator().AsEnumerable()
                        );
                        propertiesOrResourcesTokens.Add(skippedSyntax);
                    }

                    // properties must be followed by newlines
                    var newLine = this.WithRecoveryNullable(this.NewLineOrEof, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
                    if (newLine != null)
                    {
                        propertiesOrResourcesTokens.Add(newLine);
                    }
                }
            }

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new ObjectSyntax(openBrace, propertiesOrResourcesTokens, closeBrace);
        }

        private SyntaxBase ObjectProperty(ExpressionFlags expressionFlags)
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                // TODO: Clean this up a bit
                var current = this.reader.Peek();
                if (current.Type == TokenType.NewLine)
                {
                    return this.NewLine();
                }

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
                    TokenType.Colon, TokenType.NewLine);

                var colon = this.WithRecovery(() => Expect(TokenType.Colon, b => b.ExpectedCharacter(":")), GetSuppressionFlag(key), TokenType.NewLine);
                var value = this.WithRecovery(() => Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(colon), TokenType.NewLine);

                return new ObjectPropertySyntax(key, colon, value);
            }, RecoveryFlags.None, TokenType.NewLine);
        }

        private SyntaxBase IfCondition(ExpressionFlags expressionFlags, bool insideForExpression)
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

        private static bool CheckKeyword(Token? token, string keyword) => token?.Type == TokenType.Identifier && token.Text == keyword;

        private bool CheckKeyword(string keyword) => !this.IsAtEnd() && CheckKeyword(this.reader.Peek(), keyword);

        private Token ExpectKeyword(string expectedKeyword)
        {
            return GetOptionalKeyword(expectedKeyword) ?? 
                throw new ExpectedTokenException(this.reader.Peek(), b => b.ExpectedKeyword(expectedKeyword));
        }

        private Token? GetOptionalKeyword(string expectedKeyword)
        {
            if (this.CheckKeyword(expectedKeyword))
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            return null;
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

        private bool Check(Token? token, params TokenType[] types)
        {
            if (token is null)
            {
                return false;
            }

            return types.Contains(token.Type);
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

                case TokenType.DoubleQuestion:
                    return 30;

                default:
                    return -1;
            }
        }
    }
}
