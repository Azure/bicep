// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
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
            if (!(original is FunctionExpression functionExpression))
            {
                return original;
            }

            // convert a 'format' to a 'concat' function
            if (functionExpression.NameEquals("format"))
            {
                var formatString = (functionExpression.Parameters[0] as JTokenExpression)?.Value.Value<string>() ?? throw new ArgumentException($"Unable to read format statement {ExpressionsEngine.SerializeExpression(functionExpression)} as string");
                var formatHoleMatches = Regex.Matches(formatString, "(?<={){([0-9]+)}");

                var concatExpressions = new List<LanguageExpression>();
                var nextStart = 0;
                for (var i = 0; i < formatHoleMatches.Count; i++)
                {
                    var match = formatHoleMatches[i];
                    var position = match.Groups[2].Index;
                    var length = match.Groups[2].Length;
                    var intValue = int.Parse(match.Groups[2].Value);

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
                    
                    if (parameter is FunctionExpression childFunction && childFunction.NameEquals("concat"))
                    {
                        // concat directly inside a concat - break it out
                        concatExpressions.AddRange(childFunction.Parameters);
                        continue;
                    }

                    concatExpressions.Add(parameter);
                }

                // overwrite the original expression
                functionExpression = Concat(concatExpressions.ToArray());
            }

            // just return the inner portion if there's only one concat entry
            if (functionExpression.NameEquals("concat") && functionExpression.Parameters.Length == 1)
            {
                return functionExpression.Parameters[0];
            }

            return functionExpression;
        }
    }
}