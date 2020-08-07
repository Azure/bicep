using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    public class Lexer
    {
        private static readonly ImmutableDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            ["default"] = TokenType.DefaultKeyword,
            ["parameter"] = TokenType.ParameterKeyword,
            ["output"] = TokenType.OutputKeyword,
            ["variable"] = TokenType.VariableKeyword,
            ["resource"] = TokenType.ResourceKeyword,
            //["module"] = TokenType.ModuleKeyword,
            ["true"] = TokenType.TrueKeyword,
            ["false"] = TokenType.FalseKeyword,
            ["null"] = TokenType.NullKeyword
        }.ToImmutableDictionary();

        // maps the escape character (that follows the backslash) to its value
        private static readonly ImmutableSortedDictionary<char, char> CharacterEscapes = new Dictionary<char, char>
        {
            {'n', '\n'},
            {'r', '\r'},
            {'t', '\t'},
            {'\\', '\\'},
            {'\'', '\''},

            // dollar character is reserved for future string interpolation work
            {'$', '$'}
        }.ToImmutableSortedDictionary();

        private static readonly string CharacterEscapeSequences = CharacterEscapes.Keys.Select(c => $"\\{c}").ConcatString(LanguageConstants.ListSeparator);

        private readonly IList<Token> tokens = new List<Token>();
        private readonly IList<Diagnostic> errors = new List<Diagnostic>();
        private readonly SlidingTextWindow textWindow;

        public Lexer(SlidingTextWindow textWindow)
        {
            this.textWindow = textWindow;
        }

        private void AddError(TextSpan span, DiagnosticBuilder.BuildDelegate errorFunc)
        {
            var error = errorFunc(DiagnosticBuilder.ForPosition(span));
            this.errors.Add(error);
        }

        private void AddError(DiagnosticBuilder.BuildDelegate errorFunc)
            => AddError(textWindow.GetSpan(), errorFunc);

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

        public ImmutableArray<Token> GetTokens() => tokens.ToImmutableArray();

        public ImmutableArray<Diagnostic> GetErrors() => errors.ToImmutableArray();

        /// <summary>
        /// Converts string literal text into its value. May return null if wrong token type is passed in or if the token is malformed.
        /// </summary>
        /// <param name="stringToken">the string token</param>
        public static string? TryGetStringValue(Token stringToken)
        {
            try
            {
                return GetStringValue(stringToken);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Converts string literal text into its value. May throw if the specified string token is malformed due to lexer error recovery.
        /// </summary>
        /// <param name="stringToken">the string token</param>
        public static string GetStringValue(Token stringToken)
        {
            if (stringToken.Type != TokenType.String)
            {
                throw new ArgumentException($"The specified token must be of type '{TokenType.String}' but is of type '{stringToken.Type}'.");
            }

            var window = new SlidingTextWindow(stringToken.Text);

            // first char of a string token must be the opening quote
            if (window.IsAtEnd() || window.Peek() != '\'')
            {
                // lexer guarantees this, but anything can create a token object
                throw new ArgumentException($"The text of the specified token must start with a single quote. Text = {stringToken.Text}");
            }

            window.Advance();

            // the value of the string will be shorter because escapes are longer than the characters they represent
            var buffer = new StringBuilder(stringToken.Text.Length);

            while (true)
            {
                if (window.IsAtEnd())
                {
                    // this is hit in non-terminated strings (which are allowed to allow for error recovery)
                    break;
                }

                var nextChar = window.Peek();
                window.Advance();

                if (nextChar == '\'')
                {
                    // string was terminated
                    // we should be at the end
                    if (window.IsAtEnd() == false)
                    {
                        throw new ArgumentException($"String token must not contain additional characters after the string-terminating single quote. Text = {stringToken.Text}");
                    }

                    break;
                }

                if (nextChar == '\\')
                {
                    // escape sequence begins
                    if (window.IsAtEnd())
                    {
                        // unterminated string (allowing this for error recovery purposes)
                        break;
                    }

                    char escapeChar = window.Peek();
                    window.Advance();

                    if (CharacterEscapes.TryGetValue(escapeChar, out char escapeCharValue) == false)
                    {
                        // invalid escape character
                        throw new ArgumentException($"String token contains an invalid escape character. Text = {stringToken.Text}");
                    }

                    buffer.Append(escapeCharValue);

                    // continue to next iteration
                    continue;
                }

                // regular string char - append to buffer
                buffer.Append(nextChar);
            }

            return buffer.ToString();
        }

        private IEnumerable<SyntaxTrivia> ScanTrailingTrivia()
        {
            if (IsWhiteSpace(textWindow.Peek()))
            {
                yield return ScanWhitespace();
            }
            
            if (textWindow.Peek() == '/' && textWindow.Peek(1) == '/')
            {
                yield return ScanSingleLineComment();
                yield break;
            }

            if (textWindow.Peek() == '/' && textWindow.Peek(1) == '*')
            {
                yield return ScanMultiLineComment();
                yield break;
            }
        }

        private IEnumerable<SyntaxTrivia> ScanLeadingTrivia()
        {
            while (true)
            {
                if (IsWhiteSpace(textWindow.Peek()))
                {
                    yield return ScanWhitespace();
                }
                else if (textWindow.Peek() == '/' && textWindow.Peek(1) == '/')
                {
                    yield return ScanSingleLineComment();
                }
                else if (textWindow.Peek() == '/' && textWindow.Peek(1) == '*')
                {
                    yield return ScanMultiLineComment();
                    yield break;
                }
                else
                {
                    yield break;
                }
            }
        }

        private void LexToken()
        {
            textWindow.Reset();
            ScanLeadingTrivia();
            // important to force enum evaluation here via .ToImmutableArray()!
            var leadingTrivia = ScanLeadingTrivia().ToImmutableArray();

            textWindow.Reset();
            var tokenType = ScanToken();
            var tokenText = textWindow.GetText();
            var tokenSpan = textWindow.GetSpan();
            
            if (tokenType == TokenType.Unrecognized)
            {
                AddError(b => b.UnrecognizedToken(tokenText));
            }

            textWindow.Reset();
            // important to force enum evaluation here via .ToImmutableArray()!
            var trailingTrivia = ScanTrailingTrivia().ToImmutableArray();

            var token = new Token(tokenType, tokenSpan, tokenText, leadingTrivia, trailingTrivia);
            this.tokens.Add(token);
        }

        private SyntaxTrivia ScanWhitespace()
        {
            textWindow.Reset();

            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();
                switch (nextChar)
                {
                    case ' ':
                    case '\t':
                        textWindow.Advance();
                        continue;
                }

                break;
            }

            return new SyntaxTrivia(SyntaxTriviaType.Whitespace, textWindow.GetSpan(), textWindow.GetText());
        }

        private SyntaxTrivia ScanSingleLineComment()
        {
            textWindow.Reset();
            textWindow.Advance(2);

            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();

                // make sure we don't include the newline in the comment trivia
                if (IsNewLine(nextChar))
                {
                    break;
                }

                textWindow.Advance();
            }

            return new SyntaxTrivia(SyntaxTriviaType.SingleLineComment, textWindow.GetSpan(), textWindow.GetText());
        }

        private SyntaxTrivia ScanMultiLineComment()
        {
            textWindow.Reset();
            textWindow.Advance(2);

            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    AddError(b => b.UnterminatedMultilineComment());
                    break;
                }

                var nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar != '*')
                {
                    continue;
                }

                if (textWindow.IsAtEnd())
                {
                    AddError(b => b.UnterminatedMultilineComment());
                    break;
                }

                nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar == '/')
                {
                    break;
                }
            }

            return new SyntaxTrivia(SyntaxTriviaType.MultiLineComment, textWindow.GetSpan(), textWindow.GetText());
        }

        private void ScanNewLine()
        {
            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    return;
                }

                var nextChar = textWindow.Peek();
                if (IsNewLine(nextChar) == false)
                {
                    return;
                }

                textWindow.Advance();
            }
        }

        private void ScanString()
        {
            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    AddError(b => b.UnterminatedString());
                    return;
                }

                var nextChar = textWindow.Peek();

                if (IsNewLine(nextChar))
                {
                    // do not consume the new line character
                    AddError(b => b.UnterminatedStringWithNewLine());
                    return;
                }

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
                    AddError(b => b.UnterminatedStringEscapeSequenceAtEof());
                    return;
                }

                nextChar = textWindow.Peek();
                textWindow.Advance();

                if (CharacterEscapes.ContainsKey(nextChar) == false)
                {
                    // the span of the error is the incorrect escape sequence
                    AddError(textWindow.GetLookbehindSpan(2), b => b.UnterminatedStringEscapeSequenceUnrecognized(CharacterEscapeSequences));
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
                    return TokenType.Minus;
                case '%':
                    return TokenType.Modulo;
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
                                return TokenType.LogicalAnd;
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
                                return TokenType.LogicalOr;
                        }
                    }
                    return TokenType.Unrecognized;
                case '\'':
                    ScanString();
                    return TokenType.String;
                case '\n':
                case '\r':
                    this.ScanNewLine();
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

        // TODO: Need IsIdStart and IsIdContinue (to disallow starting identifiers with 0-9, for example)

        private static bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private static bool IsDigit(char c) => c >= '0' && c <= '9';

        private static bool IsWhiteSpace(char c) => c == ' ' || c == '\t';

        private static bool IsNewLine(char c) => c == '\n' || c == '\r';
    }
}