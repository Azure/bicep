// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Bicep.Cli.Helpers.Repl;

namespace Bicep.Cli.Services;

public class ReplEvaluator
{
    private readonly ExpressionConverter converter;
    private readonly SemanticModel semanticModel;

    private readonly Dictionary<string, JToken> variableValueCache = new(LanguageConstants.IdentifierComparer);
    private readonly HashSet<string> evaluationStack = new(LanguageConstants.IdentifierComparer);

    public ReplEvaluator(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        var emitterContext = new EmitterContext(semanticModel);
        converter = new ExpressionConverter(emitterContext);
    }

    public ReplEvaluationResult EvaluateExpression(SyntaxBase expressionSyntax)
    {
        try
        {
            var context = GetExpressionEvaluationContext();
            var intermediate = converter.ConvertToIntermediateExpression(expressionSyntax);
            var result = converter.ConvertExpression(intermediate).EvaluateExpression(context);
            return ReplEvaluationResult.For(result);
        }
        catch (Exception ex)
        {
            return ReplEvaluationResult.For([DiagnosticBuilder.ForPosition(expressionSyntax)
                .FailedToEvaluateSubject("expression", expressionSyntax.ToString(), ex.Message)]);
        }
    }

    private JToken EvaluateVariableInternal(string name)
    {
        // cycle detection
        if (evaluationStack.Contains(name))
        {
            throw new InvalidOperationException($"Variable cycle detected involving '{name}'");
        }
        if (variableValueCache.TryGetValue(name, out var cached))
        {
            return cached;
        }

        var declaration = semanticModel.Root.VariableDeclarations.FirstOrDefault(v => LanguageConstants.IdentifierComparer.Equals(v.Name, name));
        if (declaration is null)
        {
            throw new InvalidOperationException($"Variable '{name}' not found");
        }

        try
        {
            evaluationStack.Add(name);
            var valueSyntax = declaration.DeclaringVariable.Value;
            var intermediate = converter.ConvertToIntermediateExpression(valueSyntax);
            var context = GetExpressionEvaluationContext();
            var evaluated = converter.ConvertExpression(intermediate).EvaluateExpression(context);
            variableValueCache[name] = evaluated;
            return evaluated;
        }
        finally
        {
            evaluationStack.Remove(name);
        }
    }

    private ReplEvaluationContext GetExpressionEvaluationContext()
    {
        var helper = new TemplateExpressionEvaluationHelper
        {
            OnGetVariable = (variableName, _) => EvaluateVariableInternal(variableName)
        };

        return new(helper, this);
    }
    
    private class ReplEvaluationContext : IEvaluationContext
    {
        private readonly TemplateExpressionEvaluationHelper evaluationHelper;
        private readonly ReplEvaluator evaluator;

        public ReplEvaluationContext(TemplateExpressionEvaluationHelper evaluationHelper, ReplEvaluator evaluator)
            : this(evaluationHelper, evaluator, evaluationHelper.EvaluationContext.Scope)
        {
        }

        private ReplEvaluationContext(TemplateExpressionEvaluationHelper evaluationHelper, ReplEvaluator evaluator, ExpressionScope scope)
        {
            this.evaluationHelper = evaluationHelper;
            this.evaluator = evaluator;
            this.Scope = scope;
        }

        public bool IsShortCircuitAllowed => evaluationHelper.EvaluationContext.IsShortCircuitAllowed;

        public ExpressionScope Scope { get; }

        public bool AllowInvalidProperty(Exception exception, FunctionExpression functionExpression, FunctionArgument[] functionParametersValues, JToken[] selectedProperties) =>
            evaluationHelper.EvaluationContext.AllowInvalidProperty(exception, functionExpression, functionParametersValues, selectedProperties);

        public JToken EvaluateFunction(FunctionExpression functionExpression, FunctionArgument[] parameters, IEvaluationContext context, TemplateErrorAdditionalInfo? additionalnfo)
            => evaluationHelper.EvaluationContext.EvaluateFunction(functionExpression, parameters, this, additionalnfo);

        public bool ShouldIgnoreExceptionDuringEvaluation(Exception exception) =>
            this.evaluationHelper.EvaluationContext.ShouldIgnoreExceptionDuringEvaluation(exception);

        public IEvaluationContext WithNewScope(ExpressionScope scope) => new ReplEvaluationContext(this.evaluationHelper, this.evaluator, scope);
    }
}

public class ReplEvaluationResult
{
    private ReplEvaluationResult(JToken? value, IEnumerable<IDiagnostic>? diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics?.ToList() ?? [];
    }

    public JToken? Value { get; }
    public IReadOnlyList<IDiagnostic> Diagnostics { get; }

    public static ReplEvaluationResult For(JToken value) => new(value, null);
    public static ReplEvaluationResult For(IEnumerable<IDiagnostic> diagnostics) => new(null, diagnostics);
}