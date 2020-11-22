// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.CodeAction
{
    public class CodeReplacement : IPositionable
    {
        public CodeReplacement(TextSpan span, string text)
        {
            this.Span = span;
            this.Text = text;
        }

        public TextSpan Span { get; }

        public string Text { get; }
    }
}
