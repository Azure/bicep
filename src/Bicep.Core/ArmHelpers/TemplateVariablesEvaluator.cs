// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Parsing;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.ArmHelpers;

/// <summary>
/// Allows the evaluation of ARM template variables that do not refer to parameter values.
/// </summary>
internal class TemplateVariablesEvaluator
{
    private const string CopyPropertyName = "copy";
    private const string CopyNamePropertyName = "name";
    private const string CopyCountPropertyName = "count";
    private const string CopyItemValuePropertyName = "input";
    private const string CopyIndexFunctionName = "copyIndex";

    private interface IEvaluationResult {}
    private record EvaluatedValue(JToken Value) : IEvaluationResult {}
    private record EvaluationException(Exception Exception) : IEvaluationResult {}

    private readonly ConcurrentDictionary<string, IEvaluationResult> evaluatedVariables = new();
    private readonly Stack<string> variableEvaluationStack = new();
    private readonly Template template;
    private readonly ExpressionEvaluationContext evaluationContext;

    internal TemplateVariablesEvaluator(Template template)
    {
        this.template = template;

        TemplateExpressionEvaluationHelper helper = new()
        {
            OnGetParameter = OnGetParameter,
            OnGetVariable = OnGetVariable,
        };
        evaluationContext = helper.EvaluationContext;
    }

    internal JToken? TryGetEvaluatedVariableValue(string variableName) => evaluatedVariables.GetOrAdd(variableName, GetOrAddVariableResult) switch
    {
        EvaluatedValue evaluated => evaluated.Value,
        _ => null,
    };

    internal JToken GetEvaluatedVariableValue(string variableName) => evaluatedVariables.GetOrAdd(variableName, GetOrAddVariableResult) switch
    {
        EvaluatedValue evaluated => evaluated.Value,
        EvaluationException e => throw e.Exception,
        _ => throw new UnreachableException(),
    };

    internal JToken Evaluate(LanguageExpression languageExpression) => languageExpression.EvaluateExpression(evaluationContext);

    internal JToken? TryEvaluate(LanguageExpression languageExpression)
    {
        try
        {
            return Evaluate(languageExpression);
        }
        catch
        {
            return null;
        }
    }

