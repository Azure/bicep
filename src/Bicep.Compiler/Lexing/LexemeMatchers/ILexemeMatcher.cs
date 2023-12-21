// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Compiler.Lexing.LexemeMatchers;

public interface ILexemeMatcher
{
    int Match(ReadOnlySpan<char> text);
}
