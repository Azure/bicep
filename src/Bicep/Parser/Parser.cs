using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Syntax;

namespace Bicep.Parser
{
    public class Parser
    {
        private readonly TokenReader reader;
        private readonly IList<Error> errors = new List<Error>();

        public Parser(IEnumerable<Token> tokens)
        {
            this.reader = new TokenReader(tokens);
        }

        public SyntaxBase Parse()
            => Program();

        private SyntaxBase Program()
        {
            var statements = new List<SyntaxBase>();
            while (!IsAtEnd())
            {
                var statement = Statement();
                statements.Add(statement);
            }

            var endOfFile = reader.Read();

            return new ProgramSyntax(statements, endOfFile);
        }

        private SyntaxBase Statement()
        {
            var nextType = reader.Read().Type;
            switch (nextType)
            {
                case TokenType.InputKeyword:
                    return InputStatement();
                case TokenType.OutputKeyword:
                    return OutputStatement();
                case TokenType.VariableKeyword:
                    return VariableStatement();
                case TokenType.ResourceKeyword:
                    return ResourceStatement();
            }

            throw new NotImplementedException();
        }

        private SyntaxBase InputStatement()
            => WithRecovery(() => {
                var keyword = reader.Prev();
                var type = Identifier();
                var identifier = Identifier();
                var semicolon = Expect(TokenType.Semicolon, "");

                return new InputDeclSyntax(keyword, type, identifier, semicolon);
            });

        private SyntaxBase OutputStatement()
            => WithRecovery(() => {
                var keyword = reader.Prev();
                var identifier = Identifier();
                var colon = Expect(TokenType.Colon, "");
                var expression = Expression();
                var semicolon = Expect(TokenType.Semicolon, "");

                return new OutputDeclSyntax(keyword, identifier, colon, expression, semicolon);
            });

        private SyntaxBase VariableStatement()
            => WithRecovery(() => {
                var keyword = reader.Prev();
                var identifier = Identifier();
                var colon = Expect(TokenType.Colon, "");
                var expression = Expression();
                var semicolon = Expect(TokenType.Semicolon, "");

                return new VarDeclSyntax(keyword, identifier, colon, expression, semicolon);
            });

        private SyntaxBase ResourceStatement()
            => WithRecovery(() => {
                var keyword = reader.Prev();
                var provider = Identifier();
                var type = String();
                var identifier = Identifier();
                var colon = Expect(TokenType.Colon, "");
                var expression = Expression();
                var semicolon = Expect(TokenType.Semicolon, "");

                return new ResourceDeclSyntax(keyword, provider, type, identifier, colon, expression, semicolon);
            });

        private StringSyntax String()
        {
            var stringToken = Expect(TokenType.String, "...");

            return new StringSyntax(stringToken);
        }

        private IdentifierSyntax Identifier()
        {
            var identifier = Expect(TokenType.Identifier, "...");

            return new IdentifierSyntax(identifier);
        }

        private SyntaxBase Expression()
            => Or();

        private SyntaxBase Or()
        {
            var expression = And();

            if (Match(TokenType.BinaryOr))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Or(), BinaryOperation.Or);
            }

