// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation //asdfg wrong place for SpanWithTrivia
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

        //asdfg
        //public static TextSpan SpanWithTrivia(this SyntaxBase root)
        //{
        //    if (root is Token token)
        //    {
        //        List<TextSpan> triviaSpans = [
        //            ..token.LeadingTrivia.Select(x => x.Span),
        //            ..token.TrailingTrivia.Select(x => x.Span)];
        //        return triviaSpans.Aggregate(root.Span, TextSpan.Between);
        //    }

        //    return root.Span;
        //}

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

