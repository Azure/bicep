// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;

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
            builder.Append(this.Text);
            Successor.Layout(builder, indent, newline);
        }
    }
}
