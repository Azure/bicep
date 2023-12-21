// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing.LexemeMatchers;

public class StringMatcher : ILexemeMatcher
{
    private readonly string pattern;

    public StringMatcher(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException($"{nameof(pattern)} cannot be null or empty.");
        }

        this.pattern = pattern;
    }

    public int Match(ReadOnlySpan<char> text) =>
        text.StartsWith(pattern, StringComparison.Ordinal) ? pattern.Length : 0;
}
