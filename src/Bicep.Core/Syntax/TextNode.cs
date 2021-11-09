// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class TextNode
    {
        public TextNode(string text, TextSpan span)
        {
            Text = text;
            Span = span;
        }

        public string Text { get; }

        public TextSpan Span { get; }
    }
}
