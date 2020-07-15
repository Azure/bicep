using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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

        public ProgramSyntax Parse() => Program();

        private ProgramSyntax Program()
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

            Token? assignment = null;
            SyntaxBase? defaultValue = null;
            if (Check(TokenType.Assignment))
            {
                assignment = reader.Read();
                defaultValue = Value();
            }

            var newLine = this.NewLine();

            return new ParameterDeclarationSyntax(keyword, name, type, assignment, defaultValue, newLine);
        }

        private SyntaxBase VariableDeclaration()
        {
            var keyword = this.Expect(TokenType.VariableKeyword, "Expected the variable keyword at this location.");
            var name = this.Identifier("Expected a variable identifier at this location.");
            var assignment = this.Assignment();
            var value = this.Value();

            var newLine = this.NewLine();

            return new VariableDeclarationSyntax(keyword, name, assignment, value, newLine);
        }

        private SyntaxBase OutputDeclaration()
        {
            var keyword = this.Expect(TokenType.OutputKeyword, "Expected the output keyword at this location.");
            var name = this.Identifier("Expected an output identifier at this location.");
            var type = Type($"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}");
            var assignment = this.Assignment();
            var value = this.Value();

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

            if (int.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
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

        private SyntaxBase Value()
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

                case TokenType.LeftBrace:
                    return Object();

                case TokenType.LeftSquare:
                    return Array();

                default:
                    throw new ExpectedTokenException("The type of the specified value is incorrect. Specify a string, boolean, or integer literal.", current);
            }
        }

        private SyntaxBase Array()
        {
            var items = new List<ArrayItemSyntax>();
            
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

        private ArrayItemSyntax ArrayItem()
        {
            var value = this.Value();
            var newLines = this.NewLines();

            return new ArrayItemSyntax(value, newLines);
        }

        private SyntaxBase Object()
        {
            var properties = new List<ObjectPropertySyntax>();

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

        private ObjectPropertySyntax ObjectProperty()
        {
            var identifier = this.Identifier("Expected a property name at this location.");
            var colon = Expect(TokenType.Colon, "Expected a colon character at this location.");
            var value = Value();
            var newLines = this.NewLines();

            return new ObjectPropertySyntax(identifier, colon, value, newLines);
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
                this.SynchronizeExclusive(terminatingType);
                //Synchronize(terminatingType);

                //// there are situations where EOF is read which advances the reader position past the end of the list
                //if (this.reader.Position == this.reader.Count)
                //{
                //    // to correct that we need to step back
                //    this.reader.StepBack();
                //}

                var skippedTokens = reader.Slice(startPosition, reader.Position - startPosition);
                return new SkippedTokensTriviaSyntax(skippedTokens, exception.Message, exception.UnexpectedToken);
            }
        }

        private void Synchronize(TokenType expectedType)
        {
            while (!IsAtEnd())
            {
                if (Match(expectedType))
                {
                    return;
                }

                reader.Read();
            }
        }

        private void SynchronizeExclusive(TokenType expectedType)
        {
            while (true)
            {
                TokenType nextType = this.reader.Peek().Type;
                if (nextType == TokenType.EndOfFile || nextType == expectedType)
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
    }
}