// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bicep.Core.Semantics
{
    /// <summary>
    /// Represents the hierarchy between symbols. For example, all global declarations have a parent that is a FileSymbol. All local scopes
    /// have parents who can be a local scope or the global scope represented by the FileSymbol.
    /// </summary>
    public class SymbolHierarchy
    {
        private readonly Dictionary<Symbol, Symbol?> parentMap = new Dictionary<Symbol, Symbol?>();

        /// <summary>
        /// Adds a root node and indexes the parents for all child nodes recursively.
        /// </summary>
        /// <param name="root">The root node.</param>
        public void AddRoot(Symbol root)
        {
            var visitor = new ParentTrackingVisitor(this.parentMap);
            visitor.Visit(root);
        }

        /// <summary>
        /// Gets the parent of the specified symbol. Returns null for root nodes. Throws an exception for nodes that have not been indexed.
        /// </summary>
        /// <param name="node">The node</param>
        public Symbol? GetParent(Symbol node)
        {
            if (this.parentMap.TryGetValue(node, out var parent) == false)
            {
                throw new ArgumentException($"Unable to determine parent of specified node of type '{node.GetType().Name}' with name '{node.Name}' because it has not been indexed.");
            }

            return parent;
        }

        private sealed class ParentTrackingVisitor : SymbolVisitor
        {
            private readonly Dictionary<Symbol, Symbol?> parentMap;
            private readonly Stack<Symbol> currentParents = new();

            public ParentTrackingVisitor(Dictionary<Symbol, Symbol?> parentMap)
            {
                this.parentMap = parentMap;
            }

            protected override void VisitInternal(Symbol node)
            {
                if(node is TypeSymbol)
                {
                    // the same type may be returned as a child of multiple symbols
                    // we can skip them for now
                    return;
                }

                var parent = currentParents.Count <= 0 ? null : currentParents.Peek();
                parentMap.Add(node, parent);

                currentParents.Push(node);
                base.VisitInternal(node);
                currentParents.Pop();
            }
        }
    }
}