            return expression;
        }

        private SyntaxBase And()
        {
            var expression = Equality();

            if (Match(TokenType.BinaryAnd))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, And(), BinaryOperation.And);
            }

            return expression;
        }

        private SyntaxBase Equality()
        {
            var expression = Comparison();

            if (Match(TokenType.Equals))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Equality(), BinaryOperation.Equals);
            }

            if (Match(TokenType.NotEquals))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Equality(), BinaryOperation.NotEquals);
            }

            if (Match(TokenType.EqualsInsensitive))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Equality(), BinaryOperation.EqualsInsensitive);
            }

            if (Match(TokenType.NotEqualsInsensitive))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Equality(), BinaryOperation.NotEqualsInsensitive);
            }

            return expression;
        }

        private SyntaxBase Comparison()
        {
            var expression = Addition();

            if (Match(TokenType.GreaterThan))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Comparison(), BinaryOperation.GreaterThan);
            }

            if (Match(TokenType.GreaterThanOrEqual))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Comparison(), BinaryOperation.GreaterThanOrEqual);
            }

            if (Match(TokenType.LessThan))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Comparison(), BinaryOperation.LessThan);
            }

            if (Match(TokenType.LessThanOrEqual))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Comparison(), BinaryOperation.LessThanOrEqual);
            }

            return expression;
        }

        private SyntaxBase Addition()
        {
            var expression = Multiplication();

            if (Match(TokenType.Plus))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Addition(), BinaryOperation.Add);
            }

            if (Match(TokenType.Minus))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Addition(), BinaryOperation.Subtract);
            }

            return expression;
        }

        private SyntaxBase Multiplication()
        {
            var expression = Unary();

            if (Match(TokenType.Asterisk))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Multiplication(), BinaryOperation.Multiply);
            }

            if (Match(TokenType.Slash))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Multiplication(), BinaryOperation.Divide);
            }

            if (Match(TokenType.Modulus))
            {
                var operatorToken = reader.Prev();
                return new BinaryOperationSyntax(expression, operatorToken, Multiplication(), BinaryOperation.Modulus);
            }

            return expression;
        }

        private SyntaxBase Unary()
        {
            if (Match(TokenType.Exclamation))
            {
                var operatorToken = reader.Prev();
                return new UnaryOperationSyntax(operatorToken, Unary());
            }

            if (Match(TokenType.Minus))
            {
                var operatorToken = reader.Prev();
                return new UnaryOperationSyntax(operatorToken, Unary());
            }

            return MemberAccess();
        }

        private SyntaxBase MemberAccess()
        {
            var output = FunctionCall();

            while (true)
            {
                if (Match(TokenType.Dot))
                {
                    var dot = reader.Prev();
                    var member = FunctionCall();

                    output = new PropertyAccessSyntax(output, dot, member);
                }
                else if (Match(TokenType.LeftSquare))
                {
                    var leftSquare = reader.Prev();
                    var member = Expression();
                    var rightSquare = Expect(TokenType.RightSquare, "");

                    output = new ArrayAccessSyntax(output, leftSquare, member, rightSquare);
                }
                else if (IsAtEnd())
                {
                    throw new Exception("");
                }
                else
                {
                    break;
                }
            }

            return output;
        }

        private SyntaxBase FunctionCall()
        {
            var primary = Primary();

            if (Match(TokenType.LeftParen))
            {
                var leftParen = reader.Prev();
                var arguments = new List<SyntaxBase>();
                var separators = new List<Token>();
                while (!Match(TokenType.RightParen))
                {
                    if (IsAtEnd())
                    {
                        throw new Exception("");
                    }

                    if (arguments.Any())
                    {
                        var comma = Expect(TokenType.Comma, "");
                        separators.Add(comma);
                    }
    
                    var argument = Expression();
                    arguments.Add(argument);
                }
                var rightParen = reader.Prev();

                var syntaxList = new SeparatedSyntaxList(arguments, separators, TextSpan.BetweenNonInclusive(leftParen, rightParen));
                return new FunctionCallSyntax(primary, leftParen, syntaxList, rightParen);
            }

            return primary;
        }

        private SyntaxBase Primary()
        {
            if (Match(TokenType.Number))
            {
                // todo parse ints safely
                var literal = reader.Prev();
                var value = int.Parse(literal.Text);
                return new NumericLiteralSyntax(literal, value);
            }

            if (Match(TokenType.String))
            {
                // todo string interpolation
                var stringToken = reader.Prev();
                return new StringSyntax(stringToken);
            }

            if (Match(TokenType.Identifier))
            {
                var identifier = reader.Prev();
                return new IdentifierSyntax(identifier);
            }

            if (Match(TokenType.FalseKeyword))
            {
                var literal = reader.Prev();
                return new BooleanLiteralSyntax(literal, false);
            }

            if (Match(TokenType.TrueKeyword))
            {
                var literal = reader.Prev();
                return new BooleanLiteralSyntax(literal, true);
            }

            if (Match(TokenType.NullKeyword))
            {
                var literal = reader.Prev();
                return new NullLiteralSyntax(literal);
            }

            if (Match(TokenType.LeftParen))
            {
                var openParen = reader.Prev();
                var expression = Expression();
                var closeParen = Expect(TokenType.RightParen, "");

                return new GroupingSyntax(openParen, expression, closeParen);
            }

            if (Match(TokenType.LeftBrace))
            {
                return Object();
            }

            if (Match(TokenType.LeftSquare))
            {
                return Array();
            }

            throw new Exception("");
        }

        private ObjectSyntax Object()
        {
            var openBrace = reader.Prev();
            var properties = new List<ObjectPropertySyntax>();
            var separators = new List<Token>();
            while (!Match(TokenType.RightBrace))
            {
                if (IsAtEnd())
                {
                    throw new Exception("");
                }

                var identifier = Identifier();
                var colon = Expect(TokenType.Colon, "");
                var expression = Expression();

                var objectProperty = new ObjectPropertySyntax(identifier, colon, expression);
                properties.Add(objectProperty);

                if (Match(TokenType.Comma))
                {
                    var comma = reader.Prev();
                    separators.Add(comma);
                }
                else
                {
                    Expect(TokenType.RightBrace, "");
                    break;
                }
            }
            var closeBrace = reader.Prev();

            var syntaxList = new SeparatedSyntaxList(properties, separators, TextSpan.BetweenNonInclusive(openBrace, closeBrace));
            return new ObjectSyntax(openBrace, syntaxList, closeBrace);
        }

        private ArraySyntax Array()
        {
            var openSquare = reader.Prev();
            var items = new List<SyntaxBase>();
            var separators = new List<Token>();
            while (!Match(TokenType.RightSquare))
            {
                if (IsAtEnd())
                {
                    throw new Exception("");
                }

                var expression = Expression();
                items.Add(expression);

                if (Match(TokenType.Comma))
                {
                    var comma = reader.Prev();
                    separators.Add(comma);
                }
                else
                {
                    Expect(TokenType.RightSquare, "");
                    break;
                }
            }
            var closeSquare = reader.Prev();

            var syntaxList = new SeparatedSyntaxList(items, separators, TextSpan.BetweenNonInclusive(openSquare, closeSquare));
            return new ArraySyntax(openSquare, syntaxList, closeSquare);
        }

        private SyntaxBase WithRecovery<TSyntax>(Func<TSyntax> syntaxFunc)
            where TSyntax : SyntaxBase
        {
            return syntaxFunc();
            /* TODO recovery
            try
            {
                return syntaxFunc();
            }
            catch (Exception exception)
            {
                
            }
            */
        }

        private bool IsAtEnd()
        {
            return reader.IsAtEnd() || reader.Peek().Type == TokenType.EndOfFile;
        }

        private Token Expect(TokenType type, string message)
        {
            var token = reader.Read();
            if (token.Type == type)
            {
                return token;
            }

            throw new Exception(message);
        }

        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                reader.Read();
                return true;
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return reader.Peek().Type == type;
        }
    }
}