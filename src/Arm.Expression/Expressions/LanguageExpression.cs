// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Expressions
{
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The abstract expression.
    /// </summary>
    public abstract class LanguageExpression
    {
        /// <summary>
        /// Gets or sets the parent function expression.
        /// </summary>
        public FunctionExpression Parent { get; set; }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context">The expression evaluation context.</param>
        /// <param name="additionalInfo">The line info of the expression.</param>
        public abstract JToken EvaluateExpression(ExpressionEvaluationContext context, TemplateErrorAdditionalInfo additionalInfo = null);

        /// <summary>
        /// Accept the given visitor. Visit the current expression and pass the visitor to child expressions.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(LanguageExpressionVisitor visitor);
    }
}
