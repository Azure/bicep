// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Parsers
{
    /// <summary>
    /// The expression token type.
    /// </summary>
    public enum ExpressionTokenType
    {
        /// <summary>
        /// The literal token type.
        /// </summary>
        Literal,

        /// <summary>
        /// The identifier token type.
        /// </summary>
        Identifier,

        /// <summary>
        /// The dot token type.
        /// </summary>
        Dot,

        /// <summary>
        /// The comma token type.
        /// </summary>
        Comma,

        /// <summary>
        /// The integer token type.
        /// </summary>
        Integer,

        /// <summary>
        /// The float token type.
        /// </summary>
        Float,

        /// <summary>
        /// The left parenthesis token type.
        /// </summary>
        LeftParenthesis,

        /// <summary>
        /// The right parenthesis token type.
        /// </summary>
        RightParenthesis,

        /// <summary>
        /// The left square bracket token type.
        /// </summary>
        LeftSquareBracket,

        /// <summary>
        /// The right square bracket token type.
        /// </summary>
        RightSquareBracket,

        /// <summary>
        /// The end of data  token type.
        /// </summary>
        EndOfData,

        /// <summary>
        /// The begin of data token type.
        /// </summary>
        BeginOfData
    }
}
