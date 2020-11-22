// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    public static class SyntaxTriviaExtensions
    {
        public static SyntaxTrivia? TryFindMostSpecificTriviaInclusive(this SyntaxBase root, int offset, Func<SyntaxTrivia, bool> predicate) => 
            TryFindMostSpecificTriviaInternal(root, offset, predicate, inclusive: true);

        public static SyntaxTrivia? TryFindMostSpecificTriviaExclusive(this SyntaxBase root, int offset, Func<SyntaxTrivia, bool> predicate) => 
            TryFindMostSpecificTriviaInternal(root, offset, predicate, inclusive: false);

        private static SyntaxTrivia? TryFindMostSpecificTriviaInternal(SyntaxBase root, int offset, Func<SyntaxTrivia, bool> predicate, bool inclusive)
        {
            var visitor = new NavigationSearchVisitor(offset, predicate, inclusive);
            visitor.Visit(root);

            return visitor.Result;
        }

        private sealed class NavigationSearchVisitor : SyntaxVisitor
        {
            private readonly int offset;
            private readonly Func<SyntaxTrivia, bool> predicate;
            private readonly bool inclusive;

            public NavigationSearchVisitor(int offset, Func<SyntaxTrivia, bool> predicate, bool inclusive)
            {
                this.offset = offset;
                this.predicate = predicate;
                this.inclusive = inclusive;
            }

            public SyntaxTrivia? Result { get; private set; }

            public override void VisitSyntaxTrivia(SyntaxTrivia node)
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
                }
            }

            private bool CheckNodeContainsOffset(SyntaxTrivia node) => this.inclusive
                    ? node.Span.ContainsInclusive(offset)
                    : node.Span.Contains(offset);
        }
    }
}