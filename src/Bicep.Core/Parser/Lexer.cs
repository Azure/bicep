// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

        private static readonly IEnumerable<string> CharacterEscapeSequences = CharacterEscapes.Keys.Select(c => $"\\{c}").ToArray();

        // the rules for parsing are slightly different if we are inside an interpolated string (for example, a new line should result in a lex error).
        // to handle this, we use a modal lexing pattern with a stack to ensure we're applying the correct set of rules.
        private readonly Stack<TokenType> templateStack = new Stack<TokenType>();
        private readonly IList<Token> tokens = new List<Token>();
        private readonly IList<Diagnostic> diagnostics = new List<Diagnostic>();
        private readonly SlidingTextWindow textWindow;

        public Lexer(SlidingTextWindow textWindow)
        {
            this.textWindow = textWindow;
        }

        private void AddDiagnostic(TextSpan span, DiagnosticBuilder.ErrorBuilderDelegate diagnosticFunc)
        {
            var diagnostic = diagnosticFunc(DiagnosticBuilder.ForPosition(span));
            this.diagnostics.Add(diagnostic);
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

        public ImmutableArray<Diagnostic> GetDiagnostics() => diagnostics.ToImmutableArray();

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

                    if (CharacterEscapes.TryGetValue(escapeChar, out char escapeCharValue) == false)
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
            
            // important to force enum evaluation here via .ToImmutableArray()!
            var leadingTrivia = ScanLeadingTrivia().ToImmutableArray();

            textWindow.Reset();
            var tokenType = ScanToken();
            var tokenText = textWindow.GetText();
            var tokenSpan = textWindow.GetSpan();
            
            if (tokenType == TokenType.Unrecognized)
            {
                AddDiagnostic(b => b.UnrecognizedToken(tokenText));
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

                if (textWindow.IsAtEnd())
                {
                    AddDiagnostic(b => b.UnterminatedStringEscapeSequenceAtEof());
                    return isAtStartOfString ? TokenType.StringComplete : TokenType.StringRightPiece;
                }

                nextChar = textWindow.Peek();
                textWindow.Advance();

                if (CharacterEscapes.ContainsKey(nextChar) == false)
                {
                    // the span of the error is the incorrect escape sequence
                    AddDiagnostic(textWindow.GetLookbehindSpan(2), b => b.UnterminatedStringEscapeSequenceUnrecognized(CharacterEscapeSequences));
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
                        return TokenType.Number;
                    }

                    if (IsIdentifierStart(nextChar))
                    {
                        return this.ScanIdentifier();
                    }

                    return TokenType.Unrecognized;
            }
        }

        private static bool IsIdentifierStart(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private static bool IsIdentifierContinuation(char c) => IsIdentifierStart(c) || IsDigit(c);

        private static bool IsDigit(char c) => c >= '0' && c <= '9';

        private static bool IsWhiteSpace(char c) => c == ' ' || c == '\t';

        private static bool IsNewLine(char c) => c == '\n' || c == '\r';
    }
}
