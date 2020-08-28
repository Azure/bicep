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
        /// Represents an object literal. Operators may appear in child nodes of object literals.
        /// </summary>
        ObjectLiteral,

        /// <summary>
        /// Represents an array literal. Operators may appear in child nodes of array literals.
        /// </summary>
        ArrayLiteral,

        /// <summary>
        /// Represents an operator.
        /// </summary>
        Operator
    }
}
