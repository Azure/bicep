// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// Returns whether the specified node is a descendant of the potential ancestor. Returns false if the node has not been indexed.
        /// </summary>
        bool IsDescendant(SyntaxBase node, SyntaxBase potentialAncestor);
    }
}
