// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetPlaceholder
    {
        public SnippetPlaceholder(int index, string? name, TextSpan span)
        {
            this.Index = index;
            this.Name = name;
            this.Span = span;
        }

        public int Index { get; }

        public string? Name { get; }

        public TextSpan Span { get; }
    }
}