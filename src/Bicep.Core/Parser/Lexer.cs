using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Parser
{
    public class Lexer
    {
        // TODO: Make immutable
        private static readonly IDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        { 
            ["parameter"] = TokenType.ParameterKeyword,
            //["output"] = TokenType.OutputKeyword,
            //["variable"] = TokenType.VariableKeyword,
            //["resource"] = TokenType.ResourceKeyword,
            //["module"] = TokenType.ModuleKeyword,
            ["true"] = TokenType.TrueKeyword,
            ["false"] = TokenType.FalseKeyword,
        };

        private readonly IList<Token> tokens = new List<Token>();
        private readonly IList<Error> errors = new List<Error>();
        private readonly SlidingTextWindow textWindow;

        public Lexer(SlidingTextWindow textWindow)
        {
            this.textWindow = textWindow;
        }

        private void AddError(string message)
        {
            this.errors.Add(new Error(message, textWindow.GetSpan()));
        }

        public void Lex()
        {
            while (!textWindow.IsAtEnd())
            {
                LexToken();
            }

            // make sure we always include an end-of-line
            if (!tokens.Any() || tokens.Last().Type != TokenType.EndOfFile)
            {
                LexToken();
            }
        }

        public IEnumerable<Token> GetTokens() => tokens;

        public IEnumerable<Error> GetErrors() => errors;

        private void ScanTrailingTrivia()
        {
            ScanWhitespace();
            
            if (textWindow.Peek() == '/' && textWindow.Peek(1) == '/')
            {
                ScanSingleLineComment();
                return;
            }

            if (textWindow.Peek() == '/' && textWindow.Peek(1) == '*')
            {
                ScanMultiLineComment();
                return;
            }
        }

        private void ScanLeadingTrivia()
        {
            while (true)
            {
                if (IsWhiteSpace(textWindow.Peek()))
                {
                    ScanWhitespace();
                }
                else if (textWindow.Peek() == '/' && textWindow.Peek(1) == '/')
                {
                    ScanSingleLineComment();
                }
                else if (textWindow.Peek() == '/' && textWindow.Peek(1) == '*')
                {
                    ScanMultiLineComment();
                }
                else
                {
                    return;
                }
            }
        }

        private void LexToken()
        {
            textWindow.Reset();
            ScanLeadingTrivia();
            var leadingTrivia = textWindow.GetText();

            textWindow.Reset();
            var tokenType = ScanToken();
            var tokenText = textWindow.GetText();
            var tokenSpan = textWindow.GetSpan();

            if (tokenType == TokenType.Unrecognized)
            {
                AddError("Unrecognized token");
            }

            textWindow.Reset();
            ScanTrailingTrivia();
            var trailingTrivia = textWindow.GetText();

            var token = new Token(tokenType, tokenSpan, tokenText, leadingTrivia, trailingTrivia);
            this.tokens.Add(token);
        }

        private void ScanWhitespace()
        {
            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();
                switch (nextChar)
                {
                    case ' ':
                    case '\t':
                        textWindow.Advance();
                        break;
                    default:
                        return;
                }
            }
        }

        private void ScanSingleLineComment()
        {
            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar == '\n')
                {
                    return;
                }
            }
        }

        private void ScanMultiLineComment()
        {
            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar != '*')
                {
                    continue;
                }

                if (textWindow.IsAtEnd())
                {
                    return;
                }

                nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar == '/')
                {
                    return;
                }
            }
        }

        private void ScanString()
        {
            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    this.AddError("Unterminated string");
                    return;
                }

                var nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar == '\'')
                {
                    return;
                }

                if (nextChar != '\\')
                {
                    continue;
                }

                if (textWindow.IsAtEnd())
                {
                    this.AddError("Unterminated string");
                    return;
                }

                nextChar = textWindow.Peek();
                textWindow.Advance();

                switch (nextChar)
                {
                    case 'n':
                    case 'r':
                    case 't':
                    case '\\':
                    case '\'':
                    case '$':
                        break;
                    default:
                        this.AddError("Unrecognized escape sequence");
                        break;
                }
            }
        }

        private void ScanNumber()
        {
            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    return;
                }

                if (!IsDigit(textWindow.Peek()))
                {
                    return;
                }

                textWindow.Advance();
            }
        }

        private TokenType GetIdentifierTokenType(int length)
        {
            var identifier = textWindow.GetText();

            if (Keywords.TryGetValue(identifier, out var tokenType))
            {
                return tokenType;
            }
            return TokenType.Identifier;
        }

        private TokenType ScanIdentifier()
        {
            var length = 1;
            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    return GetIdentifierTokenType(length);
                }

                if (!IsAlphaNumeric(textWindow.Peek()))
                {
                    return GetIdentifierTokenType(length);
                }

                textWindow.Advance();
                length++;
            }
        }

        private TokenType ScanToken()
        {
            if (textWindow.IsAtEnd())
            {
                return TokenType.EndOfFile;
            }

            var nextChar = textWindow.Peek();
            textWindow.Advance();
            switch (nextChar)
            {
                case '{':
                    return TokenType.LeftBrace;
                case '}':
                    return TokenType.RightBrace;
                case '(':
                    return TokenType.LeftParen;
                case ')':
                    return TokenType.RightParen;
                case '[':
                    return TokenType.LeftSquare;
                case ']':
                    return TokenType.RightSquare;
                case ',':
                    return TokenType.Comma;
                case '.':
                    return TokenType.Dot;
                case '?':
                    return TokenType.Question;
                case ':':
                    return TokenType.Colon;
                case ';':
                    return TokenType.Semicolon;
                case '+':
                    return TokenType.Plus;
                case '-':
                    return TokenType.Plus;
                case '%':
                    return TokenType.Modulus;
                case '*':
                    return TokenType.Asterisk;
                case '/':
                    return TokenType.Slash;
                case '!':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '=':
                                textWindow.Advance();
                                return TokenType.NotEquals;
                            case '~':
                                textWindow.Advance();
                                return TokenType.NotEqualsInsensitive;
                        }
                    }
                    return TokenType.Exclamation;
                case '<':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '=':
                                textWindow.Advance();
                                return TokenType.LessThanOrEqual;
                        }
                    }
                    return TokenType.LessThan;
                case '>':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '=':
                                textWindow.Advance();
                                return TokenType.GreaterThanOrEqual;
                        }
                    }
                    return TokenType.GreaterThan;
                case '=':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '=':
                                textWindow.Advance();
                                return TokenType.Equals;
                            case '~':
                                textWindow.Advance();
                                return TokenType.EqualsInsensitive;
                        }
                    }
                    return TokenType.Assignment;
                case '&':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '&':
                                textWindow.Advance();
                                return TokenType.BinaryAnd;
                        }
                    }
                    return TokenType.Unrecognized;
                case '|':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '|':
                                textWindow.Advance();
                                return TokenType.BinaryOr;
                        }
                    }
                    return TokenType.Unrecognized;
                case '\'':
                    ScanString();
                    return TokenType.String;
                case '\n':
                    return TokenType.NewLine;
                case '\r':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '\n':
                                textWindow.Advance();
                                return TokenType.NewLine;
                        }
                    }
                    return TokenType.NewLine;
                default:
                    if (IsDigit(nextChar))
                    {
                        this.ScanNumber();
                        return TokenType.Number;
                    }

                    if (IsAlpha(nextChar))
                    {
                        return this.ScanIdentifier();
                    }

                    return TokenType.Unrecognized;
            }
        }

        private static bool IsAlpha(char c)
            => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private static bool IsAlphaNumeric(char c)
            => IsAlpha(c) || IsDigit(c);

        private static bool IsDigit(char c)
            => c >= '0' && c <= '9';

        private static bool IsWhiteSpace(char c)
            => c == ' ' || c == '\t';
    }
}