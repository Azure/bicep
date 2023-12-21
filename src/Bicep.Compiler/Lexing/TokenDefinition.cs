// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Compiler.Lexing.LexemeMatchers;

namespace Bicep.Compiler.Lexing;

public class TokenDefinition
{
    private TokenDefinition(TokenKind kind, ILexemeMatcher matcher)
    {
        this.Kind = kind;
        this.Matcher = matcher;
    }

    public TokenKind Kind { get; }

    public ILexemeMatcher Matcher { get; }

    public static TokenDefinition Create(TokenKind kind, char pattern) =>
        new(kind, new CharacterMatcher(pattern));

    public static TokenDefinition Create(TokenKind kind, string pattern) =>
        new(kind, new StringMatcher(pattern));

    public static TokenDefinition Create(TokenKind kind, Regex pattern) =>
        new(kind, new RegexMatcher(pattern));
}
