namespace Azure.ResourceManager.Deployments.Expression.Helpers
{
    using Azure.ResourceManager.Deployments.Core.Collections;
    using Azure.ResourceManager.Deployments.Core.Extensions;
    using Azure.ResourceManager.Deployments.Expression.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class for <see cref="LanguageExpression"/>.
    /// </summary>
    public static class LanguageExpressionHelper
    {
        /// <summary>
        /// Creates the parameters expression.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        public static LanguageExpression CreateParametersExpression(string parameterName)
        {
            return new FunctionExpression("parameters", CreateStringExpression(parameterName).AsArray(), EmptyArray<LanguageExpression>.Instance);
        }

        /// <summary>
        /// Creates the resource identifier expression.
        /// </summary>
        /// <param name="fullyQualifiedResourceType">The fully qualified resource type.</param>
        /// <param name="nameSegmentExpressions">The name segment expressions.</param>
        public static LanguageExpression CreateResourceIdExpression(string fullyQualifiedResourceType, IEnumerable<LanguageExpression> nameSegmentExpressions)
        {
            return new FunctionExpression("resourceId", CreateStringExpression(fullyQualifiedResourceType).AsArray().ConcatArray(nameSegmentExpressions), EmptyArray<LanguageExpression>.Instance);
        }

        /// <summary>
        /// Creates the string expression.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        public static LanguageExpression CreateStringExpression(string stringValue)
        {
            return new JTokenExpression(stringValue);
        }

        /// <summary>
        /// Combines the expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <exception cref="ArgumentOutOfRangeException">expressions - Expressions must have count > 0</exception>
        public static LanguageExpression CombineExpressions(IEnumerable<LanguageExpression> expressions)
        {
            var expressionsArray = expressions.ToArray();

            switch (expressionsArray.Length)
            {
                case 0:
                    throw new ArgumentOutOfRangeException(nameof(expressions), "Expressions must have count > 0");
                case 1:
                    return expressionsArray[0];
                default:
                    return new FunctionExpression("concat", expressionsArray, EmptyArray<LanguageExpression>.Instance);
            }
        }

        /// <summary>
        /// Combines the expressions.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        public static LanguageExpression CombineExpressions(params LanguageExpression[] expressions)
            => CombineExpressions(expressions as IEnumerable<LanguageExpression>);
    }
}
