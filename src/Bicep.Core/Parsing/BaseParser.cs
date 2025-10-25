// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Parsing
{
    public abstract class BaseParser
    {
        protected readonly TokenReader reader;

        protected BaseParser(string text)
        {
            // treating the lexer as an implementation detail of the parser
            var lexingErrorTree = new DiagnosticTree();
            var lexer = new Lexer(new SlidingTextWindow(text), lexingErrorTree);
            lexer.Lex();

            this.reader = new TokenReader(lexer.GetTokens());

            this.ParsingErrorTree = new DiagnosticTree();
            this.LexingErrorLookup = lexingErrorTree;
        }

        protected DiagnosticTree ParsingErrorTree { get; }

        public IDiagnosticLookup LexingErrorLookup { get; }

        public IDiagnosticLookup ParsingErrorLookup => ParsingErrorTree;

        protected SyntaxBase VariableDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.VariableKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedVariableIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var type = WithRecoveryNullable(() => reader.Peek().Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine or
                TokenType.Assignment => null,
                _ => Type(allowOptionalResourceType: false),
            }, GetSuppressionFlag(name), TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type ?? name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new VariableDeclarationSyntax(leadingNodes, keyword, name, type, assignment, value);
        }

        protected SyntaxBase TypeDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.TypeKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedTypeIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => Type(allowOptionalResourceType: false), GetSuppressionFlag(name), TokenType.Assignment, TokenType.LeftBrace, TokenType.NewLine);

            return new TypeDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }

        protected CompileTimeImportDeclarationSyntax CompileTimeImportDeclaration(Token keyword, IEnumerable<SyntaxBase> leadingNodes)
        {
            SyntaxBase importExpression = reader.Peek().Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine or
                TokenType.Identifier => SkipEmpty(b => b.ExpectedSymbolListOrWildcard()),
                TokenType.LeftBrace => ImportedSymbolsList(),
                TokenType.Asterisk => WithRecovery(WildcardImport, GetSuppressionFlag(keyword), TokenType.NewLine),
                _ => Skip(reader.Read(), b => b.ExpectedSymbolListOrWildcard()),
            };

            return new(leadingNodes,
                keyword,
                importExpression,
                WithRecovery(CompileTimeImportFromClause, GetSuppressionFlag(keyword), TokenType.NewLine));
        }

        private static bool CheckKeyword(Token? token, string keyword) => token?.Type == TokenType.Identifier && token.Text == keyword;

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

            var newlinesBeforeQuestion =
                this.reader.Peek(skipNewlines: true).IsOf(TokenType.Question)
                    ? this.NewLines().ToImmutableArray()
                    : [];

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

                var newlinesBeforeColon =
                    !trueExpression.IsSkipped && this.reader.Peek(skipNewlines: true).IsOf(TokenType.Colon)
                        ? this.NewLines().ToImmutableArray()
                        : [];

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

                return new TernaryOperationSyntax(candidate, newlinesBeforeQuestion, question, trueExpression, newlinesBeforeColon, colon, falseExpression);
            }

            return candidate;
        }

        public abstract ProgramSyntax Program();

        protected abstract SyntaxBase Declaration(params string[] expectedKeywords);

        private SyntaxBase Array()
        {
            var openBracket = Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightSquare,
                parseChildElement: ArrayElement);

            var closeBracket = Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

            return new ArraySyntax(openBracket, itemsOrTokens, closeBracket);
        }

        private SyntaxBase ArrayElement()
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                var current = this.reader.Peek();

                if (current.Type == TokenType.Ellipsis)
                {
                    return SpreadExpression([TokenType.Comma, TokenType.NewLine, TokenType.RightSquare]);
                }

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

                int operatorPrecedence = TokenTypeHelper.GetOperatorPrecedence(candidateOperatorToken.Type);

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

        protected bool CheckTrivia(ImmutableArray<SyntaxTrivia>? trivia, params SyntaxTriviaType[] types)
        {
            if (trivia is null || trivia?.IsEmpty is true)
            {
                return false;
            }
            var isTypes = false;
            foreach (var trivium in trivia.GetValueOrDefault())
            {
                if (types.Contains(trivium.Type))
                {
                    isTypes = true;
                    break;
                }
            }
            return isTypes;
        }

        private bool CheckKeyword(string keyword) => !this.IsAtEnd() && CheckKeyword(this.reader.Peek(), keyword);

        protected Token Expect(TokenType type, DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
        {
            if (this.Check(type))
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            throw new ExpectedTokenException(this.reader.Peek(), errorFunc);
        }

        protected Token ExpectKeyword(string expectedKeyword, DiagnosticBuilder.DiagnosticBuilderDelegate? errorFunc = null)
        {
            errorFunc ??= b => b.ExpectedKeyword(expectedKeyword);
            return GetOptionalKeyword(expectedKeyword) ??
                throw new ExpectedTokenException(this.reader.Peek(), errorFunc);
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
            var openNewlines = this.NewLines().ToImmutableArray();
            var forKeyword = this.ExpectKeyword(LanguageConstants.ForKeyword);
            SyntaxBase variableSection = this.reader.Peek().Type switch
            {
                TokenType.Identifier => new LocalVariableSyntax(this.Identifier(b => b.ExpectedLoopVariableIdentifier())),
                TokenType.LeftParen => this.ForVariableBlock(),
                _ => this.SkipEmpty(b => b.ExpectedLoopItemIdentifierOrVariableBlockStart())
            };

            var inKeyword = this.WithRecovery(() => this.ExpectKeyword(LanguageConstants.InKeyword), this.HasSyntaxError(variableSection) ? RecoveryFlags.SuppressDiagnostics : RecoveryFlags.None, TokenType.RightSquare, TokenType.NewLine);
            var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(inKeyword), TokenType.Colon, TokenType.RightSquare, TokenType.NewLine);
            var colon = this.WithRecovery(() => this.Expect(TokenType.Colon, b => b.ExpectedCharacter(":")), GetSuppressionFlag(expression), TokenType.RightSquare, TokenType.NewLine);
            var body = this.WithRecovery(
                () => this.ForBody(expressionFlags, isResourceOrModuleContext),
                GetSuppressionFlag(colon),
                TokenType.RightSquare, TokenType.NewLine);
            var closeNewlines = body.IsSkipped
                ? []
                : this.NewLines().ToImmutableArray();
            var closeBracket = this.WithRecovery(() => this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]")), GetSuppressionFlag(body), TokenType.RightSquare, TokenType.NewLine);

            return new(openBracket, openNewlines, forKeyword, variableSection, inKeyword, expression, colon, body, closeNewlines, closeBracket);
        }

        private SyntaxBase ForVariableBlock()
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(() => Expression(ExpressionFlags.None), permitNewLines: false);

            var variableBlock = GetVariableBlock(openParen, expressionsOrCommas, closeParen);

            if (variableBlock.Arguments.Length != 2 && !this.HasSyntaxError(variableBlock))
            {
                return Skip(variableBlock.AsEnumerable(), x => x.ExpectedLoopVariableBlockWith2Elements(variableBlock.Arguments.Length));
            }

            return variableBlock;
        }

        private SyntaxBase FunctionArgument(ExpressionFlags expressionFlags)
        {
            var expression = this.WithRecovery<SyntaxBase>(() => Expression(expressionFlags), RecoveryFlags.None, TokenType.NewLine, TokenType.Comma, TokenType.RightParen);

            // always return a function argument syntax, even if we have skipped trivia
            // this simplifies calculations done to show argument completions and signature help
            return new FunctionArgumentSyntax(expression);
        }

        /// <summary>
        /// Method that gets a function call identifier, its arguments plus open and close parens
        /// </summary>
        protected (IdentifierSyntax Identifier, Token OpenParen, IEnumerable<SyntaxBase> ArgumentNodes, SyntaxBase CloseParen) FunctionCallAccess(IdentifierSyntax functionName, ExpressionFlags expressionFlags)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));

            var itemsOrTokens = HandleFunctionElements(
                closingTokenType: TokenType.RightParen,
                parseChildElement: () => FunctionArgument(expressionFlags));


            SyntaxBase closeParen = Check(TokenType.RightParen)
                ? reader.Read()
                : SkipEmpty(b => b.ExpectedCharacter(")"));

            return (functionName, openParen, itemsOrTokens, closeParen);
        }

        private SyntaxBase FunctionCallOrVariableAccess(ExpressionFlags expressionFlags)
        {
            var identifierToken = Expect(TokenType.Identifier, b => b.ExpectedVariableOrFunctionName());
            var identifier = new IdentifierSyntax(identifierToken);

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
                var next = this.reader.Peek(skipNewlines: true);
                var newlinesBeforeBody = !LanguageConstants.DeclarationKeywords.Contains(next.Text)
                    ? this.NewLines().ToImmutableArray()
                    : [];
                var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);

                return new LambdaSyntax(new LocalVariableSyntax(identifier), arrow, newlinesBeforeBody, expression);
            }

            return new VariableAccessSyntax(identifier);
        }

        private SyntaxBase ParameterizedTypeArgument()
        {
            var expression = this.WithRecovery(TypeExpression, RecoveryFlags.None, TokenType.NewLine, TokenType.Comma, TokenType.RightChevron);

            return new ParameterizedTypeArgumentSyntax(expression);
        }

        protected (IdentifierSyntax Identifier, Token OpenChevron, IEnumerable<SyntaxBase> ParameterNodes, SyntaxBase CloseChevron) ParameterizedTypeInstantiation(IdentifierSyntax parameterizedTypeName)
        {
            var openChevron = this.Expect(TokenType.LeftChevron, b => b.ExpectedCharacter("<"));

            var itemsOrTokens = HandleFunctionElements(
                closingTokenType: TokenType.RightChevron,
                parseChildElement: ParameterizedTypeArgument);

            SyntaxBase closeChevron = Check(TokenType.RightChevron)
                ? reader.Read()
                : SkipEmpty(b => b.ExpectedCharacter(">"));

            return (parameterizedTypeName, openChevron, itemsOrTokens, closeChevron);
        }

        private TypeSyntax ParameterizedTypeOrTypeVariableAccess()
        {
            var identifierToken = Expect(TokenType.Identifier, b => b.ExpectedTypeIdentifier());
            var identifier = new IdentifierSyntax(identifierToken);

            if (Check(TokenType.LeftChevron))
            {
                var parameterizedType = ParameterizedTypeInstantiation(identifier);

                return new ParameterizedTypeInstantiationSyntax(
                    parameterizedType.Identifier,
                    parameterizedType.OpenChevron,
                    parameterizedType.ParameterNodes,
                    parameterizedType.CloseChevron);
            }

            return new TypeVariableAccessSyntax(identifier);
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

        private SyntaxBase GetParenthesizedExpressionInnerContent(Token openParen, ImmutableArray<SyntaxBase> expressionsOrCommas, SyntaxBase closeParen)
            => expressionsOrCommas.Length switch
            {
                0 => SkipEmpty(openParen.Span.GetEndPosition(), x => x.ParenthesesMustHaveExactlyOneItem()),
                1 when expressionsOrCommas[0] is Token token => Skip(token.AsEnumerable()),
                1 => expressionsOrCommas[0],
                _ => Skip(expressionsOrCommas, x => x.ParenthesesMustHaveExactlyOneItem()),
            };

        private VariableBlockSyntax GetVariableBlock(Token openParen, ImmutableArray<SyntaxBase> expressionsOrCommas, SyntaxBase closeParen)
        {
            var rewritten = expressionsOrCommas.Select(item => item switch
            {
                VariableAccessSyntax varAccess => new LocalVariableSyntax(varAccess.Name),
                Token { Type: TokenType.Comma or TokenType.NewLine } => item,
                SkippedTriviaSyntax => item,
                _ => new SkippedTriviaSyntax(item.Span, item.AsEnumerable()),
            });

            return new VariableBlockSyntax(openParen, rewritten, closeParen);
        }

        protected IEnumerable<SyntaxBase> HandleArrayOrObjectElements(TokenType closingTokenType, Func<SyntaxBase> parseChildElement)
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

        private IEnumerable<SyntaxBase> HandleFunctionElements(
            TokenType closingTokenType,
            Func<SyntaxBase> parseChildElement)
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
                        var peekPosition = 1;
                        while (Check(this.reader.PeekAhead(peekPosition), TokenType.NewLine) && CheckTrivia(this.reader.PeekAhead(peekPosition)?.LeadingTrivia, [SyntaxTriviaType.SingleLineComment, SyntaxTriviaType.MultiLineComment]))
                        {
                            // Check End of comments for closingTokenType
                            peekPosition++;
                        }

                        if (this.reader.PeekAhead(peekPosition) is null or { Type: TokenType.EndOfFile })
                        {
                            // We've reached the end of the file and didn't hit the closing token. Bail without
                            // consuming the newlines so that the missing char diagnostic will be in the correct place.
                            break;
                        }
                        else if (!Check(this.reader.PeekAhead(peekPosition), closingTokenType))
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

        protected IdentifierSyntax Identifier(DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
        {
            var identifier = Expect(TokenType.Identifier, errorFunc);

            return new IdentifierSyntax(identifier);
        }

        protected IdentifierSyntax IdentifierOrSkip(DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
        {
            if (this.Check(TokenType.Identifier))
            {
                var identifier = Expect(TokenType.Identifier, errorFunc);
                return new IdentifierSyntax(identifier);
            }

            var skipped = SkipEmpty(errorFunc);
            return new IdentifierSyntax(skipped);
        }

        protected IdentifierSyntax IdentifierWithRecovery(DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc, RecoveryFlags flags, params TokenType[] terminatingTypes)
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
                insideForExpression ? [TokenType.RightSquare, TokenType.LeftBrace, TokenType.NewLine] : [TokenType.LeftBrace, TokenType.NewLine]);
            var body = this.WithRecovery(
                () => this.Object(expressionFlags),
                GetSuppressionFlag(conditionExpression, conditionExpression is ParenthesizedExpressionSyntax { CloseParen: not SkippedTriviaSyntax }),
                insideForExpression ? [TokenType.RightSquare, TokenType.NewLine] : [TokenType.NewLine]);
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
                        interpExpression = new SkippedTriviaSyntax(TextSpan.Between(combined.First(), combined.Last()), combined);
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
                    return new SkippedTriviaSyntax(span, tokensOrSyntax);
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
            return current.Type switch
            {
                TokenType.TrueKeyword => new BooleanLiteralSyntax(reader.Read(), true),
                TokenType.FalseKeyword => new BooleanLiteralSyntax(reader.Read(), false),
                TokenType.Integer when NumericLiteral() is { literal: Token literal, value: ulong value }
                    => new IntegerLiteralSyntax(literal, value),
                TokenType.NullKeyword => new NullLiteralSyntax(reader.Read()),
                _ => throw new ExpectedTokenException(current, b => b.InvalidType()),
            };
        }

        private TypeSyntax LiteralType()
        {
            var current = reader.Peek();
            return current.Type switch
            {
                TokenType.TrueKeyword => new BooleanTypeLiteralSyntax(reader.Read(), true),
                TokenType.FalseKeyword => new BooleanTypeLiteralSyntax(reader.Read(), false),
                TokenType.Integer when NumericLiteral() is { literal: Token literal, value: ulong value }
                    => new IntegerTypeLiteralSyntax(literal, value),
                TokenType.NullKeyword => new NullTypeLiteralSyntax(reader.Read()),
                _ => throw new ExpectedTokenException(current, b => b.InvalidType()),
            };
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

                    Token? safeAccessMarker = null;
                    if (this.Check(TokenType.Question))
                    {
                        safeAccessMarker = this.reader.Read();
                    }

                    Token? fromEndMarker = null;
                    if (this.Check(TokenType.Hat))
                    {
                        fromEndMarker = this.reader.Read();
                    }

                    if (this.Check(TokenType.RightSquare))
                    {
                        // empty indexer - we are allowing this special case in the parser to help with completions
                        SyntaxBase skipped = SkipEmpty(b => b.EmptyIndexerNotAllowed());
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                        current = new ArrayAccessSyntax(current, openSquare, safeAccessMarker, fromEndMarker, skipped, closeSquare);
                    }
                    else
                    {
                        SyntaxBase indexExpression = this.Expression(expressionFlags);
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                        current = new ArrayAccessSyntax(current, openSquare, safeAccessMarker, fromEndMarker, indexExpression, closeSquare);
                    }

                    continue;
                }

                if (this.Check(TokenType.Dot))
                {
                    // dot operator
                    Token dot = this.reader.Read();
                    Token? safeAccessMarker = null;
                    if (this.Check(TokenType.Question))
                    {
                        safeAccessMarker = this.reader.Read();
                    }

                    IdentifierSyntax identifier = this.IdentifierOrSkip(b => b.ExpectedFunctionOrPropertyName());

                    if (Check(TokenType.LeftParen))
                    {
                        var functionCall = FunctionCallAccess(identifier, expressionFlags);
                        if (safeAccessMarker is not null)
                        {
                            functionCall = (
                                new IdentifierSyntax(new SkippedTriviaSyntax(TextSpan.Between(safeAccessMarker.Span, identifier.Span),
                                    new SyntaxBase[] { safeAccessMarker, identifier },
                                    DiagnosticBuilder.ForPosition(safeAccessMarker).SafeDereferenceNotPermittedOnInstanceFunctions().AsEnumerable())),
                                functionCall.OpenParen,
                                functionCall.ArgumentNodes,
                                functionCall.CloseParen
                            );
                        }

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
                        current = new PropertyAccessSyntax(current, dot, safeAccessMarker, identifier);
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

                if (this.Check(TokenType.Exclamation))
                {
                    current = new NonNullAssertionSyntax(current, this.reader.Read());
                    continue;
                }

                break;
            }

            return current;
        }

        private SyntaxBase MemberTypeExpression()
        {
            var current = this.PrimaryTypeExpression();

            while (true)
            {
                if (this.Check(TokenType.LeftSquare))
                {
                    // array indexer
                    Token openSquare = this.reader.Read();

                    if (this.Check(TokenType.RightSquare))
                    {
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));
                        current = new ArrayTypeSyntax(new ArrayTypeMemberSyntax(current), openSquare, closeSquare);
                    }
                    else if (this.Check(TokenType.Asterisk))
                    {
                        Token asterisk = this.Expect(TokenType.Asterisk, b => b.ExpectedCharacter("*"));
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));
                        current = new TypeItemsAccessSyntax(current, openSquare, asterisk, closeSquare);
                    }
                    else
                    {
                        SyntaxBase indexExpression = this.Expression(ExpressionFlags.None);
                        Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                        current = new TypeArrayAccessSyntax(current, openSquare, indexExpression, closeSquare);
                    }

                    continue;
                }

                if (this.Check(TokenType.Dot))
                {
                    // dot operator
                    Token dot = this.reader.Read();

                    if (this.Check(TokenType.Asterisk))
                    {
                        Token asterisk = this.Expect(TokenType.Asterisk, b => b.ExpectedCharacter("*"));
                        current = new TypeAdditionalPropertiesAccessSyntax(current, dot, asterisk);

                        continue;
                    }

                    IdentifierSyntax identifier = this.IdentifierOrSkip(b => b.ExpectedFunctionOrPropertyName());

                    if (this.Check(TokenType.LeftChevron))
                    {
                        var parameterizedType = this.ParameterizedTypeInstantiation(identifier);

                        current = new InstanceParameterizedTypeInstantiationSyntax(
                            current,
                            dot,
                            parameterizedType.Identifier,
                            parameterizedType.OpenChevron,
                            parameterizedType.ParameterNodes,
                            parameterizedType.CloseChevron);
                    }
                    else
                    {
                        current = new TypePropertyAccessSyntax(current, dot, identifier);
                    }

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
                return new SkippedTriviaSyntax(token.Span, token.AsEnumerable());
            }

            return new StringSyntax(token.AsEnumerable(), [], stringValue.AsEnumerable());
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

        protected IEnumerable<Token> NewLines()
        {
            while (Check(TokenType.NewLine))
            {
                yield return this.NewLine();
            }
        }

        private (Token literal, ulong value) NumericLiteral()
        {
            var literal = Expect(TokenType.Integer, b => b.ExpectedNumericLiteral());

            if (ulong.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out ulong value))
            {
                return (literal, value);
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
                parseChildElement: () => ObjectElement(expressionFlags));

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new ObjectSyntax(openBrace, itemsOrTokens, closeBrace);
        }

        private SpreadExpressionSyntax SpreadExpression(TokenType[] terminatingTypes)
        {
            var ellipsis = this.Expect(TokenType.Ellipsis, b => b.ExpectedCharacter("..."));
            var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(ellipsis), terminatingTypes);

            return new SpreadExpressionSyntax(ellipsis, expression);
        }

        private SyntaxBase ObjectElement(ExpressionFlags expressionFlags)
        {
            return this.WithRecovery<SyntaxBase>(() =>
            {
                var current = this.reader.Peek();

                // Nested resource declarations may be allowed - but we need lookahead to avoid
                // treating 'resource' as a reserved property name.
                if (HasExpressionFlag(expressionFlags, ExpressionFlags.AllowResourceDeclarations) &&
                    (Check(TokenType.At)
                    // You are here: |@batchSize(1)
                    //                resource <name> ...
                    //
                    // If we see a decorator declaration then we need to expect a declaration that follows it
                    || (CheckKeyword(LanguageConstants.ResourceKeyword) &&
                     // You are here: |resource <name> ...
                     //
                     // If we see an identifier then it's a resource declaration. Otherwise, fall back to the property parser.
                     Check(this.reader.PeekAhead(), TokenType.Identifier))))
                {
                    return this.Declaration(LanguageConstants.ResourceKeyword);
                }

                if (current.Type == TokenType.Ellipsis)
                {
                    return SpreadExpression([TokenType.Comma, TokenType.NewLine, TokenType.RightBrace]);
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
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(() => Expression(expressionFlags), permitNewLines: true);
            var innerSyntax = GetParenthesizedExpressionInnerContent(openParen, expressionsOrCommas, closeParen);
            return new ParenthesizedExpressionSyntax(openParen, innerSyntax, closeParen);
        }

        private TypeSyntax ParenthesizedTypeExpression()
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(TypeExpression, permitNewLines: false);
            var innerSyntax = GetParenthesizedExpressionInnerContent(openParen, expressionsOrCommas, closeParen);
            return new ParenthesizedTypeSyntax(openParen, innerSyntax, closeParen);
        }

        private (Token openParen, ImmutableArray<SyntaxBase> expressionsOrCommas, SyntaxBase closeParen) ParenthesizedExpressionList(Func<SyntaxBase> expressionParser, bool permitNewLines)
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));
            var itemsOrTokens = new List<SyntaxBase>();

            void parseNewLines()
            {
                if (permitNewLines)
                {
                    itemsOrTokens.AddRange(NewLines());
                }
            }

            parseNewLines();
            while (!this.Check(TokenType.RightParen))
            {
                var expression = this.WithRecovery(
                    expressionParser,
                    RecoveryFlags.None,
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine,
                    TokenType.Comma);
                itemsOrTokens.Add(expression);
                parseNewLines();

                if (this.Check(TokenType.Comma))
                {
                    var comma = this.Expect(TokenType.Comma, b => b.ExpectedCharacter(","));
                    itemsOrTokens.Add(comma);
                    parseNewLines();
                }
                else
                {
                    parseNewLines();
                    break;
                }
            }

            var closeParen = this.WithRecovery(
                () => this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")")),
                itemsOrTokens.Any() ? GetSuppressionFlag(itemsOrTokens.Last()) : RecoveryFlags.None,
                TokenType.StringRightPiece,
                TokenType.RightBrace,
                TokenType.RightSquare,
                TokenType.NewLine);

            return (openParen, itemsOrTokens.ToImmutableArray(), closeParen);
        }

        private SyntaxBase ParenthesizedExpressionOrLambda(ExpressionFlags expressionFlags)
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(() => Expression(expressionFlags), permitNewLines: true);

            if (Check(TokenType.Arrow))
            {
                var arrow = this.Expect(TokenType.Arrow, b => b.ExpectedCharacter("=>"));
                var next = this.reader.Peek(skipNewlines: true);
                var newlinesBeforeBody = !LanguageConstants.DeclarationKeywords.Contains(next.Text)
                    ? this.NewLines().ToImmutableArray()
                    : [];
                var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);
                var variableBlock = GetVariableBlock(openParen, expressionsOrCommas, closeParen);

                return new LambdaSyntax(variableBlock, arrow, [.. newlinesBeforeBody], expression);
            }

            var innerSyntax = GetParenthesizedExpressionInnerContent(openParen, expressionsOrCommas, closeParen);
            return new ParenthesizedExpressionSyntax(openParen, innerSyntax, closeParen);
        }

        private SyntaxBase TypedLocalVariable(params TokenType[] terminatingTypes)
        {
            var name = IdentifierOrSkip(b => b.ExpectedVariableIdentifier());
            var type = this.WithRecovery(() => Type(allowOptionalResourceType: false), RecoveryFlags.None, terminatingTypes);

            return new TypedLocalVariableSyntax(name, type);
        }

        protected SyntaxBase TypedLambda()
        {
            var (openParen, expressionsOrCommas, closeParen) = ParenthesizedExpressionList(() => TypedLocalVariable(TokenType.NewLine, TokenType.Comma, TokenType.RightParen), permitNewLines: true);

            var returnType = this.WithRecovery(() => Type(allowOptionalResourceType: false), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);
            var arrow = this.WithRecovery(() => Expect(TokenType.Arrow, b => b.ExpectedCharacter("=>")), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);
            var next = this.reader.Peek(skipNewlines: true);
            var newlinesBeforeBody = !arrow.IsSkipped && !LanguageConstants.DeclarationKeywords.Contains(next.Text)
                ? this.NewLines().ToImmutableArray()
                : [];
            var expression = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine, TokenType.RightParen);
            var variableBlock = new TypedVariableBlockSyntax(openParen, expressionsOrCommas, closeParen);

            return new TypedLambdaSyntax(variableBlock, returnType, arrow, newlinesBeforeBody, expression);
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
                    return CheckKeyword(this.reader.PeekAhead(skipNewlines: true), LanguageConstants.ForKeyword)
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

        private SyntaxBase PrimaryTypeExpression()
        {
            Token nextToken = this.reader.Peek();

            switch (nextToken.Type)
            {
                case TokenType.Integer:
                case TokenType.NullKeyword:
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    return this.LiteralType();

                case TokenType.StringComplete:
                case TokenType.StringLeftPiece:
                    return AsStringTypeLiteral(this.InterpolableString());

                case TokenType.MultilineString:
                    return AsStringTypeLiteral(this.MultilineString());

                case TokenType.LeftBrace:
                    return this.ObjectType();

                case TokenType.LeftSquare:
                    return this.TupleType();

                case TokenType.LeftParen:
                    return this.ParenthesizedTypeExpression();

                case TokenType.Identifier:
                    return this.ParameterizedTypeOrTypeVariableAccess();

                default:
                    throw new ExpectedTokenException(nextToken, b => b.UnrecognizedTypeExpression());
            }
        }

        private static SyntaxBase AsStringTypeLiteral(SyntaxBase syntax) => syntax switch
        {
            StringSyntax @string => new StringTypeLiteralSyntax(@string.StringTokens, @string.Expressions, @string.SegmentValues),
            _ => syntax,
        };

        protected SkippedTriviaSyntax Skip(SyntaxBase syntax, DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
            => Skip(syntax.AsEnumerable(), errorFunc);

        private SkippedTriviaSyntax Skip(IEnumerable<SyntaxBase> syntax, DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
        {
            var syntaxArray = syntax.ToImmutableArray();
            var span = TextSpan.Between(syntaxArray.First(), syntaxArray.Last());
            return new SkippedTriviaSyntax(span, syntaxArray, errorFunc(DiagnosticBuilder.ForPosition(span)).AsEnumerable());
        }

        private static SkippedTriviaSyntax Skip(IEnumerable<SyntaxBase> syntax)
        {
            var syntaxArray = syntax.ToImmutableArray();
            var span = TextSpan.Between(syntaxArray.First(), syntaxArray.Last());

            return new SkippedTriviaSyntax(span, syntaxArray);
        }

        protected SkippedTriviaSyntax SkipEmpty()
            => SkipEmpty(this.reader.Peek().Span.Position, null);

        protected SkippedTriviaSyntax SkipEmpty(DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
            => SkipEmpty(this.reader.Peek().Span.Position, errorFunc);

        private SkippedTriviaSyntax SkipEmpty(int position, DiagnosticBuilder.DiagnosticBuilderDelegate? errorFunc)
        {
            var span = new TextSpan(position, 0);
            var errors = errorFunc is null ? [] : errorFunc(DiagnosticBuilder.ForPosition(span)).AsEnumerable();
            return new SkippedTriviaSyntax(span, ImmutableArray<SyntaxBase>.Empty, errors);
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

        private SkippedTriviaSyntax SynchronizeAndReturnTrivia(int startReaderPosition, RecoveryFlags flags, DiagnosticBuilder.DiagnosticBuilderDelegate diagnosticBuilder, params TokenType[] expectedTypes)
        {
            var startToken = reader.AtPosition(startReaderPosition);

            // Generally we don't want the error span to include the terminating token, so synchronize with and without if required.
            // The skipped trivia returned should always include the full span
            Synchronize(false, expectedTypes);
            var skippedTokens = reader.Slice(startReaderPosition, reader.Position - startReaderPosition).ToArray();
            var skippedSpan = TextSpan.SafeBetween(skippedTokens, startToken.Span.Position);
            var errorSpan = skippedSpan;

            if (flags.HasFlag(RecoveryFlags.ConsumeTerminator))
            {
                Synchronize(true, expectedTypes);

                skippedTokens = [.. reader.Slice(startReaderPosition, reader.Position - startReaderPosition)];
                skippedSpan = TextSpan.SafeBetween(skippedTokens, startToken.Span.Position);
            }

            var errors = flags.HasFlag(RecoveryFlags.SuppressDiagnostics)
                ? ImmutableArray<Diagnostic>.Empty
                : [diagnosticBuilder(DiagnosticBuilder.ForPosition(errorSpan))];

            return new SkippedTriviaSyntax(skippedSpan, skippedTokens, errors);
        }

        protected SyntaxBase ThrowIfSkipped(Func<SyntaxBase> syntaxFunc, DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
        {
            var startToken = reader.Peek();
            var syntax = syntaxFunc();

            if (syntax.IsSkipped)
            {
                throw new ExpectedTokenException(startToken, errorFunc);
            }

            return syntax;
        }

        protected SyntaxBase Type(bool allowOptionalResourceType)
        {
            if (CheckKeyword(LanguageConstants.ResourceKeyword) && this.reader.PeekAhead(1)?.Type != TokenType.LeftChevron)
            {
                var resourceKeyword = reader.Read();
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

            return TypeExpression();
        }

        protected SyntaxBase TypeExpression()
        {
            // Parse optional leading '|' for union types.
            List<SyntaxBase>? unionTypeNodes = HasUnionMemberSeparator()
                ? new(NewLines()) { reader.Read() }
                : null;

            var candidate = this.UnaryTypeExpression();

            if (unionTypeNodes is not null || HasUnionMemberSeparator())
            {
                unionTypeNodes ??= new();
                unionTypeNodes.Add(new UnionTypeMemberSyntax(candidate));

                while (HasUnionMemberSeparator())
                {
                    // consume the pipe and newline
                    unionTypeNodes.AddRange(NewLines());
                    unionTypeNodes.Add(reader.Read());

                    // error reporting gets really wonky if users can have newlines after the pipe. `type foo = 'foo'|` causes the start of the next declaration (i.e., a language keyword) to be reported as a non-existent symbol
                    if (Check(TokenType.NewLine))
                    {
                        unionTypeNodes.Add(SkipEmpty(b => b.ExpectedTypeLiteral()));
                    }
                    else
                    {
                        unionTypeNodes.Add(WithRecovery(() => new UnionTypeMemberSyntax(UnaryTypeExpression()), RecoveryFlags.None));
                    }
                }

                return new UnionTypeSyntax(unionTypeNodes);
            }

            return candidate;
        }

        private bool HasUnionMemberSeparator() => Check(TokenType.Pipe) || (Check(TokenType.NewLine) && Check(reader.PeekAhead(), TokenType.Pipe));

        private ObjectTypeSyntax ObjectType()
        {
            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightBrace,
                parseChildElement: ObjectPropertyType);

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new ObjectTypeSyntax(openBrace, itemsOrTokens, closeBrace);
        }

        private SyntaxBase ObjectPropertyType()
        {
            var leadingNodes = DecorableSyntaxLeadingNodes().ToImmutableArray();

            var current = this.reader.Peek();

            var key = this.WithRecovery(
                () => ThrowIfSkipped(
                    () =>
                        current.Type switch
                        {
                            TokenType.Identifier => this.Identifier(b => b.ExpectedPropertyName()),
                            TokenType.StringComplete or TokenType.StringLeftPiece => this.InterpolableString(),
                            TokenType.Asterisk => this.Expect(TokenType.Asterisk, b => b.ExpectedCharacter("*")),
                            _ => throw new ExpectedTokenException(current, b => b.ExpectedPropertyNameOrMatcher()),
                        }, b => b.ExpectedPropertyName()),
                RecoveryFlags.None,
                TokenType.Colon, TokenType.NewLine, TokenType.RightBrace);

            var colon = this.WithRecovery(() => Expect(TokenType.Colon, b => b.ExpectedCharacter(":")), GetSuppressionFlag(key), TokenType.NewLine, TokenType.RightBrace);
            var value = this.WithRecovery(TypeExpression, GetSuppressionFlag(colon), TokenType.NewLine, TokenType.RightBrace);

            if (key is Token { Type: TokenType.Asterisk })
            {
                return new ObjectTypeAdditionalPropertiesSyntax(leadingNodes, key, colon, value);
            }

            return new ObjectTypePropertySyntax(leadingNodes, key, colon, value);
        }

        private TupleTypeSyntax TupleType()
        {
            var openBracket = Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightSquare,
                parseChildElement: TupleMemberType);

            var closeBracket = Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

            return new TupleTypeSyntax(openBracket, itemsOrTokens, closeBracket);
        }

        private SyntaxBase TupleMemberType() => WithRecovery(
            () => new TupleTypeItemSyntax(DecorableSyntaxLeadingNodes().ToImmutableArray(), TypeExpression()),
            RecoveryFlags.None,
            TokenType.NewLine,
            TokenType.RightSquare);

        protected IEnumerable<SyntaxBase> DecorableSyntaxLeadingNodes()
        {
            while (this.Check(TokenType.At))
            {
                yield return this.Decorator();

                // All decorators must be followed by a newline.
                yield return this.WithRecovery(this.NewLine, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);


                while (this.Check(TokenType.NewLine))
                {
                    // In case there are skipped trivial syntaxes after a decorator, we need to consume
                    // all the newlines after them.
                    yield return this.NewLine();
                }
            }
        }

        protected SyntaxBase Decorator()
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
                        current = new PropertyAccessSyntax(current, dot, null, identifier);
                    }
                }

                return current;
            },
            RecoveryFlags.None,
            TokenType.NewLine);

            return new DecoratorSyntax(at, expression);
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

        private SyntaxBase UnaryTypeExpression()
        {
            var candidate = UnaryTypeBaseExpression();

            while (true)
            {
                if (Check(TokenType.Question))
                {
                    candidate = new NullableTypeSyntax(candidate, Expect(TokenType.Question, b => b.ExpectedCharacter("?")));
                    continue;
                }

                if (Check(TokenType.Exclamation))
                {
                    candidate = new NonNullableTypeSyntax(candidate, Expect(TokenType.Exclamation, b => b.ExpectedCharacter("!")));
                    continue;
                }

                break;
            }

            return candidate;
        }

        private SyntaxBase UnaryTypeBaseExpression()
        {
            Token operatorToken = this.reader.Peek();

            if (Operators.TokenTypeToUnaryOperator.TryGetValue(operatorToken.Type, out var @operator))
            {
                this.reader.Read();

                var expression = this.WithRecovery(
                    MemberTypeExpression,
                    RecoveryFlags.None,
                    TokenType.StringRightPiece,
                    TokenType.RightBrace,
                    TokenType.RightParen,
                    TokenType.RightSquare,
                    TokenType.NewLine);

                return new UnaryTypeOperationSyntax(operatorToken, expression);
            }

            return this.MemberTypeExpression();
        }

        private ImportedSymbolsListSyntax ImportedSymbolsList()
        {
            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightBrace,
                parseChildElement: ImportedSymbolsListItem);

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new(openBrace, itemsOrTokens, closeBrace);
        }

        private SyntaxBase ImportedSymbolsListItem()
        {
            SyntaxBase originalSymbolName = reader.Peek().Type switch
            {
                TokenType.Identifier => Identifier(b => b.ExpectedExportedSymbolName()),
                TokenType.StringComplete => InterpolableString(),
                TokenType.StringLeftPiece => Skip(InterpolableString(), b => b.CompileTimeConstantRequired()),
                _ => Skip(reader.Read(), b => b.ExpectedExportedSymbolName()),
            };

            var aliasAsClause = ImportedSymbolsListItemAsClause();

            if (originalSymbolName is StringSyntax && aliasAsClause is null)
            {
                return new SkippedTriviaSyntax(originalSymbolName.Span,
                    originalSymbolName.AsEnumerable(),
                    DiagnosticBuilder.ForPosition(originalSymbolName).ImportListItemDoesNotIncludeDeclaredSymbolName().AsEnumerable());
            }

            return new ImportedSymbolsListItemSyntax(originalSymbolName, aliasAsClause);
        }

        private AliasAsClauseSyntax? ImportedSymbolsListItemAsClause() => CheckKeyword(reader.Peek(), LanguageConstants.AsKeyword)
            ? new(ExpectKeyword(LanguageConstants.AsKeyword),
                IdentifierWithRecovery(b => b.ExpectedTypeIdentifier(), RecoveryFlags.None, TokenType.Comma, TokenType.NewLine))
            : null;

        private WildcardImportSyntax WildcardImport() => new(Expect(TokenType.Asterisk, b => b.ExpectedCharacter("*")),
            new AliasAsClauseSyntax(ExpectKeyword(LanguageConstants.AsKeyword),
                Identifier(b => b.ExpectedNamespaceIdentifier())));

        protected SyntaxBase FunctionDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.FunctionKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedVariableIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var lambda = this.WithRecovery(() => this.TypedLambda(), GetSuppressionFlag(name), TokenType.NewLine);

            return new FunctionDeclarationSyntax(leadingNodes, keyword, name, lambda);
        }

        private CompileTimeImportFromClauseSyntax CompileTimeImportFromClause()
        {
            var keyword = ExpectKeyword(LanguageConstants.FromKeyword);
            var path = WithRecovery(
                () => ThrowIfSkipped(InterpolableString, b => b.ExpectedModulePathString()),
                GetSuppressionFlag(keyword),
                TokenType.NewLine);

            return new(keyword, path);
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

        private bool HasSyntaxError(SyntaxBase syntax)
        {
            if (this.LexingErrorLookup.Contains(syntax))
            {
                return true;
            }

            var diagnosticWriter = new SimpleDiagnosticWriter();
            var parsingErrorVisitor = new ParseDiagnosticsVisitor(diagnosticWriter);

            parsingErrorVisitor.Visit(syntax);

            return diagnosticWriter.HasDiagnostics();
        }
    }
}
