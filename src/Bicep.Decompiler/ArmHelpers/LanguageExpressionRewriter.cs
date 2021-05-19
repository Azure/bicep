// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;

namespace Bicep.Decompiler.ArmHelpers
{
    public static class LanguageExpressionRewriter
    {
        public static LanguageExpression Rewrite(LanguageExpression expression, Func<LanguageExpression, LanguageExpression> rewriteFunc)
        {
            var (_, rewritten) = RewriteInternal(expression, rewriteFunc);

            return rewritten;
        }

        private static (bool hasChanges, LanguageExpression[] output) RewriteInternal(LanguageExpression[] input, Func<LanguageExpression, LanguageExpression> rewriteFunc)
        {
            var output = new LanguageExpression[input.Length];

            var hasChanges = false;
            for (var i = 0; i < input.Length; i++)
            {
                var (entryHasChanges, entryOutput) = RewriteInternal(input[i], rewriteFunc);

                hasChanges |= entryHasChanges;
                output[i] = entryOutput;
            }

            return (hasChanges, output);
        }

        private static (bool hasChanges, LanguageExpression output) RewriteInternal(LanguageExpression input, Func<LanguageExpression, LanguageExpression> rewriteFunc)
        {
            switch (input)
            {
                case FunctionExpression function:
                    {
                        var hasChanges = false;

                        var newParameters = function.Parameters;
                        if (function.Parameters is not null)
                        {
                            var (hasChangesInner, output) = RewriteInternal(function.Parameters, rewriteFunc);
                            hasChanges |= hasChangesInner;
                            newParameters = output;
                        }

                        var newProperties = function.Properties;
                        if (function.Properties is not null)
                        {
                            var (hasChangesInner, output) = RewriteInternal(function.Properties, rewriteFunc);
                            hasChanges |= hasChangesInner;
                            newProperties = output;
                        }

                        if (hasChanges)
                        {
                            function = new FunctionExpression(function.Function, newParameters, newProperties);
                        }

                        var rewritten = rewriteFunc(function);
                        hasChanges |= !object.ReferenceEquals(function, rewritten);

                        return (hasChanges, rewritten);
                    }
                case JTokenExpression jtoken:
                    {
                        var rewritten = rewriteFunc(jtoken);
                        var hasChanges = !object.ReferenceEquals(jtoken, rewritten);

                        return (hasChanges, rewritten);
                    }
                default:
                    throw new NotImplementedException($"Unrecognized expression type {input.GetType()}");
            }
        }
    }
}
