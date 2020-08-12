// -----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Expressions
{
    using System;

    /// <summary>
    /// The language expression visitor.
    /// </summary>
    public class LanguageExpressionVisitor
    {
        /// <summary>
        /// Gets or sets the action to perform on function expression.
        /// </summary>
        public Action<FunctionExpression> OnFunctionExpression { get; set; }

        /// <summary>
        /// Gets or sets the action to perform on JToken expressions.
        /// </summary>
        public Action<JTokenExpression> OnJTokenExpression { get; set; }

        /// <summary>
        /// Gets or sets a function telling whether the visitor don't need to traverse the rest of the expression tree.
        /// </summary>
        public Func<bool> ShortCircuitVisitor { get; set; }
    }
}