    internal JToken? TryGetUnevaluatedDeclaringToken(string name)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(name, CopyPropertyName))
        {
            // The {CopyPropertyName} variable is a template metaprogramming directive, not a variable.
            return null;
        }

        if (template.Variables?.TryGetValue(name, out var assignment) is true)
        {
            return assignment.Value;
        }

        return null;
    }

    internal (JToken CountToken, JToken ValueItemToken)? TryGetUnevaluatedCopyDeclaration(string name)
    {
        if (TryGetRawUnevaluatedCopyDeclaration(name) is JObject copiedVariable &&
            copiedVariable.TryGetValue(CopyCountPropertyName, StringComparison.OrdinalIgnoreCase, out var copyCountToken) &&
            copiedVariable.TryGetValue(CopyItemValuePropertyName, StringComparison.OrdinalIgnoreCase, out var copyItemToken))
        {
            return (copyCountToken, copyItemToken);
        }


        return null;
    }

    private static JToken OnGetParameter(string parameterName, TemplateErrorAdditionalInfo? additionalInfo)
        => throw new InvalidOperationException($"The {parameterName} parameter was requested.");

    private JToken OnGetVariable(string variableName, TemplateErrorAdditionalInfo? additionalInfo) => GetEvaluatedVariableValue(variableName);

    private IEvaluationResult GetOrAddVariableResult(string name)
    {
        if (variableEvaluationStack.Any(underEvaluation => StringComparer.OrdinalIgnoreCase.Equals(name, underEvaluation)))
        {
            return new EvaluationException(new InvalidOperationException($"Variable cycle detected: {string.Join(" -> ", variableEvaluationStack.Reverse().Append(name))}"));
        }

        try
        {
            return new EvaluatedValue(EvaluateVariableDeclaration(name, GetUnevaluatedDeclaringToken(name)));
        }
        catch (Exception e)
        {
            return new EvaluationException(e);
        }
    }

    private JToken GetUnevaluatedDeclaringToken(string name)
    {
        if (TryGetUnevaluatedDeclaringToken(name) is JToken singularVariable)
        {
            return singularVariable;
        }

        if (TryGetRawUnevaluatedCopyDeclaration(name) is JObject copiedVariable)
        {
            if (!copiedVariable.TryGetValue(CopyCountPropertyName, StringComparison.OrdinalIgnoreCase, out var copyCountToken))
            {
                throw new InvalidOperationException($"The '{name}' variable did not declare a copy count.");
            }

            if (!copiedVariable.TryGetValue(CopyItemValuePropertyName, StringComparison.OrdinalIgnoreCase, out var copyItemToken))
            {
                throw new InvalidOperationException($"The '{name}' variable did not declare a copy input.");
            }

            var copyCount = ExpressionsEngine.EvaluateLanguageExpressionAsInteger(copyCountToken, evaluationContext, null).ToObject<int>();
            JArray target = new();
            for (int i = 0; i < copyCount; i++)
            {
                var cloned = copyItemToken.DeepClone();
                foreach (var kvp in ExpressionsEngine.ParseLanguageExpressionsRecursive(cloned))
                {
                    if (kvp.Value is FunctionExpression functionExpression)
                    {
                        JToken? replacement = functionExpression switch
                        {
                            _ when IsCopyIndexFunction(functionExpression)
                                => EvaluateCopyIndexFunction(name, i, functionExpression.Parameters[0], functionExpression.Parameters.Skip(1).FirstOrDefault()),
                            _ when ReplaceCopyIndexFunctionExpressionsInPlace(functionExpression, name, i)
                                => $"[{SerializeExpression(functionExpression)}]",
                            _ => null,
                        };

                        if (replacement is not null)
                        {
                            if (kvp.Key.Parent is not null)
                            {
                                kvp.Key.Replace(replacement);
                            }
                            else
                            {
                                // if the token that was parsed into the replaced expression has no parent, then we must be replacing the root
                                cloned = replacement;
                                break;
                            }
                        }
                    }
                }

                target.Add(cloned);
            }

            return target;
        }

        throw new InvalidOperationException($"Variable '{name}' was not found.");
    }

    private JObject? TryGetRawUnevaluatedCopyDeclaration(string name)
    {
        if (template.Variables?.TryGetValue(CopyPropertyName, out var copyVariable) is true &&
            copyVariable.Value is JArray copiedVariables &&
            copiedVariables.OfType<JObject>()
                .Where(copy => copy.TryGetValue(CopyNamePropertyName, StringComparison.OrdinalIgnoreCase, out var copyNameToken) &&
                    Evaluate(copyNameToken) is JValue { Value: string copyName } &&
                    StringComparer.OrdinalIgnoreCase.Equals(name, copyName))
                .FirstOrDefault() is JObject copiedVariable)
        {
            return copiedVariable;
        }

        return null;
    }

    private JToken EvaluateVariableDeclaration(string variableName, JToken unevaluatedDeclaringToken)
    {
        variableEvaluationStack.Push(variableName);
        try
        {
            return Evaluate(unevaluatedDeclaringToken);
        }
        finally
        {
            variableEvaluationStack.Pop();
        }
    }

    private JToken Evaluate(JToken toEvaluate)
        // ExpressionsEngine.EvaluateLanguageExpressionsRecursive will modify the provided JToken in place by replacing expressions with the evaluated value thereof, so clone the provided token
        => ExpressionsEngine.EvaluateLanguageExpressionsRecursive(toEvaluate.DeepClone(), evaluationContext);

    private int EvaluateCopyIndexFunction(string expectedName, int loopIteration, LanguageExpression nameArgument, LanguageExpression? startIndexArgument)
    {
        if (nameArgument.EvaluateExpression(evaluationContext) is not JValue { Value: string copyIndexName } || !StringComparer.OrdinalIgnoreCase.Equals(expectedName, copyIndexName))
        {
            throw new InvalidOperationException("Invalid copy index encountered.");
        }

        return startIndexArgument?.EvaluateExpression(evaluationContext) switch
        {
            null => loopIteration,
            JToken startIndexToken when startIndexToken.Type == JTokenType.Integer => checked(loopIteration + startIndexToken.ToObject<int>()),
            _ => throw new InvalidOperationException("Invalid start index encountered."),
        };
    }

    private bool ReplaceCopyIndexFunctionExpressionsInPlace(FunctionExpression toModify, string expectedCopyIndexName, int loopIteration)
    {
        var hasChanges = false;

        if (toModify.Parameters is not null)
        {
            for (int i = 0; i < toModify.Parameters.Length; i++)
            {
                if (toModify.Parameters[i] is FunctionExpression parameterFunc)
                {
                    if (IsCopyIndexFunction(parameterFunc))
                    {
                        toModify.Parameters[i] = new JTokenExpression(EvaluateCopyIndexFunction(expectedCopyIndexName,
                            loopIteration,
                            parameterFunc.Parameters[0],
                            parameterFunc.Parameters.Skip(1).FirstOrDefault()));
                        hasChanges = true;
                    }
                    else if (ReplaceCopyIndexFunctionExpressionsInPlace(parameterFunc, expectedCopyIndexName, loopIteration))
                    {
                        hasChanges = true;
                    }
                }
            }
        }

        if (toModify.Properties is not null)
        {
            for (int i = 0; i < toModify.Properties.Length; i++)
            {
                if (toModify.Properties[i] is FunctionExpression propertyFunc)
                {
                    if (IsCopyIndexFunction(propertyFunc))
                    {
                        toModify.Properties[i] = new JTokenExpression(EvaluateCopyIndexFunction(expectedCopyIndexName,
                            loopIteration,
                            propertyFunc.Parameters[0],
                            propertyFunc.Parameters.Skip(1).FirstOrDefault()));
                        hasChanges = true;
                    }
                    else if (ReplaceCopyIndexFunctionExpressionsInPlace(propertyFunc, expectedCopyIndexName, loopIteration))
                    {
                        hasChanges = true;
                    }
                }
            }
        }

        return hasChanges;
    }

    private static bool IsCopyIndexFunction(FunctionExpression expression) => StringComparer.OrdinalIgnoreCase.Equals(expression.Function, CopyIndexFunctionName);

    private static string SerializeExpression(LanguageExpression expression) => expression switch
    {
        JTokenExpression { Value: JValue { Value: string strVal } } => $"'{strVal.Replace("'", "''")}'",
        JTokenExpression otherwise => otherwise.Value.ToString(),
        FunctionExpression functionExpression => SerializeExpression(functionExpression),
        _ => throw new InvalidOperationException($"Unrecognized ARM LanguageExpression of type {expression.GetType().Name} encountered."),
    };

    private static string SerializeExpression(FunctionExpression functionExpression)
    {
        StringBuilder builder = new(functionExpression.Function);
        builder.Append('(');
        var isFirstArgument = true;
        foreach (var argument in functionExpression.Parameters)
        {
            if (!isFirstArgument)
            {
                builder.Append(", ");
            }
            else
            {
                isFirstArgument = false;
            }

            builder.Append(SerializeExpression(argument));
        }
        builder.Append(')');

        foreach (var property in functionExpression.Properties)
        {
            if (property is JTokenExpression { Value: JValue { Value: string strVal } } && Lexer.IsValidIdentifier(strVal))
            {
                builder.Append('.').Append(strVal);
            }
            else
            {
                builder.Append('[').Append(SerializeExpression(property)).Append(']');
            }
        }

        return builder.ToString();
    }
}
