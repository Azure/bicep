// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

        /// <summary>
        /// Gets all ancestor nodes assignable to <typeparamref name="TSyntax" /> in descending order
        /// from the top of the tree.
        /// </summary>
        /// <param name="syntax">The syntax node.</param>
        /// <typeparam name="TSyntax">The type of node to query.</typeparam>
        /// <returns>The list of ancestors.</returns>
        public ImmutableArray<TSyntax> GetAllAncestors<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase =>
            // Use default implementation
            ((ISyntaxHierarchy)this).GetAllAncestors<TSyntax>(syntax);

        /// <summary>
        /// Gets the nearest ancestor assignable to <typeparamref name="TSyntax" /> above <paramref name="syntax" />
        /// in an ascending walk towards the root of the syntax tree.
        /// </summary>
        /// <param name="syntax">The syntax node.</param>
        /// <typeparam name="TSyntax">The type of node to query.</typeparam>
        /// <returns>The nearest ancestor or <c>null</c>.</returns>
        public TSyntax? GetNearestAncestor<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase =>
            // Use default implementation
            ((ISyntaxHierarchy)this).GetNearestAncestor<TSyntax>(syntax);

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
