// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Expressions
{
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Azure.ResourceManager.Deployments.Core.Instrumentation.Extensions;
    using Azure.ResourceManager.Deployments.Expression.Configuration;
    using Azure.ResourceManager.Deployments.Expression.Exceptions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The JToken expression.
    /// </summary>
    public class JTokenExpression : LanguageExpression
    {
        /// <summary>
        /// Gets the expression value.
        /// </summary>
        public JToken Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JTokenExpression"/> class.
        /// </summary>
        /// <param name="value">The expression value.</param>
        /// <param name="additionalInfo">The additional info of the value.</param>
        public JTokenExpression(string value, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            if (value.Length > ExpressionConstants.LiteralLimit)
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.ExpressionLiteralLimitExceeded.ToLocalizedMessage(ExpressionConstants.LiteralLimit, value.Length),
                    additionalInfo: additionalInfo);
            }

            this.Value = JValue.CreateString(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JTokenExpression"/> class.
        /// </summary>
        /// <param name="value">The expression value.</param>
        public JTokenExpression(int value)
        {
            this.Value = JValue.FromObject(value);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context">The expression evaluation context.</param>
        /// <param name="additionalInfo">The additional Info of the evaluated context.</param>
        public override JToken EvaluateExpression(ExpressionEvaluationContext context, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            return this.Value;
        }

        /// <summary>
        /// Accept the given visitor. Visit the current expression and pass the visitor to child expressions.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(LanguageExpressionVisitor visitor)
        {
            visitor?.OnJTokenExpression?.Invoke(this);
        }
    }
}
