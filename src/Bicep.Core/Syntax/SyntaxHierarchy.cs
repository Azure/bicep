// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Bicep.Core.Syntax
{
    public class SyntaxHierarchy : ISyntaxHierarchy
    {
        private readonly Dictionary<SyntaxBase, SyntaxBase?> parentMap = new Dictionary<SyntaxBase, SyntaxBase?>();

        /// <summary>
        /// Adds a root node and indexes the parents for all child nodes recursively.
        /// </summary>
        /// <param name="root">The root node.</param>
        public void AddRoot(SyntaxBase root)
        {
            var visitor = new ParentTrackingVisitor(this.parentMap);
            visitor.Visit(root);
        }

        /// <summary>
        /// Gets the parent of the specified node. Returns null for root nodes. Throws an exception for nodes that have not been indexed.
        /// </summary>
        /// <param name="node">The node</param>
        public SyntaxBase? GetParent(SyntaxBase node)
        {
            if (this.parentMap.TryGetValue(node, out var parent) == false)
            {
                throw new ArgumentException($"Unable to determine parent of specified node of type '{node.GetType().Name}' at span '{node.Span}' because it has not been indexed.");
            }

            return parent;
        }

        public bool IsDescendant(SyntaxBase node, SyntaxBase potentialAncestor)
        {
            var current = node;
            while (current != null)
            {
                current = this.GetParent(current);
                if (ReferenceEquals(current, potentialAncestor))
                {
                    return true;
                }
            }

            return false;
        }

        private sealed class ParentTrackingVisitor : SyntaxVisitor
        {
            private readonly Dictionary<SyntaxBase, SyntaxBase?> parentMap;
            private readonly Stack<SyntaxBase> currentParents = new Stack<SyntaxBase>();

            public ParentTrackingVisitor(Dictionary<SyntaxBase, SyntaxBase?> parentMap)
            {
                this.parentMap = parentMap;
            }

            protected override void VisitInternal(SyntaxBase node)
            {
                var parent = currentParents.Count <= 0 ? null : currentParents.Peek();
                parentMap.Add(node, parent);

                currentParents.Push(node);
                base.VisitInternal(node);
                currentParents.Pop();
            }
        }
    }
}
