// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Bicep.Core.Parsing;

namespace Bicep.Core.PrettyPrint.Documents
{
    public class TextDocument : ILinkedDocument
    {
        private readonly string text;

        private readonly ImmutableArray<ILinkedDocument> successors;

        public TextDocument(string text)
            : this(text, ImmutableArray<ILinkedDocument>.Empty)
        {
        }

        public TextDocument(string text, ImmutableArray<ILinkedDocument> successors)
        {
            this.text = text;
            this.successors = successors;
        }

        public ILinkedDocument Concat(ILinkedDocument other)
        {
            return new TextDocument(this.text, this.successors.Add(other));
        }

        public ILinkedDocument Nest()
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            return new TextDocument(this.text, successors.Select(s => s.Nest()).ToImmutableArray());
        }

        public void Layout(StringBuilder sb, string indent, string newline)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            // Normalize newlines.
            sb.Append(StringUtils.ReplaceNewlines(this.text, newline));

            foreach (var successor in this.successors)
            {
                successor.Layout(sb, indent, newline);
            }
        }
    }
}
