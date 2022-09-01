// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Syntax
{
    public static class ISyntaxHierarchyExtensions
    {
        /// <summary>
        /// Enumerate ancestor nodes in ascending order.
        /// </summary>
        public static IEnumerable<SyntaxBase> EnumerateAncestorsUpwards(this ISyntaxHierarchy hierarchy, SyntaxBase syntax)
        {
            var parent = hierarchy.GetParent(syntax);
            while (parent is not null)
            {
                yield return parent;

                parent = hierarchy.GetParent(parent);
            }
        }

        /// <summary>
        /// Gets all ancestor nodes assignable to <typeparamref name="TSyntax" /> in descending order
        /// from the top of the tree.
        /// </summary>
        /// <param name="syntax">The syntax node.</param>
        /// <typeparam name="TSyntax">The type of node to query.</typeparam>
        /// <returns>The list of ancestors.</returns>
        public static ImmutableArray<TSyntax> GetAllAncestors<TSyntax>(this ISyntaxHierarchy hierarchy, SyntaxBase syntax)
            where TSyntax : SyntaxBase
            => EnumerateAncestorsUpwards(hierarchy, syntax)
                .OfType<TSyntax>()
                .Reverse()
                .ToImmutableArray();

        /// <summary>
        /// Gets the nearest ancestor assignable to <typeparamref name="TSyntax" /> above <paramref name="syntax" />
        /// in an ascending walk towards the root of the syntax tree.
        /// </summary>
        /// <param name="syntax">The syntax node.</param>
        /// <typeparam name="TSyntax">The type of node to query.</typeparam>
        /// <returns>The nearest ancestor or <c>null</c>.</returns>
        public static TSyntax? GetNearestAncestor<TSyntax>(this ISyntaxHierarchy hierarchy, SyntaxBase syntax)
            where TSyntax : SyntaxBase
            => EnumerateAncestorsUpwards(hierarchy, syntax)
                .OfType<TSyntax>()
                .FirstOrDefault();
    }
}
