// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Navigation
{
    public static class SyntaxBaseExtensions
    {
        public static SyntaxBase? TryFindMostSpecificNodeInclusive(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate) =>
            TryFindMostSpecificNodeInternal(root, offset, predicate, inclusive: true);

        public static SyntaxBase? TryFindMostSpecificNodeExclusive(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate) =>
            TryFindMostSpecificNodeInternal(root, offset, predicate, inclusive: false);

        private static SyntaxBase? TryFindMostSpecificNodeInternal(SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate, bool inclusive)
        {
            var visitor = new NavigationSearchVisitor(offset, predicate, inclusive);
            visitor.Visit(root);

            return visitor.Result;
        }

        public static TextSpan GetSpanIncludingTrivia(this SyntaxBase root)
        {
            if (root is not Token token)
            {
                return root.Span;
            }

            var first = token.LeadingTrivia.Where(t => !t.Span.IsNil).FirstOrDefault();
            var start = first is { } ? first.Span.Position : token.Span.Position;

            var last = token.TrailingTrivia.Where(t => !t.Span.IsNil).LastOrDefault();
            var end = last is { } ? last.GetEndPosition() : token.GetEndPosition();

            Debug.Assert(start >= 0 && end >= 0, "start and end shouldn't be nil");
            Debug.Assert(start <= end, "start <= end");

            return new TextSpan(start, end - start);
        }

        private sealed class NavigationSearchVisitor : CstVisitor
        {
            private readonly int offset;
            private readonly Func<SyntaxBase, bool> predicate;
            private readonly bool inclusive;

            public NavigationSearchVisitor(int offset, Func<SyntaxBase, bool> predicate, bool inclusive)
            {
                this.offset = offset;
                this.predicate = predicate;
                this.inclusive = inclusive;
            }

            public SyntaxBase? Result { get; private set; }

            protected override void VisitInternal(SyntaxBase node)
            {
                // check if offset is inside the node's span
                if (CheckNodeContainsOffset(node))
                {
                    // the node span contains the offset
                    // check the predicate
                    if (this.predicate(node))
                    {
                        // store the potential result
                        this.Result = node;
                    }

                    // visiting the children may find a more specific node
                    base.VisitInternal(node);
                }

                // the offset is outside of the node span
                // there's no point to visit the children
            }

            private bool CheckNodeContainsOffset(SyntaxBase node) => this.inclusive
                    ? node.Span.ContainsInclusive(offset)
                    : node.Span.Contains(offset);
        }
    }
}

