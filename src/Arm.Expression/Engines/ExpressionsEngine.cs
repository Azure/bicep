// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Engines
{
    using Azure.ResourceManager.Deployments.Core.Collections;
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;
    using Azure.ResourceManager.Deployments.Core.Extensions;
    using Azure.ResourceManager.Deployments.Core.Helpers;
    using Azure.ResourceManager.Deployments.Core.Instrumentation.Extensions;
    using Azure.ResourceManager.Deployments.Core.Utilities;
    using Azure.ResourceManager.Deployments.Expression.Configuration;
    using Azure.ResourceManager.Deployments.Expression.Exceptions;
    using Azure.ResourceManager.Deployments.Expression.Expressions;
    using Azure.ResourceManager.Deployments.Expression.Extensions;
    using Azure.ResourceManager.Deployments.Expression.Parsers;
    using Azure.ResourceManager.Deployments.Expression.Serializers;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The expressions engine.
    /// </summary>
    public static class ExpressionsEngine
    {
        /// <summary>
        /// Determines whether specified value is language expression.
        /// </summary>
        /// <param name="expression">The language expression.</param>
        public static bool IsLanguageExpression(string expression)
        {
            return ExpressionParser.IsLanguageExpression(expression: expression);
        }

        /// <summary>
        /// Determines whether specified token has a language expression on a property name or on its direct value.
        /// </summary>
        /// <param name="root">The root node.</param>
        public static bool ContainsLanguageExpressionOnPropertyNameOrValue(JToken root)
        {
            if (root.Type == JTokenType.String)
            {
                return ExpressionsEngine.IsLanguageExpression(root.ToString());
            }

            var containsLanguageExpression = false;
            JsonUtility.WalkJsonRecursive(
                root: root,
                propertyAction:
                    jProperty =>
                    {
                        if (!string.IsNullOrEmpty(jProperty.Name) && ExpressionsEngine.IsLanguageExpression(jProperty.Name))
                        {
                            containsLanguageExpression = true;
                        }
                    });

            return containsLanguageExpression;
        }

        /// <summary>
        /// Parses the language expression.
        /// </summary>
        /// <param name="expression">The language expression.</param>
        /// <param name="additionalInfo">The additional Info of the expression.</param>
        public static LanguageExpression ParseLanguageExpression(string expression, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            return ExpressionParser.ParseLanguageExpression(expression, additionalInfo);
        }

        /// <summary>
        /// Parses the language expressions recursive.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        /// <param name="skipEvaluationPaths">The hash set of all paths that should be skipped in evaluation.</param>
        public static Dictionary<JToken, LanguageExpression> ParseLanguageExpressionsRecursive(JToken root, TemplateErrorAdditionalInfo additionalInfo = null, InsensitiveHashSet skipEvaluationPaths = null)
        {
            var expressions = new Dictionary<JToken, LanguageExpression>(
                comparer: JTokenReferenceComparer.Instance);

            JsonUtility.WalkJsonRecursive(
                root: root,
                propertyAction:
                    jProperty =>
                    {
                        if (!string.IsNullOrEmpty(jProperty.Name)
                            && ExpressionsEngine.IsLanguageExpression(jProperty.Name)
                            && !ExpressionsEngine.SkipEvaluation(jProperty.Path, skipEvaluationPaths))
                        {
                            expressions.Add(jProperty, ExpressionsEngine.ParseLanguageExpression(jProperty.Name, additionalInfo));
                        }
                    },
                tokenAction:
                    jToken =>
                    {
                        if (jToken.Type == JTokenType.String
                            && !ExpressionsEngine.SkipEvaluation(jToken.Path, skipEvaluationPaths))
                        {
                            var expression = jToken.ToString();
                            if (ExpressionsEngine.IsLanguageExpression(expression))
                            {
                                expressions.Add(jToken, ExpressionsEngine.ParseLanguageExpression(expression, additionalInfo));
                            }
                        }
                    });

            return expressions;
        }

        /// <summary>
        /// Check if the path should be skipped in evaluation.
        /// </summary>
        /// <param name="path">The JToken path.</param>
        /// <param name="skipEvaluationPaths">The paths that needs to be skipped in evaluation.</param>
        /// <returns>A boolean indicating if the path should be skipped.</returns>
        public static bool SkipEvaluation(string path, InsensitiveHashSet skipEvaluationPaths)
        {
            return skipEvaluationPaths != null
                && skipEvaluationPaths.Any(candidatePath => path.StartsWithInsensitively(candidatePath));
        }

        /// <summary>
        /// Evaluates the language expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The additional Info of the expression.</param>
        public static JToken EvaluateLanguageExpression(string expression, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (!ExpressionsEngine.IsLanguageExpression(expression))
            {
                if (expression == null)
                {
                    return null;
                }

                return expression;
            }

            return ExpressionsEngine.ParseLanguageExpression(expression, additionalInfo).EvaluateExpression(evaluationContext, additionalInfo);
        }

        /// <summary>
        /// Evaluates the language expression as string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The additional Info of the function.</param>
        public static string EvaluateLanguageExpressionAsString(string expression, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            if (expression != null)
            {
                var value = ExpressionsEngine.EvaluateLanguageExpression(expression, evaluationContext, additionalInfo);
                if (value == null || !value.IsTextBasedJTokenType())
                {
                    throw new ExpressionException(
                        message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(expression, value == null ? JTokenType.Null : value.Type, JTokenType.String),
                        additionalInfo: additionalInfo);
                }

                return value.ToString();
            }

            return expression;
        }

        /// <summary>
        /// Evaluates the language expression as integer.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        public static JToken EvaluateLanguageExpressionAsInteger(JToken expression, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo)
        {
            if (expression != null)
            {
                if (expression.Type == JTokenType.Integer)
                {
                    return expression;
                }
                else if (expression.Type == JTokenType.String)
                {
                    var value = ExpressionsEngine.EvaluateLanguageExpression(expression.ToString(), evaluationContext, additionalInfo);
                    if (value != null && value.Type == JTokenType.Integer)
                    {
                        return value;
                    }

                    throw new ExpressionException(
                        message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(expression, value != null ? value.Type : JTokenType.Null, JTokenType.Integer),
                        additionalInfo: additionalInfo);
                }

                throw new ExpressionException(
                    message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(expression, expression.Type, JTokenType.Integer),
                    additionalInfo: additionalInfo);
            }

            return expression;
        }

        /// <summary>
        /// Evaluates the language expression as boolean.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        public static bool EvaluateLanguageExpressionAsBoolean(JToken expression, ExpressionEvaluationContext evaluationContext)
        {
            if (expression != null)
            {
                if (expression.Type == JTokenType.Boolean)
                {
                    return (bool)expression;
                }
                else if (expression.Type == JTokenType.String)
                {
                    var value = ExpressionsEngine.EvaluateLanguageExpression(expression: expression.ToString(), evaluationContext: evaluationContext, additionalInfo: null);
                    if (value != null && value.Type == JTokenType.Boolean)
                    {
                        return (bool)value;
                    }

                    throw new ExpressionException(
                        message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(expression, value != null ? value.Type : JTokenType.Null, JTokenType.Boolean));
                }

                throw new ExpressionException(
                    message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(expression, expression.Type, JTokenType.Boolean));
            }

            throw new ExpressionException(
                message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(expression, JTokenType.Null, JTokenType.Boolean));
        }

        /// <summary>
        /// Evaluates language expression as one of the JToken types
        /// </summary>
        /// <param name="jtokenTypes">The list of JToken types.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        public static JToken EvaluateLanguageExpressionAsAny(JTokenType[] jtokenTypes, JToken expression, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            // Note(ramya): Tested this method with basic types string, integer, float
            if (expression != null && jtokenTypes.CoalesceEnumerable().Any())
            {
                if (jtokenTypes.Contains(expression.Type) && expression.Type != JTokenType.String)
                {
                    return EvaluateLanguageExpressionsRecursive(expression, evaluationContext, additionalInfo);
                }
                else if (expression.Type == JTokenType.String)
                {
                    var value = ExpressionsEngine.EvaluateLanguageExpression(expression.ToString(), evaluationContext, additionalInfo);
                    if (value != null && jtokenTypes.Contains(value.Type))
                    {
                        // Note(ramya): Returning the value as is and not reprocessing object/array types here.
                        return value;
                    }

                    throw new ExpressionException(
                        message: ErrorResponseMessages.UnsupportedExpressionEvaluationType.ToLocalizedMessage(
                            expression,
                            value != null ? value.Type : JTokenType.Null,
                            jtokenTypes.Select(type => type.ToString()).ConcatStrings(", ")),
                         additionalInfo: additionalInfo);
                }

                throw new ExpressionException(
                    message: ErrorResponseMessages.UnsupportedExpressionEvaluationType.ToLocalizedMessage(
                        expression,
                        expression.Type,
                        jtokenTypes.Select(type => type.ToString()).ConcatStrings(", ")),
                    additionalInfo: additionalInfo);
            }

            return expression;
        }

        /// <summary>
        /// Evaluates the language expressions recursive.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The additional Info of the root.</param>
        /// <param name="skipEvaluationPaths">The paths that shall be skipped in evaluation.</param>
        public static JToken EvaluateLanguageExpressionsRecursive(JToken root, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo = null, InsensitiveHashSet skipEvaluationPaths = null)
        {
            return ExpressionsEngine.EvaluateLanguageExpressionsInternal(root, evaluationContext, ignoreExceptions: false, additionalInfo: additionalInfo, skipEvaluationPaths: skipEvaluationPaths);
        }

        /// <summary>
        /// Evaluates the language expressions recursive as of particular JToken type.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="expectedType">The JToken type.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        public static JToken EvaluateLanguageExpressionsRecursiveAsType(JToken root, ExpressionEvaluationContext evaluationContext, JTokenType expectedType, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            if (root != null)
            {
                var value = ExpressionsEngine.EvaluateLanguageExpressionsRecursive(root, evaluationContext, additionalInfo);

                if (value == null || value.Type != expectedType)
                {
                    throw new ExpressionException(
                        message: ErrorResponseMessages.UnexpectedExpressionType.ToLocalizedMessage(root, value == null ? JTokenType.Null : value.Type, expectedType),
                        additionalInfo: additionalInfo);
                }

                return value;
            }

            return root;
        }

        /// <summary>
        /// Evaluates the language expressions optimistically.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The additional Info of the root.</param>
        /// <param name="skipEvaluationPaths">The paths that need to be skipped evaluation.</param>
        public static JToken EvaluateLanguageExpressionsOptimistically(JToken root, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo = null, InsensitiveHashSet skipEvaluationPaths = null)
        {
            return ExpressionsEngine.EvaluateLanguageExpressionsInternal(
                root: root,
                evaluationContext: evaluationContext,
                ignoreExceptions: true,
                additionalInfo: additionalInfo,
                skipEvaluationPaths: skipEvaluationPaths);
        }

        /// <summary>
        /// Evaluates the language expressions optimistically.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        public static void EvaluateLanguageExpressionsOptimistically(LanguageExpression expression, ExpressionEvaluationContext evaluationContext, TemplateErrorAdditionalInfo additionalInfo)
        {
            try
            {
                expression.EvaluateExpression(evaluationContext, additionalInfo);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal() ||
                    evaluationContext.CanEvaluateOptimistically == null ||
                    !evaluationContext.CanEvaluateOptimistically(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Selects the earliest token with value from a path.
        /// </summary>
        /// <param name="validateContent">The content to validate.</param>
        /// <param name="path">The path.</param>
        public static JToken[] SelectRootExistingJTokens(JToken validateContent, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return EmptyArray<JToken>.Instance;
            }

            var pathTokens = validateContent.SelectTokensInsensitively(path);
            if (pathTokens.CoalesceEnumerable().Any())
            {
                return pathTokens;
            }

            if (path.EndsWithInsensitively(JTokenExtensions.ArrayTokenSuffix))
            {
                return ExpressionsEngine.SelectRootExistingJTokens(validateContent, path.Remove(path.LastIndexOf(JTokenExtensions.ArrayTokenSuffix, StringComparison.InvariantCultureIgnoreCase)));
            }

            var segments = path.SplitRemoveEmpty(".");
            return ExpressionsEngine.SelectRootExistingJTokens(validateContent, segments.Take(segments.Length - 1).ConcatStrings("."));
        }

        /// <summary>
        /// Whether the expression contains the given function.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="functionName">the function.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        public static bool ExpressionHasFunction(string expression, string functionName, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            return ExpressionsEngine.IsLanguageExpression(expression) &&
                ExpressionsEngine
                    .ParseLanguageExpression(expression, additionalInfo)
                    .HasFunction(functionName);
        }

        /// <summary>
        /// Whether the expression contains the given function.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="functionName">the function.</param>
        /// <param name="additionalInfo">The line information of the function.</param>
        public static bool ExpressionHasFunction(JToken expression, string functionName, TemplateErrorAdditionalInfo additionalInfo = null)
        {
            return ExpressionsEngine
                .ParseLanguageExpressionsRecursive(expression, additionalInfo)
                .CoalesceDictionary().Values
                .Any(value => value.HasFunction(functionName));
        }

        /// <summary>
        /// Serializes the specified expression. This is intended for one-off usages of the serializer.
        /// </summary>
        /// <param name="expression">The expression to serialize</param>
        /// <param name="settings">The optional serializer settings</param>
        public static string SerializeExpression(LanguageExpression expression, ExpressionSerializerSettings settings = null) =>
            ExpressionsEngine
                .CreateExpressionSerializer(settings: settings)
                .SerializeExpression(expression: expression);

        /// <summary>
        /// Escapes all string values in a JToken.
        /// </summary>
        /// <param name="root">The root token.</param>
        /// <param name="settings">The serializer settings</param>
        /// <remarks>This function operates in-place on the token passed in, which may result in mutations.</remarks>
        public static JToken EscapeJTokenStringValues(JToken root, ExpressionSerializerSettings settings = null)
        {
            if (root == null)
            {
                return null;
            }

            var serializer = ExpressionsEngine.CreateExpressionSerializer(settings: settings);

            JTokenHelper.TransformJsonStringValues(
                root: root,
                transformFunc: (key, value) => serializer.SerializeExpression(new JTokenExpression(value)));

            return root;
        }

        /// <summary>
        /// Escapes a string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="settings">The serializer settings</param>
        public static string EscapeString(string value, ExpressionSerializerSettings settings)
        {
            if (value == null)
            {
                return null;
            }

            return SerializeExpression(new JTokenExpression(value), settings);
        }

        /// <summary>
        /// Creates the expression serializer. This is intended for situations when you need to reuse serializer settings when serialize multiple expressions.
        /// </summary>
        /// <param name="settings">The optional serializer settings</param>
        public static ExpressionSerializer CreateExpressionSerializer(ExpressionSerializerSettings settings = null) => new ExpressionSerializer(settings: settings);

        /// <summary>
        /// Checks if the specified string is a valid function name according to the expression parsing rules.
        /// </summary>
        /// <param name="value">The function name to validate.</param>
        /// <param name="additionalInfo">The additional Info of the value.</param>
        public static bool IsFunctionName(string value, TemplateErrorAdditionalInfo additionalInfo = null) => ExpressionParser.IsFunctionName(value: value, additionalInfo: additionalInfo);

        #region EvaluateLanguageExpressions

        /// <summary>
        /// Evaluates the language expressions recursive.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <param name="ignoreExceptions">The value indicating whether to ignore exceptions or not.</param>
        /// <param name="additionalInfo">The additional Info of the root.</param>
        /// <param name="skipEvaluationPaths">The paths that shall be skipped in evaluation.</param>
        private static JToken EvaluateLanguageExpressionsInternal(JToken root, ExpressionEvaluationContext evaluationContext, bool ignoreExceptions, TemplateErrorAdditionalInfo additionalInfo, InsensitiveHashSet skipEvaluationPaths = null)
        {
            foreach (var expressionKvp in ExpressionsEngine.ParseLanguageExpressionsRecursive(root, additionalInfo, skipEvaluationPaths))
            {
                try
                {
                    var expression = expressionKvp.Value;
                    var expressionNode = expressionKvp.Key;
                    var expressionValue = expression.EvaluateExpression(evaluationContext, additionalInfo);

                    if (expressionNode == root)
                    {
                        root = expressionValue;
                    }
                    else if (expressionNode is JProperty)
                    {
                        var property = expressionNode as JProperty;
                        var parent = property.Parent as JObject;

                        if (!expressionValue.IsTextBasedJTokenType())
                        {
                            throw new ExpressionException(
                                message: ErrorResponseMessages.InvalidPropertyExpressionType.ToLocalizedMessage(property.Name, expressionValue.Type),
                                additionalInfo: additionalInfo);
                        }

                        var propertyName = expressionValue.ToObject<string>();
                        if (parent.ContainsInsensitively(propertyName))
                        {
                            throw new ExpressionException(
                                message: ErrorResponseMessages.InvalidPropertyAlreadyExists.ToLocalizedMessage(property.Name, propertyName),
                                additionalInfo: additionalInfo);
                        }

                        property.RenameProperty(propertyName);
                    }
                    else
                    {
                        expressionNode.Replace(expressionValue);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.IsFatal() ||
                        !ignoreExceptions ||
                        evaluationContext.CanEvaluateOptimistically == null ||
                        !evaluationContext.CanEvaluateOptimistically(ex))
                    {
                        throw;
                    }
                }
            }

            return root;
        }

        #endregion

        #region JTokenReferenceComparer

        /// <summary>
        /// The JToken reference equality comparer.
        /// </summary>
        private class JTokenReferenceComparer : IEqualityComparer<JToken>
        {
            /// <summary>
            /// The singleton instance of JToken reference equality comparer.
            /// </summary>
            public static readonly JTokenReferenceComparer Instance = new JTokenReferenceComparer();

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first token.</param>
            /// <param name="y">The second token.</param>
            public bool Equals(JToken x, JToken y)
            {
                return object.ReferenceEquals(x, y);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The token.</param>
            public int GetHashCode(JToken obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        #endregion
    }
}
