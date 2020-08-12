//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Extensions
{
    using Azure.ResourceManager.Deployments.Core.Collections;
    using Azure.ResourceManager.Deployments.Core.Extensions;
    using Azure.ResourceManager.Deployments.Expression.Expressions;
    using System.Linq;

    /// <summary>
    /// Extension methods for language expressions
    /// </summary>
    public static class LanguageExpressionExtensions
    {
        /// <summary>
        /// Whether the expression has the given function.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="functionName">The function name.</param>
        public static bool HasFunction(this LanguageExpression expression, string functionName)
        {
            if (expression is FunctionExpression)
            {
                var found = false;
                var visitor = new LanguageExpressionVisitor()
                {
                    OnFunctionExpression = functionExpression => found |= functionExpression.Function.EqualsOrdinalInsensitively(functionName),
                    ShortCircuitVisitor = () => found
                };

                expression.Accept(visitor);
                return found;
            }

            return false;
        }

        /// <summary>
        /// Whether the expression has any of the given functions.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="functionNames">The function names.</param>
        public static bool HasAnyFunction(this LanguageExpression expression, params string[] functionNames)
        {
            if (expression is FunctionExpression && functionNames != null && functionNames.Any())
            {
                var found = false;
                var visitor = new LanguageExpressionVisitor()
                {
                    OnFunctionExpression = functionExpression => found |= functionNames.ContainsOrdinalInsensitively(functionExpression.Function),
                    ShortCircuitVisitor = () => found
                };

                expression.Accept(visitor);
                return found;
            }

            return false;
        }

        /// <summary>
        /// Get the functions used in the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public static string[] GetFunctions(this LanguageExpression expression)
        {
            var functions = new OrdinalInsensitiveHashSet();
            if (expression is FunctionExpression)
            {
                var visitor = new LanguageExpressionVisitor()
                {
                    OnFunctionExpression = functionExpression => functions.Add(functionExpression.Function)
                };

                expression.Accept(visitor);
                return functions.ToArray();
            }

            return EmptyArray<string>.Instance;
        }
    }
}
