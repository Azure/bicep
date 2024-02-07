// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetPlaceholder(int index, string? name, TextSpan span)
    {
        public int Index { get; } = index;

        public string? Name { get; } = name;

        public TextSpan Span { get; } = span;
    }
}
