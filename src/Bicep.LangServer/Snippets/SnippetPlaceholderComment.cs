// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetPlaceholderComment
    {
        public SnippetPlaceholderComment(TextSpan span, string replacementText)
        {
            ReplacementText = replacementText;
            Span = span;
        }

        public string ReplacementText { get; }

        public TextSpan Span { get; }
    }
}
