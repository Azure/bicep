// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Compiler.Lexing.LexemeMatchers;

public class RegexMatcher : ILexemeMatcher
{
    private readonly Regex pattern;

    public RegexMatcher(Regex pattern)
    {
        this.pattern = pattern;
    }

    public int Match(ReadOnlySpan<char> text)
    {
        foreach (var match in this.pattern.EnumerateMatches(text))
        {
            return match.Length;
        }

        return 0;
    }
}
