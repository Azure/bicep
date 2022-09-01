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
    }
}
