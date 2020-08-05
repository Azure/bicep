﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    public class Parser
    {
        private readonly TokenReader reader;

        private readonly ImmutableArray<Error> lexicalErrors;
        
        public Parser(string text)
        {
            // treating the lexer as an implementation detail of the parser
            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            this.lexicalErrors = lexer.GetErrors();

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

            return new ProgramSyntax(statements, endOfFile, this.lexicalErrors);
        }

        private SyntaxBase Declaration()
        {
            return this.WithRecovery(TokenType.NewLine, () =>
            {
                var current = reader.Peek();
                switch (current.Type)
                {
                    case TokenType.ParameterKeyword:
                        return this.ParameterDeclaration();

                    case TokenType.VariableKeyword:
                        return this.VariableDeclaration();

                    case TokenType.ResourceKeyword:
                        return this.ResourceDeclaration();

                    case TokenType.OutputKeyword:
                        return this.OutputDeclaration();

                    case TokenType.NewLine:
                        return this.NoOpStatement();
                }

                // TODO: Update when adding other statement types
                throw new ExpectedTokenException("This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.", current);
            });
        }

        private SyntaxBase ParameterDeclaration()
        {
            var keyword = Expect(TokenType.ParameterKeyword, "Expected the parameter keyword at this location.");
            var name = Identifier("Expected a parameter identifier at this location.");
            var type = Type($"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}");

            var current = reader.Peek();
            SyntaxBase? modifier = current.Type switch
            {
                // the parameter does not have a modifier
                TokenType.NewLine => null,

                // default value is specified
                TokenType.DefaultKeyword => this.ParameterDefaultValue(),

                // modifier is specified
                TokenType.LeftBrace => this.Object(),

                _ => throw new ExpectedTokenException("Expected the default keyword, a parameter modifier, or a newline at this location.", current)
            };

            var newLine = this.NewLine();

            return new ParameterDeclarationSyntax(keyword, name, type, modifier, newLine);
        }

        private SyntaxBase ParameterDefaultValue()
        {
            Token defaultKeyword = this.Expect(TokenType.DefaultKeyword, "Expected the default keyword at this location.");
            SyntaxBase defaultValue = this.Expression();

            return new ParameterDefaultValueSyntax(defaultKeyword, defaultValue);
        }

        private SyntaxBase VariableDeclaration()
        {
            var keyword = this.Expect(TokenType.VariableKeyword, "Expected the variable keyword at this location.");
            var name = this.Identifier("Expected a variable identifier at this location.");
            var assignment = this.Assignment();
            var value = this.Expression();

            var newLine = this.NewLine();

            return new VariableDeclarationSyntax(keyword, name, assignment, value, newLine);
        }

        private SyntaxBase OutputDeclaration()
        {
            var keyword = this.Expect(TokenType.OutputKeyword, "Expected the output keyword at this location.");
            var name = this.Identifier("Expected an output identifier at this location.");
            var type = Type($"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}");
            var assignment = this.Assignment();
            var value = this.Expression();

            var newLine = this.NewLine();

            return new OutputDeclarationSyntax(keyword, name, type, assignment, value, newLine);
        }

        private SyntaxBase ResourceDeclaration()
        {
            var keyword = this.Expect(TokenType.ResourceKeyword, "Expected the resource keyword at this location.");
            var name = this.Identifier("Expected a resource identifier at this location.");

            // TODO: Unify StringSyntax with TypeSyntax
            var type = this.StringLiteral();

            var assignment = this.Assignment();
            var body = this.Object();

            var newLine = this.NewLine();

            return new ResourceDeclarationSyntax(keyword, name, type, assignment, body, newLine);
        }

        private SyntaxBase NoOpStatement()
        {
            var newLine = this.NewLine();
            return new NoOpDeclarationSyntax(newLine);
        }

        private Token NewLine()
        {
            return Expect(TokenType.NewLine, "Expected a new line character at this location.");
        }

        public SyntaxBase Expression()
        {
            var candidate = this.BinaryExpression();

            if (this.Check(TokenType.Question))
            {
                var question = this.reader.Read();
                var trueExpression = this.Expression();
                var colon = this.Expect(TokenType.Colon, "Expected a : character at this location.");
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
                    Token closeSquare = this.Expect(TokenType.RightSquare, "Expected the ] character at this location.");

                    current = new ArrayAccessSyntax(current, openSquare, indexExpression, closeSquare);

                    continue;
                }

                if (this.Check(TokenType.Dot))
                {
                    // dot operator
                    Token dot = this.reader.Read();
                    IdentifierSyntax propertyName = this.Identifier("Expected a property identifier at this location.");

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
                case TokenType.String:
                case TokenType.Number:
                case TokenType.NullKeyword:
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    return this.LiteralValue();

                case TokenType.LeftBrace:
                    return this.Object();

                case TokenType.LeftSquare:
                    return this.Array();

                case TokenType.LeftParen:
                    return this.ParenthesizedExpression();

                case TokenType.Identifier:
                    return this.FunctionCallOrVariableAccess();

                default:
                    throw new ExpectedTokenException("Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.", nextToken);
            }
        }

        private SyntaxBase ParenthesizedExpression()
        {
            var openParen = this.Expect(TokenType.LeftParen, "Expected the ( character at this location.");
            var expression = this.Expression();
            var closeParen = this.Expect(TokenType.RightParen, "Expected the ) character at this location.");

            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }

        private SyntaxBase FunctionCallOrVariableAccess()
        {
            var identifier = this.Identifier("Expected a function name at this location.");

            if (this.Check(TokenType.LeftParen) == false)
            {
                // just a reference to a variable, parameter, or output
                return new VariableAccessSyntax(identifier);
            }

            var openParen = this.Expect(TokenType.LeftParen, "Expected the ( character at this location.");

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

                var comma = this.Expect(TokenType.Comma, "Expected the , character at this location.");
                
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
            return this.Expect(TokenType.Assignment, "Expected an = character at this location.");
        }

        private IdentifierSyntax Identifier(string message)
        {
            var identifier = Expect(TokenType.Identifier, message);

            return new IdentifierSyntax(identifier);
        }

        private TypeSyntax Type(string message)
        {
            var identifier = Expect(TokenType.Identifier, message);

            return new TypeSyntax(identifier);
        }

        private NumericLiteralSyntax NumericLiteral()
        {
            var literal = Expect(TokenType.Number, "Expected a numeric literal at this location.");

            if (Int32.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
            {
                return new NumericLiteralSyntax(literal, value);
            }

            // TODO: Should probably be moved to type checking
            // integer is invalid (too long to fit in an int32)
            throw new ExpectedTokenException("Expected a valid 32-bit signed integer.", literal);
        }

        private StringSyntax StringLiteral()
        {
            return new StringSyntax(reader.Read());
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

                case TokenType.String:
                    return StringLiteral();

                case TokenType.NullKeyword:
                    return new NullLiteralSyntax(reader.Read());

                default:
                    throw new ExpectedTokenException("The type of the specified value is incorrect. Specify a string, boolean, or integer literal.", current);
            }
        }

        private SyntaxBase Array()
        {
            var items = new List<SyntaxBase>();
            
            var openBracket = Expect(TokenType.LeftSquare, "Expected a [ character at this location.");
            var newLines = this.NewLines();

            while (this.IsAtEnd() == false && this.reader.Peek().Type != TokenType.RightSquare)
            {
                var item = this.ArrayItem();
                items.Add(item);
            }

            var closeBracket = Expect(TokenType.RightSquare, "Expected a ] character at this location.");

            return new ArraySyntax(openBracket, newLines, items, closeBracket);
        }

        private SyntaxBase ArrayItem()
        {
            return this.WithRecovery(TokenType.NewLine, () =>
            {
                var value = this.Expression();
                var newLines = this.NewLines();

                return new ArrayItemSyntax(value, newLines);
            });
        }

        private SyntaxBase Object()
        {
            var properties = new List<SyntaxBase>();

            var openBrace = Expect(TokenType.LeftBrace, "Expected a { character at this location.");
            var newLines = this.NewLines();

            while (this.IsAtEnd() == false && this.reader.Peek().Type != TokenType.RightBrace)
            {
                var property = this.ObjectProperty();
                properties.Add(property);
            }

            var closeBrace = Expect(TokenType.RightBrace, "Expected a } character at this location.");

            return new ObjectSyntax(openBrace, newLines, properties, closeBrace);
        }

        private SyntaxBase ObjectProperty()
        {
            return this.WithRecovery(TokenType.NewLine, () =>
            {
                var identifier = this.Identifier("Expected a property name at this location.");
                var colon = Expect(TokenType.Colon, "Expected a colon character at this location.");
                var value = Expression();
                var newLines = this.NewLines();

                return new ObjectPropertySyntax(identifier, colon, value, newLines);
            });
        }

        private SyntaxBase WithRecovery<TSyntax>(TokenType terminatingType, Func<TSyntax> syntaxFunc)
            where TSyntax : SyntaxBase
        {
            var startPosition = reader.Position;
            try
            {
                return syntaxFunc();
            }
            catch (ExpectedTokenException exception)
            {
                this.Synchronize(terminatingType);
                
                var skippedTokens = reader.Slice(startPosition, reader.Position - startPosition);
                return new SkippedTokensTriviaSyntax(skippedTokens, exception.Message, exception.UnexpectedToken);
            }
        }

        private void Synchronize(TokenType expectedType)
        {
            while (!IsAtEnd())
            {
                TokenType nextType = this.reader.Peek().Type;
                if (Match(expectedType))
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

        private Token Expect(TokenType type, string message)
        {
            if (this.Check(type))
            {
                // only read the token if it matches the expectations
                // otherwise, we could accidentally consume EOF
                return reader.Read();
            }

            throw new ExpectedTokenException(message, this.reader.Peek());
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