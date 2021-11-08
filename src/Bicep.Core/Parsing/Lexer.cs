// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class Lexer
    {
        // maps the escape character (that follows the backslash) to its value
        private static readonly ImmutableSortedDictionary<char, char> SingleCharacterEscapes = new Dictionary<char, char>
        {
            {'n', '\n'},
            {'r', '\r'},
            {'t', '\t'},
            {'\\', '\\'},
            {'\'', '\''},
            {'$', '$'}
        }.ToImmutableSortedDictionary();

        private static readonly ImmutableArray<string> CharacterEscapeSequences = SingleCharacterEscapes.Keys
            .Select(c => $"\\{c}")
            .Append("\\u{...}")
            .ToImmutableArray();

        private const int MultilineStringTerminatingQuoteCount = 3;

        // the rules for parsing are slightly different if we are inside an interpolated string (for example, a new line should result in a lex error).
        // to handle this, we use a modal lexing pattern with a stack to ensure we're applying the correct set of rules.
        private readonly Stack<TokenType> templateStack = new Stack<TokenType>();
        private readonly IList<Token> tokens = new List<Token>();
        private readonly IDiagnosticWriter diagnosticWriter;
        private readonly SlidingTextWindow textWindow;

        public Lexer(SlidingTextWindow textWindow, IDiagnosticWriter diagnosticWriter)
        {
            this.textWindow = textWindow;
            this.diagnosticWriter = diagnosticWriter;
        }

        private void AddDiagnostic(TextSpan span, DiagnosticBuilder.ErrorBuilderDelegate diagnosticFunc)
        {
            var diagnostic = diagnosticFunc(DiagnosticBuilder.ForPosition(span));
            this.diagnosticWriter.Write(diagnostic);
        }

        private void AddDiagnostic(DiagnosticBuilder.ErrorBuilderDelegate diagnosticFunc)
            => AddDiagnostic(textWindow.GetSpan(), diagnosticFunc);

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

        /// <summary>
        /// Converts a set of string literal tokens into their raw values. Returns null if any of the tokens are of the wrong type or malformed.
        /// </summary>
        /// <param name="stringTokens">the string tokens</param>
        public static IEnumerable<string>? TryGetRawStringSegments(IReadOnlyList<Token> stringTokens)
        {
            var segments = new string[stringTokens.Count];

            for (var i = 0; i < stringTokens.Count; i++)
            {
                var nextSegment = Lexer.TryGetStringValue(stringTokens[i]);
                if (nextSegment == null)
                {
                    return null;
                }

                segments[i] = nextSegment;
            }

            return segments;
        }

        public static string? TryGetMultilineStringValue(Token stringToken)
        {
            var tokenText = stringToken.Text;

            if (tokenText.Length < MultilineStringTerminatingQuoteCount * 2)
            {
                return null;
            }

            for (var i = 0; i < MultilineStringTerminatingQuoteCount; i++)
            {
                if (tokenText[i] != '\'')
                {
                    return null;
                }
            }

            for (var i = tokenText.Length - MultilineStringTerminatingQuoteCount; i < tokenText.Length; i++)
            {
                if (tokenText[i] != '\'')
                {
                    return null;
                }
            }

            var startOffset = MultilineStringTerminatingQuoteCount;

            // we strip a leading \r\n or \n
            if (tokenText[startOffset] == '\r')
            {
                startOffset++;
            }
            if (tokenText[startOffset] == '\n')
            {
                startOffset++;
            }

            return tokenText.Substring(startOffset, tokenText.Length - startOffset - MultilineStringTerminatingQuoteCount);
        }

        /// <summary>
        /// Converts string literal text into its value. Returns null if the specified string token is malformed due to lexer error recovery.
        /// </summary>
        /// <param name="stringToken">the string token</param>
        public static string? TryGetStringValue(Token stringToken)
        {
            var (start, end) = stringToken.Type switch {
                TokenType.StringComplete => (LanguageConstants.StringDelimiter, LanguageConstants.StringDelimiter),
                TokenType.StringLeftPiece => (LanguageConstants.StringDelimiter, LanguageConstants.StringHoleOpen),
                TokenType.StringMiddlePiece => (LanguageConstants.StringHoleClose, LanguageConstants.StringHoleOpen),
                TokenType.StringRightPiece => (LanguageConstants.StringHoleClose, LanguageConstants.StringDelimiter),
                _ => (null, null),
            };

            if (start == null || end == null)
            {
                return null;
            }

            if (stringToken.Text.Length < start.Length + end.Length ||
                stringToken.Text.Substring(0, start.Length) != start ||
                stringToken.Text.Substring(stringToken.Text.Length - end.Length) != end)
            {
                // any lexer-generated token should not hit this problem as the start & end are already verified
                return null;
            }

            var contents = stringToken.Text.Substring(start.Length, stringToken.Text.Length - start.Length - end.Length);
            var window = new SlidingTextWindow(contents);

            // the value of the string will be shorter because escapes are longer than the characters they represent
            var buffer = new StringBuilder(contents.Length);

            while (!window.IsAtEnd())
            {
                var nextChar = window.Next();

                if (nextChar == '\'')
                {
                    return null;
                }

                if (nextChar == '\\')
                {
                    // escape sequence begins
                    if (window.IsAtEnd())
                    {
                        return null;
                    }

                    char escapeChar = window.Next();

                    if (escapeChar == 'u')
                    {
                        // unicode escape
                        char openCurly = window.Next();
                        if (openCurly != '{')
                        {
                            return null;
                        }

                        var codePointText = ScanHexNumber(window);
                        if(!TryParseCodePoint(codePointText, out uint codePoint))
                        {
                            // invalid codepoint
                            return null;
                        }

                        char closeCurly = window.Next();
                        if (closeCurly != '}')
                        {
                            return null;
                        }

                        char charOrHighSurrogate = CodepointToString(codePoint, out char lowSurrogate);
                        buffer.Append(charOrHighSurrogate);
                        if (lowSurrogate != SlidingTextWindow.InvalidCharacter)
                        {
                            // previous char was a high surrogate
                            // also append the low surrogate
                            buffer.Append(lowSurrogate);
                        }

                        continue;
                    }

                    if (SingleCharacterEscapes.TryGetValue(escapeChar, out char escapeCharValue) == false)
                    {
                        // invalid escape character
                        return null;
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

        private static bool TryParseCodePoint(string text, out uint codePoint) => uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint) && codePoint <= 0x10FFFF;

        /// <summary>
        /// Determines if the specified string is a valid identifier. To be considered a valid identifier, the string must start
        /// with the identifier start character and remaining characters must be identifier continuation characters.
        /// </summary>
        /// <param name="value">The value</param>
        public static bool IsValidIdentifier(string value)
        {
            if (value.Length <= 0)
            {
                return false;
            }

            var result = IsIdentifierStart(value[0]);
            var index = 1;
            while (result && index < value.Length)
            {
                result = result && IsIdentifierContinuation(value[index]);
                index++;
            }

            return result;
        }

        private static char CodepointToString(uint codePoint, out char lowSurrogate)
        {
            if (codePoint < 0x00010000)
            {
                lowSurrogate = SlidingTextWindow.InvalidCharacter;
                return (char) codePoint;
            }

            Debug.Assert(codePoint > 0x0000FFFF && codePoint <= 0x0010FFFF);
            lowSurrogate = (char)((codePoint - 0x00010000) % 0x0400 + 0xDC00);
            return (char)((codePoint - 0x00010000) / 0x0400 + 0xD800);
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
                else if (textWindow.Peek() == '#' && CheckAdjacentText(LanguageConstants.DisableNextLineDiagnosticsKeyword))
                {
                    yield return ScanDisableDiagnosticsStatement(LanguageConstants.DisableNextLineDiagnosticsKeyword,
                                                                 SyntaxTriviaType.DisableNextLineStatement);
                }
                else
                {
                    yield break;
                }
            }
        }

        private bool CheckAdjacentText(string text)
        {
            int i = 0;
            foreach (char c in text)
            {
                if (textWindow.Peek(i + 1) == c)
                {
                    i++;
                    continue;
                }
                else
                {
                    return false;
                }
            }

            if (i != text.Length)
            {
                return false;
            }

            return true;
        }

        private SyntaxTrivia ScanDisableDiagnosticsStatement(string text, SyntaxTriviaType syntaxTriviaType)
        {
            textWindow.Reset();
            textWindow.Advance(text.Length + 1); // Length of disable next statement plus #

            StringBuilder sb = new StringBuilder();

            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();
                sb.Append(nextChar);

                if (IsIdentifierContinuation(nextChar) || nextChar == '-' || nextChar == ' ' || nextChar == '\t')
                {
                    textWindow.Advance();
                }
                else
                {
                    break;
                }

            }

            if (string.IsNullOrWhiteSpace(sb.ToString()))
            {
                AddDiagnostic(b => b.MissingDiagnosticCodes());
            }

            return new SyntaxTrivia(syntaxTriviaType, textWindow.GetSpan(), textWindow.GetText());
        }

        private void LexToken()
        {
            textWindow.Reset();

            // important to force enum evaluation here via .ToImmutableArray()!
            var leadingTrivia = ScanLeadingTrivia().ToImmutableArray();

            textWindow.Reset();
            var tokenType = ScanToken();
            var tokenText = textWindow.GetText();
            var tokenSpan = textWindow.GetSpan();

            if (tokenType == TokenType.Unrecognized)
            {
                if (tokenText == "\"")
                {
                    AddDiagnostic(b => b.DoubleQuoteToken(tokenText));
                }
                else
                {
                    AddDiagnostic(b => b.UnrecognizedToken(tokenText));
                }
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
                    AddDiagnostic(b => b.UnterminatedMultilineComment());
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
                    AddDiagnostic(b => b.UnterminatedMultilineComment());
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

        private TokenType ScanMultilineString()
        {
            var successiveQuotes = 0;

            // we've already scanned the "'''", so get straight to scanning the string contents.
            while (!textWindow.IsAtEnd())
            {
                var nextChar = textWindow.Peek();
                textWindow.Advance();

                switch (nextChar)
                {
                    case '\'':
                        successiveQuotes++;
                        if (successiveQuotes == MultilineStringTerminatingQuoteCount)
                        {
                            // we've scanned the terminating "'''". Keep scanning as long as we find more "'" characters;
                            // it is possible for the verbatim string's last character to be "'", and we should accept this.
                            while (textWindow.Peek() == '\'')
                            {
                                textWindow.Advance();
                            }

                            return TokenType.MultilineString;
                        }
                        break;
                    default:
                        successiveQuotes = 0;
                        break;
                }
            }

            // We've reached the end of the file without finding terminating quotes.
            // We still want to return a string token so that highlighting shows up.
            AddDiagnostic(b => b.UnterminatedMultilineString());
            return TokenType.MultilineString;
        }

        private TokenType ScanStringSegment(bool isAtStartOfString)
        {
            // to handle interpolation, strings are broken down into multiple segments, to detect the portions of string between the 'holes'.
            // 'complete' string: a string with no holes (no interpolation), e.g. "'hello'"
            // string 'left piece': the portion of an interpolated string up to the first hole, e.g. "'hello$"
            // string 'middle piece': the portion of an interpolated string between two holes, e.g. "}hello${"
            // string 'right piece': the portion of an interpolated string after the last hole, e.g. "}hello'"

            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    AddDiagnostic(b => b.UnterminatedString());
                    return isAtStartOfString ? TokenType.StringComplete : TokenType.StringRightPiece;
                }

                var nextChar = textWindow.Peek();

                if (IsNewLine(nextChar))
                {
                    // do not consume the new line character
                    AddDiagnostic(b => b.UnterminatedStringWithNewLine());
                    return isAtStartOfString ? TokenType.StringComplete : TokenType.StringRightPiece;
                }

                int escapeBeginPosition = textWindow.GetAbsolutePosition();
                textWindow.Advance();

                if (nextChar == '\'')
                {
                    return isAtStartOfString ? TokenType.StringComplete : TokenType.StringRightPiece;
                }

                if (nextChar == '$' && !textWindow.IsAtEnd() && textWindow.Peek() == '{')
                {
                    textWindow.Advance();
                    return isAtStartOfString ? TokenType.StringLeftPiece : TokenType.StringMiddlePiece;
                }

                if (nextChar != '\\')
                {
                    continue;
                }

                // an escape sequence was started with \
                if (textWindow.IsAtEnd())
                {
                    // the escape was unterminated
                    AddDiagnostic(b => b.UnterminatedStringEscapeSequenceAtEof());
                    return isAtStartOfString ? TokenType.StringComplete : TokenType.StringRightPiece;
                }

                // the escape sequence has a char after the \
                // consume it
                nextChar = textWindow.Peek();
                textWindow.Advance();

                if (nextChar == 'u')
                {
                    // unicode escape

                    if (textWindow.IsAtEnd())
                    {
                        // string was prematurely terminated
                        // reusing the first check in the loop body to produce the diagnostic
                        continue;
                    }

                    nextChar = textWindow.Peek();
                    if (nextChar != '{')
                    {
                        // \u must be followed by {, but it's not
                        AddDiagnostic(textWindow.GetSpanFromPosition(escapeBeginPosition), b => b.InvalidUnicodeEscape());
                        continue;
                    }

                    textWindow.Advance();
                    if (textWindow.IsAtEnd())
                    {
                        // string was prematurely terminated
                        // reusing the first check in the loop body to produce the diagnostic
                        continue;
                    }

                    string codePointText = ScanHexNumber(textWindow);
                    if (textWindow.IsAtEnd())
                    {
                        // string was prematurely terminated
                        // reusing the first check in the loop body to produce the diagnostic
                        continue;
                    }

                    if (string.IsNullOrEmpty(codePointText))
                    {
                        // we didn't get any hex digits
                        AddDiagnostic(textWindow.GetSpanFromPosition(escapeBeginPosition), b => b.InvalidUnicodeEscape());
                        continue;
                    }

                    nextChar = textWindow.Peek();
                    if (nextChar != '}')
                    {
                        // hex digits must be followed by }, but it's not
                        AddDiagnostic(textWindow.GetSpanFromPosition(escapeBeginPosition), b => b.InvalidUnicodeEscape());
                        continue;
                    }

                    textWindow.Advance();

                    if (!TryParseCodePoint(codePointText, out _))
                    {
                        // code point is not actually valid
                        AddDiagnostic(textWindow.GetSpanFromPosition(escapeBeginPosition), b => b.InvalidUnicodeEscape());
                        continue;
                    }
                }
                else
                {
                    // not a unicode escape
                    if (SingleCharacterEscapes.ContainsKey(nextChar) == false)
                    {
                        // the span of the error is the incorrect escape sequence
                        AddDiagnostic(textWindow.GetLookbehindSpan(2), b => b.UnterminatedStringEscapeSequenceUnrecognized(CharacterEscapeSequences));
                    }
                }
            }
        }

        private static string ScanHexNumber(SlidingTextWindow window)
        {
            var buffer = new StringBuilder();
            while (true)
            {
                if (window.IsAtEnd())
                {
                    return buffer.ToString();
                }

                char current = window.Peek();
                if (!IsHexDigit(current))
                {
                    return buffer.ToString();
                }

                buffer.Append(current);
                window.Advance();
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

        private TokenType GetIdentifierTokenType()
        {
            var identifier = textWindow.GetText();

            if (LanguageConstants.Keywords.TryGetValue(identifier, out var tokenType))
            {
                return tokenType;
            }
            return TokenType.Identifier;
        }

        private TokenType ScanIdentifier()
        {
            while (true)
            {
                if (textWindow.IsAtEnd())
                {
                    return GetIdentifierTokenType();
                }

                if (!IsIdentifierContinuation(textWindow.Peek()))
                {
                    return GetIdentifierTokenType();
                }

                textWindow.Advance();
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
                    if (templateStack.Any())
                    {
                        // if we're inside a string interpolation hole, and we find an object open brace,
                        // push it to the stack, so that we can match it up against an object close brace.
                        // this allows us to determine whether we're terminating an object or closing an interpolation hole.
                        templateStack.Push(TokenType.LeftBrace);
                    }
                    return TokenType.LeftBrace;
                case '}':
                    if (templateStack.Any())
                    {
                        var prevTemplateToken = templateStack.Peek();
                        if (prevTemplateToken != TokenType.LeftBrace)
                        {
                            var stringToken = ScanStringSegment(false);
                            if (stringToken == TokenType.StringRightPiece)
                            {
                                templateStack.Pop();
                            }

                            return stringToken;
                        }
                    }
                    return TokenType.RightBrace;
                case '(':
                    return TokenType.LeftParen;
                case ')':
                    return TokenType.RightParen;
                case '[':
                    return TokenType.LeftSquare;
                case ']':
                    return TokenType.RightSquare;
                case '@':
                    return TokenType.At;
                case ',':
                    return TokenType.Comma;
                case '.':
                    return TokenType.Dot;
                case '?':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case '?':
                                textWindow.Advance();
                                return TokenType.DoubleQuestion;
                        }
                    }
                    return TokenType.Question;
                case ':':
                    if (!textWindow.IsAtEnd())
                    {
                        switch (textWindow.Peek())
                        {
                            case ':':
                                textWindow.Advance();
                                return TokenType.DoubleColon;
                        }
                    }
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
                    // "'''" means we're starting a multiline string.
                    if (textWindow.Peek(0) == '\'' && textWindow.Peek(1) == '\'')
                    {
                        textWindow.Advance(2);
                        return ScanMultilineString();
                    }

                    var token = ScanStringSegment(true);
                    if (token == TokenType.StringLeftPiece)
                    {
                        // if we're beginning a string interpolation statement, we need to keep track of it
                        templateStack.Push(token);
                    }
                    return token;
                case '\n':
                case '\r':
                    if (templateStack.Any())
                    {
                        // need to re-check the newline token on next pass
                        textWindow.Rewind();

                        // do not consume the new line character
                        // TODO: figure out a way to avoid returning this multiple times for nested interpolation
                        AddDiagnostic(b => b.UnterminatedStringWithNewLine());

                        templateStack.Clear();
                        return TokenType.StringRightPiece;
                    }
                    this.ScanNewLine();
                    return TokenType.NewLine;
                default:
                    if (IsDigit(nextChar))
                    {
                        this.ScanNumber();
                        return TokenType.Integer;
                    }

                    if (IsIdentifierStart(nextChar))
                    {
                        return this.ScanIdentifier();
                    }

                    return TokenType.Unrecognized;
            }
        }

        // obtaining the unicode category is expensive and should be avoided in the main cases
        private static bool IsIdentifierStart(char c) => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';

        // obtaining the unicode category is expensive and should be avoided in the main cases
        private static bool IsIdentifierContinuation(char c) => IsIdentifierStart(c) || IsDigit(c);

        private static bool IsDigit(char c) => c >= '0' && c <= '9';

        private static bool IsHexDigit(char c) => IsDigit(c) || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';

        private static bool IsWhiteSpace(char c) => c == ' ' || c == '\t';

        private static bool IsNewLine(char c) => c == '\n' || c == '\r';
    }
}
