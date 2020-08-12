//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Extensions
{
    using Azure.ResourceManager.Deployments.Expression.Engines;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The expression extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Wraps the input as expression literal.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public static string AsExpressionWithoutEscaping(string value)
        {
            if (ExpressionsEngine.IsLanguageExpression(value))
            {
                return value.Substring(1, value.Length - 2);
            }
            else
            {
                return string.Format("'{0}'", value);
            }
        }

        /// <summary>
        /// To expression string in JSON.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        public static string ToExpressionStringWithoutEscaping(string expression)
        {
            return string.Format("[{0}]", expression);
        }

        /// <summary>
        /// Determines whether the JToken is JTokenType String or Uri.
        /// </summary>
        /// <param name="token">The JToken object.</param>
        public static bool IsTextBasedJTokenType(this JToken token)
        {
            return token.Type == JTokenType.String || token.Type == JTokenType.Uri;
        }

        /// <summary>
        /// Determines whether the JToken is Integer or Float.
        /// </summary>
        /// <param name="token">The JToken object.</param>
        public static bool IsNumericJTokenType(this JToken token)
        {
            return token.Type == JTokenType.Integer || token.Type == JTokenType.Float;
        }

        /// <summary>
        /// Counts numbers of unique elements in a collection and returns a dictionary representing the counting result.
        /// </summary>
        /// <typeparam name="TElement">Type of elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public static IDictionary<TElement, int> CountElements<TElement>(this IEnumerable<TElement> collection, IEqualityComparer<TElement> equalityComparer)
        {
            return collection
                .GroupBy(element => element, equalityComparer)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        /// <summary>
        /// Converts a string into its expression representation. (Single quotes are replaced with double single quotes.)
        /// </summary>
        /// <param name="value">The value to escape</param>
        public static string EscapeStringLiteral(string value) => value.Replace(oldValue: "'", newValue: "''");

        /// <summary>
        /// Converts a string using expression representation to its unescaped form. (Double single quotes are replaced with single quotes.)
        /// </summary>
        /// <param name="value">The value to unescape</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Unescape' is used throughout .net APIs that deal with the concept.")]
        public static string UnescapeStringLiteral(string value) => value.Replace(oldValue: "''", newValue: "'");
    }
}
