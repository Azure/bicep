// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents an operator acting on a pair of expressions.
    /// </summary>
    public enum BinaryOperator
    {
        /// <summary>
        /// Boolean OR
        /// </summary>
        LogicalOr,

        /// <summary>
        /// Boolean AND
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// Equality
        /// </summary>
        Equals,

        /// <summary>
        /// Not equals
        /// </summary>
        NotEquals,

        /// <summary>
        /// Case-insensitive equals
        /// </summary>
        EqualsInsensitive,

        /// <summary>
        /// Case-insensitive not equals
        /// </summary>
        NotEqualsInsensitive,

        /// <summary>
        /// Less than
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Greater than
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Addition
        /// </summary>
        Add,

        /// <summary>
        /// Subtraction
        /// </summary>
        Subtract,

        /// <summary>
        /// Multiplication
        /// </summary>
        Multiply,

        /// <summary>
        /// Division
        /// </summary>
        Divide,

        /// <summary>
        /// Modulo
        /// </summary>
        Modulo,

        /// <summary>
        /// Coalesce
        /// </summary>
        Coalesce
    }
}
