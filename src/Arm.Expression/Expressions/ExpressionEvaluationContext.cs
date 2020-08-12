// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Expressions
{
    using System;
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The expression function evaluation delegate.
    /// </summary>
    /// <param name="functionExpression">The function expression.</param>
    /// <param name="parameters">The function parameters.</param>
    /// <param name="additionalnfo">The additional info of the function expression.</param>
    public delegate JToken ExpressionFunctionEvaluation(FunctionExpression functionExpression, JToken[] parameters, TemplateErrorAdditionalInfo additionalnfo);

    /// <summary>
    /// The optimistic evaluation delegate.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public delegate bool OptimisticEvaluationCheck(Exception exception);

    /// <summary>
    /// The delegate for determining whether to ignore property selection exceptions
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="functionExpression">The function expression.</param>
    /// <param name="selectedProperties">The chain of properties selected which led to an exception.</param>
    public delegate bool IsValidPropertyCheck(Exception exception, FunctionExpression functionExpression, string[] selectedProperties);

    /// <summary>
    /// The expression evaluation context.
    /// </summary>
    public class ExpressionEvaluationContext
    {
        /// <summary>
        /// Gets or sets the function evaluation delegate.
        /// </summary>
        public ExpressionFunctionEvaluation EvaluateFunction { get; set; }

        /// <summary>
        /// Gets or sets the optimistic evaluation check delegate.
        /// </summary>
        public OptimisticEvaluationCheck CanEvaluateOptimistically { get; set; }

        /// <summary>
        /// Gets or sets the valid property check delegate. 
        /// </summary>
        public IsValidPropertyCheck AllowInvalidProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether short circuit evaluation is allowed.
        /// </summary>
        public bool IsShortCircuitAllowed { get; set; } = true;
    }
}
