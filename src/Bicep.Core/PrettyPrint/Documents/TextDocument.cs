// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using Bicep.Core.Parsing;

namespace Bicep.Core.PrettyPrint.Documents
{
    public class TextDocument : ILinkedDocument
    {
        public TextDocument(string text, ILinkedDocument successor)
        {
            Text = text;
            Successor = successor;
        }

        public string Text { get; }

        public ILinkedDocument Successor { get; }

        public ILinkedDocument Concat(ILinkedDocument other)
        {
            return new TextDocument(this.Text, Successor.Concat(other));
        }

        public ILinkedDocument Nest(int level)
        {
            return new TextDocument(this.Text, Successor.Nest(level));
        }

        public void Layout(StringBuilder builder, string indent, string newline)
        {
            // Normalize newlines.
            var text = StringUtils.ReplaceNewlines(this.Text, newline);

            builder.Append(text);
            Successor.Layout(builder, indent, newline);
        }
    }
}
