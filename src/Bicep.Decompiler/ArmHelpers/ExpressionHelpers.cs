// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Core.Utilities;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Decompiler.ArmHelpers
{

    public static class ExpressionHelpers
    {
        public static LanguageExpression ParseExpression(string value)
        {
            if (ExpressionsEngine.IsLanguageExpression(value))
            {
                return ExpressionsEngine.ParseLanguageExpression(value);
            }

            return new JTokenExpression(value);
        }

        public static FunctionExpression Concat(params LanguageExpression[] expressions)
            => new FunctionExpression("concat", expressions, new LanguageExpression[0]);

        private static IEnumerable<LanguageExpression> CombineConcatArguments(IEnumerable<LanguageExpression> arguments)
        {
            var current = "";
            foreach (var argument in arguments)
            {
                var stringVal = (argument as JTokenExpression)?.Value.Value<string>();
                if (stringVal != null)
                {
                    current += stringVal;

                    // don't return the string yet - there may be another string to join
                    continue;
                }

                if (current.Length > 0)
                {
                    yield return new JTokenExpression(current);
                    current = "";
                }

                yield return argument;
            }

            if (current.Length > 0)
            {
                yield return new JTokenExpression(current);
            }
        }

        public static LanguageExpression FlattenStringOperations(LanguageExpression original)
        {
            if (original is not FunctionExpression functionExpression)
            {
                return original;
            }

            // convert a 'format' to a 'concat' function
            if (functionExpression.NameEquals("format"))
            {
                var formatString = (functionExpression.Parameters[0] as JTokenExpression)?.Value.Value<string>() ?? throw new ArgumentException($"Unable to read format statement {ExpressionsEngine.SerializeExpression(functionExpression)} as string");
                var formatHoleMatches = Regex.Matches(formatString, "{([0-9]+)}");

                var concatExpressions = new List<LanguageExpression>();
                var nextStart = 0;
                for (var i = 0; i < formatHoleMatches.Count; i++)
                {
                    var match = formatHoleMatches[i];
                    var position = match.Groups[1].Index;
                    var length = match.Groups[1].Length;
                    var intValue = int.Parse(match.Groups[1].Value);

                    // compensate for the {
                    if (nextStart < position - 1)
                    {
                        var betweenPortion = formatString.Substring(nextStart, position - 1 - nextStart);
                        concatExpressions.Add(new JTokenExpression(betweenPortion));
                    }

                    // replace it with the appropriately-numbered expression
                    concatExpressions.Add(functionExpression.Parameters[intValue + 1]);

                    // compensate for the }
                    nextStart = position + length + 1;
                }

                if (nextStart < formatString.Length)
                {
                    var betweenPortion = formatString.Substring(nextStart, formatString.Length - nextStart);
                    concatExpressions.Add(new JTokenExpression(betweenPortion));
                }

                // overwrite the original expression
                functionExpression = Concat(concatExpressions.ToArray());
            }

            // flatten nested 'concat' functions
            if (functionExpression.NameEquals("concat"))
            {
                var concatExpressions = new List<LanguageExpression>();
                foreach (var parameter in functionExpression.Parameters)
                {
                    // recurse
                    var flattenedParameter = FlattenStringOperations(parameter);
                    
                    if (flattenedParameter is FunctionExpression childFunction && childFunction.NameEquals("concat"))
                    {
                        // concat directly inside a concat - break it out
                        concatExpressions.AddRange(childFunction.Parameters);
                        continue;
                    }

                    concatExpressions.Add(flattenedParameter);
                }

                // overwrite the original expression
                functionExpression = Concat(CombineConcatArguments(concatExpressions).ToArray());
            }

            // just return the inner portion if there's only one concat entry
            if (functionExpression.NameEquals("concat") && functionExpression.Parameters.Length == 1)
            {
                return functionExpression.Parameters[0];
            }

            return functionExpression;
        }

        private static LanguageExpression FormatNameExpression(IEnumerable<LanguageExpression> nameSegments)
        {
            var pathSegments = nameSegments
                .Select(FlattenStringOperations)
                .SelectMany((expression, i) => i == 0 ? new [] { expression } : new [] { new JTokenExpression("/"), expression })
                .ToArray();

            return pathSegments.Length > 1 ? Concat(pathSegments) : pathSegments.First();
        }

        public static (string typeString, LanguageExpression nameExpression)? TryGetResourceNormalizedForm(LanguageExpression expression)
        {
            expression = FlattenStringOperations(expression);

            if (expression is not FunctionExpression functionExpression)
            {
                // could be a string literal (e.g. "My.RP/typeA/nameA"). May as well try and parse it with the below logic.
                functionExpression = Concat(expression);
            }

            if (functionExpression.NameEquals("concat"))
            {
                // this is a heuristic but appears to be quite common - e.g. to format a resourceId using concat('My.Rp/type', parameters(name)) rather than resourceId().
                // It doesn't really hurt to see if we can find a match for it.

                var segments = functionExpression.Parameters
                    .SelectMany(x => x is JTokenExpression jTokenExpression ? jTokenExpression.Value.ToString().Split('/').Where(y => y != "").Select(y => new JTokenExpression(y)) : x.AsEnumerable())
                    .ToArray();

                if (segments.Length < 3 || segments.Length % 2 != 1)
                {
                    return null;
                }

                var types = segments.Take(1).Concat(segments.Skip(1).Where((x, i) => i % 2 == 0));
                var names = segments.Skip(1).Where((x, i) => i % 2 == 1);

                var typeBuilder = new StringBuilder();
                foreach (var type in types)
                {
                    if (typeBuilder.Length > 0)
                    {
                        typeBuilder.Append("/");
                    }

                    if (type is not JTokenExpression jTokenType)
                    {
                        return null;
                    }

                    typeBuilder.Append(jTokenType.Value.Value<string>());
                }

                var typeParam = new JTokenExpression(typeBuilder.ToString());

                functionExpression = new FunctionExpression(
                    "resourceId",
                    typeParam.AsEnumerable().Concat(names).ToArray(),
                    new LanguageExpression[] { });
            }

            if (!functionExpression.NameEquals("resourceId") ||
                functionExpression.Parameters.Length < 2)
            {
                return null;
            }

            var typeString = (functionExpression.Parameters[0] as JTokenExpression)?.Value.Value<string>();
            if (typeString == null)
            {
                return null;
            }

            var nameExpression = FormatNameExpression(functionExpression.Parameters.Skip(1));

            return (typeString, nameExpression);
        }

        public static string? TryGetLocalFilePathForTemplateLink(LanguageExpression templateLinkExpression)
        {
            LanguageExpression? relativePath = null;
            if (templateLinkExpression is FunctionExpression uriExpression && uriExpression.Function == "uri")
            {
                // it's common to format references to files using the uri function. the second param is the relative path (which should match the file system path)
                relativePath = uriExpression.Parameters[1];
            }
            else if (templateLinkExpression is FunctionExpression concatExpression && concatExpression.Function == "concat")
            {
                if (concatExpression.Parameters[0] is FunctionExpression concatUriExpression && concatUriExpression.Function == "uri")
                {
                    // or sometimes the other way around - uri expression inside a concat
                    relativePath = concatUriExpression.Parameters[1];
                }
                else if (concatExpression.Parameters[0] is FunctionExpression concatParametersExpression && concatParametersExpression.Function == "parameters" && concatExpression.Parameters.Length == 2)
                {
                    // URI prefix in a parameter
                    relativePath = concatExpression.Parameters[1];
                }
            }
            else if (templateLinkExpression is JTokenExpression templateLinkJtoken)
            {
                relativePath = templateLinkJtoken;
            }

            if (relativePath is not JTokenExpression jTokenExpression)
            {
                // return the original expression so that the author can fix it up rather than failing
                return null;
            }
            
            var output = jTokenExpression.Value.ToString();
            if (output.IndexOf("./") == 0)
            {
                output = output.Substring(2);
            }

            return output.Trim('/');
        }

        public static string? TryGetStringValue(LanguageExpression expression)
        {
            if (expression is not JTokenExpression jTokenExpression)
            {
                return null;
            }

            if (jTokenExpression.Value.Type != JTokenType.String)
            {
                return null;
            }

            return jTokenExpression.Value.ToString();
        }

        public static FunctionExpression? TryGetNamedFunction(LanguageExpression expression, string name)
        {
            if (expression is not FunctionExpression function)
            {
                return null;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals(function.Function, name))
            {
                return null;
            }

            return function;
        }
    }
}