// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bicep.Core.PrettyPrint.Documents
{
    public class NestDocument : ILinkedDocument
    {
        private readonly int level;

        private readonly ImmutableArray<ILinkedDocument> successors;

        public NestDocument(int level)
            : this(level, ImmutableArray<ILinkedDocument>.Empty)
        {
        }

        public NestDocument(int level, ImmutableArray<ILinkedDocument> successors)
        {
            this.level = level;
            this.successors = successors;
        }

        public ILinkedDocument Concat(ILinkedDocument other)
        {
            return new NestDocument(this.level, this.successors.Add(other));
        }

        public ILinkedDocument Nest()
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            return new NestDocument(this.level + 1, this.successors.Select(s => s.Nest()).ToImmutableArray());
        }

        public void Layout(StringBuilder sb, string indent, string newline)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            sb.Append(newline);

            // Avoid putting whitespaces between newlines.
            if (this.successors.FirstOrDefault() is not NestDocument)
            {
                for (int i = 0; i < this.level; i++)
                {
                    sb.Append(indent);
                }
            }

            foreach (var successor in this.successors)
            {
                successor.Layout(sb, indent, newline);
            }
        }
    }
}
