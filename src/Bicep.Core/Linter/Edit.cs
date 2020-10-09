// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Linter
{
    public class Edit: IPositionable
    {
        public Edit(TextSpan span, string text)
        {
            this.Span = span;
            this.Text = text;
        }

        public TextSpan Span { get; }

        public string Text { get; }

        public static Edit ReplacePosition(TextSpan span, string text)
            => new Edit(span, text);

        public static Edit ReplacePosition(IPositionable positionable, string text)
            => ReplacePosition(positionable.Span, text);
    }
}
