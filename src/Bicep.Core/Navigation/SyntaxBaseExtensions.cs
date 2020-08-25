// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    public static class SyntaxBaseExtensions
    {
        public static SyntaxBase? TryFindMostSpecificNode(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate)
        {
            var visitor = new NavigationSearchVisitor(offset, predicate);
            visitor.Visit(root);

            return visitor.Result;
        }

        private sealed class NavigationSearchVisitor : SyntaxVisitor
        {
            private readonly int offset;
            private readonly Func<SyntaxBase, bool> predicate;

            public NavigationSearchVisitor(int offset, Func<SyntaxBase, bool> predicate)
            {
                this.offset = offset;
                this.predicate = predicate;
            }

            public SyntaxBase? Result { get; private set; }

            protected override void VisitInternal(SyntaxBase node)
            {
                // check if offset is inside the node's span
                if (node.Span.ContainsInclusive(offset))
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
        }
    }
}

