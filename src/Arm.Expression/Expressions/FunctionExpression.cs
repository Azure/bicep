// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Expressions
{
    using Azure.ResourceManager.Deployments.Core.Collections;
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Azure.ResourceManager.Deployments.Core.Extensions;
    using Azure.ResourceManager.Deployments.Core.Instrumentation.Extensions;
    using Azure.ResourceManager.Deployments.Core.Json;
    using Azure.ResourceManager.Deployments.Expression.Configuration;
    using Azure.ResourceManager.Deployments.Expression.Exceptions;
    using Azure.ResourceManager.Deployments.Expression.Extensions;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The function expression.
    /// </summary>
    public class FunctionExpression : LanguageExpression
    {
        /// <summary>
        /// The operations that support short circuit functionality.
        /// </summary>
        public static readonly string[] ShortCircuitFunctions = new string[] { ExpressionConstants.AndFunction, ExpressionConstants.OrFunction, ExpressionConstants.IfFunction };

        /// <summary>
        /// Gets or sets the expression function.
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// Gets or sets the expression function parameters.
        /// </summary>
        public LanguageExpression[] Parameters { get; set; }

        /// <summary>
        /// Gets or sets the expression function properties.
        /// </summary>
        public LanguageExpression[] Properties { get; set; }

        /// <summary>
        /// Gets or sets the cached expression value.
        /// </summary>
        private JToken Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionExpression"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="properties">The properties.</param>
        public FunctionExpression(string function, LanguageExpression[] parameters, LanguageExpression[] properties)
        {
            this.Function = function;
            this.Parameters = parameters;
            this.Properties = properties;
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="context">The expression evaluation context.</param>
        /// <param name="additionalInfo">The line info of context.</param>
        public override JToken EvaluateExpression(ExpressionEvaluationContext context, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (this.Value == null)
            {
                var parametersValues = this.EvaluateParameters(context, additionalInfo);
                var root = context.EvaluateFunction(functionExpression: this, additionalnfo: additionalInfo, parameters: parametersValues);

                var selectedProperties = new List<JToken>();
                try
                {
                    foreach (var propertyExpression in this.Properties.CoalesceEnumerable())
                    {
                        var property = propertyExpression.EvaluateExpression(context, additionalInfo);
                        selectedProperties.Add(property);

                        root = FunctionExpression.SelectProperty(
                            root: root,
                            property: property,
                            additionalInfo: additionalInfo);
                    }

                    this.Value = root;
                }
                catch (ExpressionException exception)
                {
                    // Note(camarvin): Policies with runtime functions, e.g. resourcegroup(), 
                    // may reference properties which don't exist on the object being 
                    // evaluated (e.g. resourcegroup().tags.tagName). Ignore such exceptions and 
                    // set the value to empty string instead so that policy evaluation can continue.
                    if (exception.IsFatal() ||
                        context.AllowInvalidProperty == null ||
                        !context.AllowInvalidProperty(
                            exception: exception,
                            functionExpression: this,
                            selectedProperties: selectedProperties.CoalesceEnumerable().SelectArray(property => property.ToString())))
                    {
                        throw;
                    }

                    this.Value = string.Empty;
                }
            }

            return this.Value;
        }

        /// <summary>
        /// Accept the given visitor. Visit the current expression and pass the visitor to child expressions.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(LanguageExpressionVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.OnFunctionExpression?.Invoke(this);

                // Continue visiting until explicitly told to stop
                if (visitor.ShortCircuitVisitor?.Invoke() != true)
                {
                    var expressionsEnumerator = this.Parameters.CoalesceEnumerable().Concat(this.Properties.CoalesceEnumerable()).GetEnumerator();
                    while (expressionsEnumerator.MoveNext() && visitor.ShortCircuitVisitor?.Invoke() != true)
                    {
                        expressionsEnumerator.Current.Accept(visitor);
                    }
                }
            }
        }

        #region Private methods

        /// <summary>
        /// Selects the property from JSON object.
        /// </summary>        
        /// <param name="root">The root node.</param>
        /// <param name="property">The property path.</param>
        /// <param name="additionalInfo">The additional info of the expression.</param>
        private static JToken SelectProperty(JToken root, JToken property, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (root == null)
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.NullExpressionProperty.ToLocalizedMessage(property),
                    additionalInfo: additionalInfo);
            }

            switch (root.Type)
            {
                case JTokenType.Array:
                    return FunctionExpression.SelectArrayProperty(
                        root: root.ToObject<JArray>(),
                        token: property,
                        additionalInfo: additionalInfo);

                case JTokenType.Object:
                    return FunctionExpression.SelectObjectProperty(
                        root: root.ToObject<JObject>(),
                        token: property,
                        additionalInfo: additionalInfo);

                default:
                    throw new ExpressionException(
                        message: ErrorResponseMessages.UnexpectedExpressionProperty.ToLocalizedMessage(property),
                        additionalInfo: additionalInfo);
            }
        }

        /// <summary>
        /// Selects the array property from JSON object.
        /// </summary>        
        /// <param name="root">The root node.</param>
        /// <param name="token">The child token.</param>
        /// <param name="additionalInfo">The line info of the expression.</param>
        private static JToken SelectArrayProperty(JArray root, JToken token, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (token.Type != JTokenType.Integer)
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.InvalidExpressionPropertyArrayIndex.ToLocalizedMessage(token),
                    additionalInfo: additionalInfo);
            }

            var index = token.ToObject<int>();
            if (index < 0 || index >= root.Count)
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.ExpressionPropertyArrayIndexOutOfBounds.ToLocalizedMessage(index),
                    additionalInfo: additionalInfo);
            }

            return root[index];
        }

        /// <summary>
        /// Selects the object property from JSON object.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="token">The child token.</param>
        /// <param name="additionalInfo">The line info of the expression.</param>
        private static JToken SelectObjectProperty(JObject root, JToken token, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (!token.IsTextBasedJTokenType())
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.NonStringExpressionProperty.ToLocalizedMessage(token.ToString()),
                    additionalInfo: additionalInfo);
            }

            JToken value;
            var propertyName = token.ToObject<string>();
            if (!root.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out value))
            {
                var availableProperties = root
                    .Properties()
                    .Select(property => property.Name)
                    .ConcatStrings(", ");

                throw new ExpressionException(
                    message: ErrorResponseMessages.NonExistingExpressionProperty.ToLocalizedMessage(propertyName, availableProperties),
                    additionalInfo: additionalInfo);
            }

            return value;
        }

        /// <summary>
        /// Evaluates the function parameters.
        /// </summary>
        /// <param name="context">The evaluation context.</param>
        /// <param name="additionalInfo">The line info of the evaluated context.</param>
        private JToken[] EvaluateParameters(ExpressionEvaluationContext context, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (FunctionExpression.ShortCircuitFunctions.ContainsInsensitively(this.Function) && context.IsShortCircuitAllowed)
            {
                if (this.Function.EqualsInsensitively(ExpressionConstants.IfFunction))
                {
                    return this.EvaluateShortCircuitIf(context, additionalInfo);
                }

                return this.Function.EqualsInsensitively(ExpressionConstants.AndFunction)
                    ? this.EvaluateShortCircuitParameters(context: context, defaultValue: true, additionalInfo: additionalInfo)
                    : this.EvaluateShortCircuitParameters(context: context, defaultValue: false, additionalInfo: additionalInfo);
            }

            return this.Parameters.SelectArray(parameter => parameter.EvaluateExpression(context, additionalInfo));
        }

        /// <summary>
        /// Evaluates only the correct function value.
        /// </summary>
        /// <param name="context">The evaluation context.</param>
        /// <param name="additionalInfo">The line info of the evaluated context.</param>
        private JToken[] EvaluateShortCircuitIf(ExpressionEvaluationContext context, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (this.Parameters.Length == 3)
            {
                var conditionValue = this.GetBooleanParameterValue(this.Parameters[0], context, additionalInfo);

                // note(racces): We need to set the parameters to eliminate a possible reference value during preprocessing.
                if (conditionValue)
                {
                    this.Parameters[2] = new JTokenExpression(string.Empty, additionalInfo);
                }
                else
                {
                    this.Parameters[1] = new JTokenExpression(string.Empty, additionalInfo);
                }

                var resultParameter = conditionValue
                    ? this.Parameters[1].EvaluateExpression(context, additionalInfo)
                    : this.Parameters[2].EvaluateExpression(context, additionalInfo);

                return new JToken[]
                {
                    conditionValue.ToJToken(),
                    resultParameter,
                    resultParameter
                };
            }

            return this.Parameters.SelectArray(parameter => parameter.EvaluateExpression(context, additionalInfo));
        }

        /// <summary>
        /// Evaluates the function parameters for short circuit enabled functions.
        /// </summary>
        /// <param name="context">The evaluation context.</param>
        /// <param name="defaultValue">The boolean value that won't cause a short circuit.</param>
        /// <param name="additionalInfo">The line info of the evaluated context.</param>
        private JToken[] EvaluateShortCircuitParameters(ExpressionEvaluationContext context, bool defaultValue, TemplateErrorAdditionalInfo additionalInfo)
        {
            var parameterValues = Enumerable
                .Repeat(defaultValue.ToJToken(), this.Parameters.Length)
                .ToArray();

            var defaultValueExpression = new FunctionExpression(
                function: "bool",
                parameters: new JTokenExpression(defaultValue.ToString(), additionalInfo).AsArray(),
                properties: EmptyArray<LanguageExpression>.Instance);

            // note(racces): We need to set the parameters to eliminate a possible reference value during preprocessing.
            LanguageExpression breakingParameter = null;
            for (var i = 0; i < this.Parameters.Length; i++)
            {
                if (breakingParameter == null)
                {
                    var parameterValue = this.GetBooleanParameterValue(this.Parameters[i], context, additionalInfo);
                    parameterValues[i] = parameterValue.ToJToken();
                    if (parameterValue == !defaultValue)
                    {
                        breakingParameter = this.Parameters[i];
                    }
                }
                else
                {
                    this.Parameters[i] = defaultValueExpression;
                }
            }

            return parameterValues;
        }

        /// <summary>
        /// Gets the boolean value of a function parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="context">The evaluation context.</param>
        /// <param name="additionalInfo">The line info of evaluated context.</param>
        private bool GetBooleanParameterValue(LanguageExpression parameter, ExpressionEvaluationContext context, TemplateErrorAdditionalInfo additionalInfo)
        {
            var parameterValue = parameter.EvaluateExpression(context, additionalInfo);
            if (parameterValue.Type != JTokenType.Boolean)
            {
                throw new ExpressionException(
                    message: ErrorResponseMessages.InvalidTemplateFunctionParametersBooleanTypes.ToLocalizedMessage(this.Function),
                    additionalInfo: additionalInfo);
            }

            return parameterValue.ToObject<bool>();
        }

        #endregion
    }
}
