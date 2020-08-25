// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a non-recursive kind of expression syntax node.
    /// </summary>
    public enum ExpressionKind
    {
        /// <summary>
        /// Not an expression node.
        /// </summary>
        None,

        /// <summary>
        /// Represents a simple literal like null, boolean, number, or an uninterpolated string.
        /// </summary>
        SimpleLiteral,

        /// <summary>
        /// Represents a complex literal like an object or array. Operators may appear in child nodes of complex literals.
        /// </summary>
        ComplexLiteral,

        /// <summary>
        /// Represents an operator.
        /// </summary>
        Operator
    }
}
