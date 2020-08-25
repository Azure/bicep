// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents an operator acting on a single expression.
    /// </summary>
    public enum UnaryOperator
    {
        /// <summary>
        /// Boolean NOT
        /// </summary>
        Not,

        /// <summary>
        /// Unary minus operator (used to construct negative numbers)
        /// </summary>
        Minus
    }
}

