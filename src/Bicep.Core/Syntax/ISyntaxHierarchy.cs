// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Syntax
{
    public interface ISyntaxHierarchy
    {
        /// <summary>
        /// Gets the parent of the specified node. Returns null for root nodes. Throws an exception for nodes that have not been indexed.
        /// </summary>
        /// <param name="node">The node</param>
        SyntaxBase? GetParent(SyntaxBase node);

        /// <summary>
        /// Gets all ancestor nodes assignable to <typeparamref name="TSyntax" /> in descending order
        /// from the top of the tree.
        /// </summary>
        /// <param name="syntax">The syntax node.</param>
        /// <typeparam name="TSyntax">The type of node to query.</typeparam>
        /// <returns>The list of ancestors.</returns>
        public ImmutableArray<TSyntax> GetAllAncestors<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase
        {
            var ancestors = new List<TSyntax>();

            SyntaxBase? next = syntax;
            do
            {
                next = GetParent(next);
                if (next is TSyntax match)
                {
                    ancestors.Insert(0, match);
                }
            }
            while (next is not null);

            return ancestors.ToImmutableArray();
        }

        /// <summary>
        /// Gets the nearest ancestor assignable to <typeparamref name="TSyntax" /> above <paramref name="syntax" />
        /// in an ascending walk towards the root of the syntax tree.
        /// </summary>
        /// <param name="syntax">The syntax node.</param>
        /// <typeparam name="TSyntax">The type of node to query.</typeparam>
        /// <returns>The nearest ancestor or <c>null</c>.</returns>
        public TSyntax? GetNearestAncestor<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase
        {
            SyntaxBase? next = syntax;
            do
            {
                next = GetParent(next);
                if (next is TSyntax match)
                {
                    return match;
                }
            }
            while (next is not null);

            // Reached the top without finding a match.
            return null;
        }
    }
}