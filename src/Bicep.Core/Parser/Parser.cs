using System;
using System.Collections.Generic;
using System.Globalization;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    public class Parser
    {
        private readonly TokenReader reader;
        
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
            return this.WithRecovery(TokenType.NewLine, () =>
            {
                var current = reader.Peek();
                switch (current.Type)
                {
                    case TokenType.ParameterKeyword:
                        return this.ParameterStatement();

                    case TokenType.NewLine:
                        return this.NoOpStatement();
                }

                // TODO: Update when adding other statement types
                throw new ExpectedTokenException("Unsupported statement type. Only parameter statements are allowed here.", current);
            });
        }

        private SyntaxBase ParameterStatement()
        {
            var keyword = Expect(TokenType.ParameterKeyword, "Expected parameter keyword at this location.");
            var name = Identifier();
            var type = Identifier();

            Token? assignment = null;
            SyntaxBase? defaultValue = null;
            if (Check(TokenType.Assignment))
            {
                assignment = reader.Read();
                defaultValue = DefaultValueSyntax();
            }

            var newLine = Expect(TokenType.NewLine, "Expected end of line at this location.");

            return new ParameterDeclarationSyntax(keyword, name, type, assignment, defaultValue, newLine);
        }

        private SyntaxBase NoOpStatement()
        {
            var newLine = Expect(TokenType.NewLine, "Expected end of line at this location.");
            return new NoOpDeclarationSyntax(newLine);
        }

        private IdentifierSyntax Identifier()
        {
            var identifier = Expect(TokenType.Identifier, "Expected an identifier.");

            return new IdentifierSyntax(identifier);
        }

        private NumericLiteralSyntax NumericLiteral()
        {
            var literal = Expect(TokenType.Number, "Expected a numeric literal.");

            if (int.TryParse(literal.Text, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
            {
                return new NumericLiteralSyntax(literal, value);
            }

            // TODO: Should probably be moved to type checking
            // integer is invalid (too long to fit in an int32)
            throw new ExpectedTokenException("Expected a valid 32-bit signed integer.", literal);
        }

        private SyntaxBase DefaultValueSyntax()
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
                    return new StringSyntax(reader.Read());

                default:
                    throw new ExpectedTokenException("Default values only support boolean literals, integer literals and strings.", current);
            }
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
                Synchronize(terminatingType);

                // there are situations where EOF is read which advances the reader position past the end of the list
                if (this.reader.Position == this.reader.Count)
                {
                    // to correct that we need to step back
                    this.reader.StepBack();
                }

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

            throw new ExpectedTokenException(message, token);
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