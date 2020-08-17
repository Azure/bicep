using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Bicep.Core.Diagnostics;
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
            var statements = new List<SyntaxBase>();
            while (!this.IsAtEnd())
            {
                var statement = Declaration();
                statements.Add(statement);
            }

            var endOfFile = reader.Read();

            return new ProgramSyntax(statements, endOfFile, this.lexerDiagnostics);
        }

        private SyntaxBase Declaration()
        {
            return this.WithRecovery(() =>
            {
                var current = reader.Peek();

                return current.Type switch {
                    TokenType.Identifier => current.Text switch {
                        LanguageConstants.ParameterKeyword => this.ParameterDeclaration(),
                        LanguageConstants.VariableKeyword => this.VariableDeclaration(),
                        LanguageConstants.ResourceKeyword => this.ResourceDeclaration(),
                        LanguageConstants.OutputKeyword => this.OutputDeclaration(),
                        _ => throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                    },
                    TokenType.NewLine => this.NoOpStatement(),
                    // TODO: Update when adding other statement types
                    _ => throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                };
            }, TokenType.NewLine);
        }

        private SyntaxBase ParameterDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.ParameterKeyword);
            var name = Identifier(b => b.ExpectedParameterIdentifier());
            var type = Type(b => b.ExpectedParameterType());

            var current = reader.Peek();
            SyntaxBase? modifier = current.Type switch
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

            var newLine = this.NewLineOrEof();

            return new ParameterDeclarationSyntax(keyword, name, type, modifier, newLine);
        }

        private SyntaxBase ParameterDefaultValue()
        {
            var assignmentToken = this.Expect(TokenType.Assignment, b => b.ExpectedCharacter("="));
            SyntaxBase defaultValue = this.Expression();

            return new ParameterDefaultValueSyntax(assignmentToken, defaultValue);
        }

        private SyntaxBase VariableDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.VariableKeyword);
            var name = this.Identifier(b => b.ExpectedVariableIdentifier());
            var assignment = this.Assignment();
            var value = this.Expression();

            var newLine = this.NewLineOrEof();

            return new VariableDeclarationSyntax(keyword, name, assignment, value, newLine);
        }

        private SyntaxBase OutputDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.OutputKeyword);
            var name = this.Identifier(b => b.ExpectedOutputIdentifier());
            var type = Type(b => b.ExpectedParameterType());
            var assignment = this.Assignment();
            var value = this.Expression();

            var newLine = this.NewLineOrEof();

            return new OutputDeclarationSyntax(keyword, name, type, assignment, value, newLine);
        }

        private SyntaxBase ResourceDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.ResourceKeyword);
            var name = this.Identifier(b => b.ExpectedResourceIdentifier());

            // TODO: Unify StringSyntax with TypeSyntax
            var type = this.InterpolableString();

            var assignment = this.Assignment();
            var body = this.Object();

            var newLine = this.NewLineOrEof();

            return new ResourceDeclarationSyntax(keyword, name, type, assignment, body, newLine);
        }

        private SyntaxBase NoOpStatement()
        {
            var newLine = this.NewLine();
            return new NoOpDeclarationSyntax(newLine);
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

        public SyntaxBase Expression()
        {
            var candidate = this.BinaryExpression();

            if (this.Check(TokenType.Question))
            {
                var question = this.reader.Read();
                var trueExpression = this.Expression();
                var colon = this.Expect(TokenType.Colon, b => b.ExpectedCharacter(":"));
                var falseExpression = this.Expression();

                return new TernaryOperationSyntax(candidate, question, trueExpression, colon, falseExpression);
            }

            return candidate;
        }

        private SyntaxBase BinaryExpression(int precedence = 0)
        {
            var current = this.UnaryExpression();

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

                SyntaxBase rightExpression = this.BinaryExpression(operatorPrecedence);
                current = new BinaryOperationSyntax(current, candidateOperatorToken, rightExpression);
            }

            return current;
        }

        private SyntaxBase UnaryExpression()
        {
            Token operatorToken = this.reader.Peek();

            if (Operators.TokenTypeToUnaryOperator.TryGetValue(operatorToken.Type, out var @operator))
            {
                this.reader.Read();

                var expression = this.MemberExpression();
                return new UnaryOperationSyntax(operatorToken, expression);
            }

            return this.MemberExpression();
        }

        private SyntaxBase MemberExpression()
        {
            var current = this.PrimaryExpression();

            while (true)
            {
                if (this.Check(TokenType.LeftSquare))
                {
                    // array indexer
                    Token openSquare = this.reader.Read();
                    SyntaxBase indexExpression = this.Expression();
                    Token closeSquare = this.Expect(TokenType.RightSquare, b => b.ExpectedCharacter("]"));

                    current = new ArrayAccessSyntax(current, openSquare, indexExpression, closeSquare);

                    continue;
                }

                if (this.Check(TokenType.Dot))
                {
                    // dot operator
                    Token dot = this.reader.Read();
                    IdentifierSyntax propertyName = this.Identifier(b => b.ExpectedPropertyIdentifier());

                    current = new PropertyAccessSyntax(current, dot, propertyName);

                    continue;
                }

                break;
            }

            return current;
        }

        private SyntaxBase PrimaryExpression()
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

                case TokenType.LeftBrace:
                    return this.Object();

                case TokenType.LeftSquare:
                    return this.Array();

                case TokenType.LeftParen:
                    return this.ParenthesizedExpression();

                case TokenType.Identifier:
                    return this.FunctionCallOrVariableAccess();

                default:
                    throw new ExpectedTokenException(nextToken, b => b.UnrecognizedExpression());
            }
        }

        private SyntaxBase ParenthesizedExpression()
        {
            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));
            var expression = this.Expression();
            var closeParen = this.Expect(TokenType.RightParen, b => b.ExpectedCharacter(")"));

            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }

        private SyntaxBase FunctionCallOrVariableAccess()
        {
            var identifier = this.Identifier(b => b.ExpectedFunctionName());

            if (this.Check(TokenType.LeftParen) == false)
            {
                // just a reference to a variable, parameter, or output
                return new VariableAccessSyntax(identifier);
            }

            var openParen = this.Expect(TokenType.LeftParen, b => b.ExpectedCharacter("("));

            var arguments = new List<(SyntaxBase expression, Token? comma)>();

            if (this.Check(TokenType.RightParen))
            {
                // end of a parameter-less function call
                var closeParen = this.reader.Read();
                return new FunctionCallSyntax(identifier, openParen, ImmutableArray<FunctionArgumentSyntax>.Empty, closeParen);
            }

            while (true)
            {
                var expression = this.Expression();
                arguments.Add((expression, null));

                if (this.Check(TokenType.RightParen))
                {
                    // end of function call
                    // return the accumulated arguments
                    var closeParen = this.reader.Read();
                    var argumentNodes = arguments.Select(a => new FunctionArgumentSyntax(a.expression, a.comma));
                    return new FunctionCallSyntax(identifier, openParen, argumentNodes, closeParen);
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
            var startPosition = reader.Position;
            SyntaxBase TerminateString(IReadOnlyList<Token> stringTokens, IEnumerable<SyntaxBase> syntaxExpressions)
            {
                // the lexer may return unterminated string tokens to allow lexing to continue over an interpolated string.
                // we should catch that here and prevent parsing from succeeding.
                var segments = Lexer.TryGetRawStringSegments(stringTokens);
                if (segments == null)
                {
                    // We've got a string terminator, so we can do better than throwing an ExpectedTokenException,
                    // which would force the Parser to skip to the end of the line.
                    var skippedTokens = reader.Slice(startPosition, reader.Position - startPosition).ToArray();
                    var tokensSpan = TextSpan.Between(skippedTokens.First(), skippedTokens.Last());
                    return new SkippedTokensTriviaSyntax(skippedTokens, null, null);
                }
                return new StringSyntax(stringTokens, syntaxExpressions, segments);
            }

            return this.WithRecovery(() => {
                var stringTokens = new List<Token>();
                var syntaxExpressions = new List<SyntaxBase>();

                var nextToken = reader.Read();
                switch (nextToken.Type)
                {
                    case TokenType.StringComplete:
                        stringTokens.Add(nextToken);
                        return TerminateString(stringTokens, syntaxExpressions);
                    case TokenType.StringLeftPiece:
                        stringTokens.Add(nextToken);
                        syntaxExpressions.Add(Expression());
                        break;
                    default:
                        // don't actually consume the next token - leave this up to recovery
                        reader.StepBack();
                        throw new ExpectedTokenException(nextToken, b => b.MalformedString());
                }
                
                // we're handling an interpolated string
                while (true)
                {
                    nextToken = reader.Read();
                    switch (nextToken.Type)
                    {
                        case TokenType.StringRightPiece:
                            stringTokens.Add(nextToken);
                            return TerminateString(stringTokens, syntaxExpressions);
                        case TokenType.StringMiddlePiece:
                            stringTokens.Add(nextToken);
                            syntaxExpressions.Add(Expression());
                            break;
                        default:
                            // don't actually consume the next token - leave this up to recovery
                            reader.StepBack();
                            throw new ExpectedTokenException(nextToken, b => b.MalformedString());
                    }
                }
            }, TokenType.NewLine);
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
            var items = new List<SyntaxBase>();
            
            var openBracket = Expect(TokenType.LeftSquare, b => b.ExpectedCharacter("["));
            var newLines = this.NewLines();

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
                var value = this.Expression();
                var newLines = this.NewLines();

                return new ArrayItemSyntax(value, newLines);
            }, TokenType.NewLine);
        }

        private SyntaxBase Object()
        {
            var properties = new List<SyntaxBase>();

            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));
            var newLines = this.NewLines();

            while (this.IsAtEnd() == false && this.reader.Peek().Type != TokenType.RightBrace)
            {
                var property = this.ObjectProperty();
                properties.Add(property);
            }

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new ObjectSyntax(openBrace, newLines, properties, closeBrace);
        }

        private SyntaxBase ObjectProperty()
        {
            return this.WithRecovery(() =>
            {
                var identifier = this.Identifier(b => b.ExpectedPropertyName());
                var colon = Expect(TokenType.Colon, b => b.ExpectedCharacter(":"));
                var value = Expression();
                var newLines = this.NewLines();

                return new ObjectPropertySyntax(identifier, colon, value, newLines);
            }, TokenType.NewLine);
        }

        private SyntaxBase WithRecovery<TSyntax>(Func<TSyntax> syntaxFunc, params TokenType[] terminatingTypes)
            where TSyntax : SyntaxBase
        {
            var startPosition = reader.Position;
            try
            {
                return syntaxFunc();
            }
            catch (ExpectedTokenException exception)
            {
                this.Synchronize(terminatingTypes);
                
                var skippedTokens = reader.Slice(startPosition, reader.Position - startPosition);
                return new SkippedTokensTriviaSyntax(skippedTokens, exception.Error, exception.UnexpectedToken);
            }
        }

        private void Synchronize(params TokenType[] expectedTypes)
        {
            while (!IsAtEnd())
            {
                if (Match(expectedTypes))
                {
                    return;
                }

                reader.Read();
            }
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