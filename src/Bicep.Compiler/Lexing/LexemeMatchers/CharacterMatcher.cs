// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing.LexemeMatchers;

public class CharacterMatcher : ILexemeMatcher
{
    private readonly char pattern;

    public CharacterMatcher(char pattern)
    {
        this.pattern = pattern;
    }

    public int Match(ReadOnlySpan<char> text) =>
        text.Length > 0 && text[0] == pattern ? 1 : 0;
}
