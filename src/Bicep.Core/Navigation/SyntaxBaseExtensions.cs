// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Syntax;

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

        private sealed class NavigationSearchVisitor : SyntaxVisitor
        {
            private readonly int offset;
            private readonly Func<SyntaxBase, bool> predicate;
            private readonly bool inclusive;
            private bool visitInstanceFunctionCallBaseExpression = false;

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

                    // when we are in a variable declaration syntax, if its value is an instance function call
                    // ensure to visit its base expression as it will contain the namespace name
                    if (node is VariableDeclarationSyntax)
                    {
                        visitInstanceFunctionCallBaseExpression = true;
                    }

                    // visiting the children may find a more specific node
                    base.VisitInternal(node);
                }

                // the offset is outside of the node span
                // there's no point to visit the children
            }

            public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            {
                // Skip base expression visit if the initial node visited is an instance function call, this is to ensure
                // that the instance function call syntax is returned as the most specific node, otherwise visiting a base expression
                // overrides the most specific node result.
                // In an instance function call, a base expression contains a namespace value, which means if the initial node visited
                // is an instance function call, visiting the base expression will set the most specific node to a namespace when the expectation
                // is to be set to a function call.
                if (visitInstanceFunctionCallBaseExpression)
                {
                    this.Visit(syntax.BaseExpression);
                }
                
                this.Visit(syntax.Dot);
                this.Visit(syntax.Name);
                this.Visit(syntax.OpenParen);
                this.VisitNodes(syntax.Arguments);
                this.Visit(syntax.CloseParen);
            }

            private bool CheckNodeContainsOffset(SyntaxBase node) => this.inclusive
                    ? node.Span.ContainsInclusive(offset)
                    : node.Span.Contains(offset);
        }
    }
}

