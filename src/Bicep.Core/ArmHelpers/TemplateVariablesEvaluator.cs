// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.ArmHelpers;

/// <summary>
/// Allows the evaluation of ARM template variables that do not refer to parameter values.
/// </summary>
internal class TemplateVariablesEvaluator
{
    internal interface IEvaluationResult {}
    internal record EvaluatedValue(JToken Value) : IEvaluationResult {}
    internal record EvaluationException(Exception Exception) : IEvaluationResult {}

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

    internal IEvaluationResult this[string variableName] => evaluatedVariables.GetOrAdd(variableName, GetOrAddVariableResult);

    internal IEvaluationResult Evaluate(LanguageExpression languageExpression)
        => Evaluate(() => languageExpression.EvaluateExpression(evaluationContext));

    private static JToken OnGetParameter(string parameterName, TemplateErrorAdditionalInfo? additionalInfo)
        => throw new InvalidOperationException($"The {parameterName} parameter was requested.");

    private JToken OnGetVariable(string variableName, TemplateErrorAdditionalInfo? additionalInfo) => evaluatedVariables.GetOrAdd(variableName, GetOrAddVariableResult) switch
    {
        EvaluatedValue evaluatedValue => evaluatedValue.Value,
        EvaluationException exception => throw exception.Exception,
        _ => throw new InvalidOperationException("This switch was already exhaustive."),
    };

    private IEvaluationResult GetOrAddVariableResult(string name)
    {
        if (variableEvaluationStack.Any(underEvaluation => StringComparer.OrdinalIgnoreCase.Equals(name, underEvaluation)))
        {
            return new EvaluationException(new InvalidOperationException($"Variable cycle detected: {string.Join(" -> ", variableEvaluationStack.Reverse().Append(name))}"));
        }

        if (template.Variables?.TryGetValue(name, out var assignment) is true)
        {
            variableEvaluationStack.Push(name);
            var evaluated = Evaluate(assignment.Value);
            variableEvaluationStack.Pop();

            return evaluated;
        }

        return new EvaluationException(new InvalidOperationException($"Variable '{name}' was not found."));
    }

    private IEvaluationResult Evaluate(JToken toEvaluate)
        // ExpressionsEngine.EvaluateLanguageExpressionsRecursive will modify the provided JToken in place by replacing expressions with the evaluated value thereof, so clone the provided token
        => Evaluate(() => ExpressionsEngine.EvaluateLanguageExpressionsRecursive(toEvaluate.DeepClone(), evaluationContext));

    private static IEvaluationResult Evaluate(Func<JToken> evaluationFunc)
    {
        try
        {
            return new EvaluatedValue(evaluationFunc());
        }
        catch (Exception e)
        {
            return new EvaluationException(e);
        }
    }
}
