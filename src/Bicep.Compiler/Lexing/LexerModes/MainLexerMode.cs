// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Compiler.Lexing.LexerModes;

public partial class MainLexerMode : LexerMode
{
    private readonly static IReadOnlyDictionary<char, TokenKind> TokenKindsByPunctuators = InitializeTokenKindsByPunctuators();

    private readonly static IReadOnlyDictionary<string, TokenKind> TokenKindsByReservedKeywords = InitializeTokenKindsByReservedKeywords();

    public MainLexerMode(LexerBuffer buffer)
        : base(buffer)
    {
    }

    public override Token Next(Stack<LexerMode> modes)
    {
        if (this.Buffer.IsEmpty)
        {
            return new Token(TokenKind.EndOfFile);
        }

        var nextCharacter = this.Buffer.PeekOne();

        if (nextCharacter is '\'')
        {
            this.Buffer.Advance();
            modes.Push(new StringLexerMode(this.Buffer));

            return new Token(TokenKind.StringStart);
        }

        if (TokenKindsByPunctuators.TryGetValue(nextCharacter, out var tokenKind))
        {
            this.Buffer.Advance();

            var compositeOperatorKind = (nextCharacter, this.Buffer.PeekOne()) switch
            {
                ('=', '=') => TokenKind.Equals,
                ('!', '=') => TokenKind.NotEquals,
                ('<', '=') => TokenKind.LessThanOrEquals,
                ('>', '=') => TokenKind.GreaterThanOrEquals,
                ('=', '~') => TokenKind.EqualsInsensitive,
                ('!', '~') => TokenKind.NotEqualsInsensitive,
                ('&', '&') => TokenKind.LogicalAnd,
                ('|', '|') => TokenKind.LogicalOr,

                (_, _) => tokenKind,
            };

            if (compositeOperatorKind != tokenKind)
            {
                this.Buffer.Advance();

                return new Token(compositeOperatorKind);
            }

            return new Token(tokenKind);
        }

        foreach (var identifierMatch in IdentifierPattern().EnumerateMatches(this.Buffer.AsSpan()))
        {
            this.Buffer.Advance(identifierMatch.Length);
        }
    }

    private static IReadOnlyDictionary<char, TokenKind> InitializeTokenKindsByPunctuators()
    {
        var result = new Dictionary<char, TokenKind>();

        foreach (var tokenKind in Enum.GetValues<TokenKind>())
        {
            if (tokenKind >= TokenKind.Equals)
            {
                break;
            }

            if (TokenFacts.TryGetText(tokenKind) is { Length: 1} text)
            {
                result[text[0]] = tokenKind;
            }
        }

        return result;
    }

    private static IReadOnlyDictionary<string, TokenKind> InitializeTokenKindsByReservedKeywords()
    {
        var result = new Dictionary<string, TokenKind>();

        foreach (var tokenKind in Enum.GetValues<TokenKind>())
        {
            if (tokenKind >= TokenKind.NullKeyword &&
                tokenKind < TokenKind.Identifier &&
                TokenFacts.TryGetText(tokenKind) is { } text)
            {
                result[text] = tokenKind;
            }
        }

        return result;
    }

    [GeneratedRegex(@"[_\p{L}\p{Nl}][\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]*")]
    private static partial Regex IdentifierPattern();
}
