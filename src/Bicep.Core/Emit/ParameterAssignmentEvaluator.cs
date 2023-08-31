// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Emit
{
    public class ParameterAssignmentEvaluator
    {
        public class Result
        {
            private Result(JToken? value, ParameterKeyVaultReferenceExpression? keyVaultReference, IDiagnostic? diagnostic)
            {
                Value = value;
                KeyVaultReference = keyVaultReference;
                Diagnostic = diagnostic;
            }

            public JToken? Value { get; }
            public ParameterKeyVaultReferenceExpression? KeyVaultReference { get; }
            public IDiagnostic? Diagnostic { get; }

            public static Result For(JToken value) => new(value, null, null);

            public static Result For(ParameterKeyVaultReferenceExpression expression) => new(null, expression, null);

            public static Result For(IDiagnostic diagnostic) => new(null, null, diagnostic);
        }

        private readonly ConcurrentDictionary<ParameterAssignmentSymbol, Result> Results = new();
        private readonly ConcurrentDictionary<VariableSymbol, Result> VarResults = new();
        private readonly SemanticModel model;
        private readonly ImmutableDictionary<string, ParameterAssignmentSymbol> paramsByName;
        private readonly ImmutableDictionary<string, VariableSymbol> variablesByName;

        public ParameterAssignmentEvaluator(SemanticModel model)
        {
            this.model = model;
            this.paramsByName = model.Root.ParameterAssignments
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
            this.variablesByName = model.Root.VariableDeclarations
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        public Result EvaluateParameter(ParameterAssignmentSymbol parameter)
            => Results.GetOrAdd(
                parameter,
                parameter =>
                {
                    var context = GetExpressionEvaluationContext();
                    var converter = new ExpressionConverter(new(model));

                    var intermediate = converter.ConvertToIntermediateExpression(parameter.DeclaringParameterAssignment.Value);

                    if (intermediate is ParameterKeyVaultReferenceExpression keyVaultReferenceExpression)
                    {
                        return Result.For(keyVaultReferenceExpression);
                    }

                    try
                    {
                        return Result.For(converter.ConvertExpression(intermediate).EvaluateExpression(context));
                    }
                    catch (Exception ex)
                    {
                        return Result.For(DiagnosticBuilder.ForPosition(parameter.DeclaringParameterAssignment.Value)
                            .FailedToEvaluateParameter(parameter.Name, ex.Message));
                    }
                });

        public Result EvaluateVariable(VariableSymbol variable)
            => VarResults.GetOrAdd(
                variable,
                variable =>
                {
                    try
                    {
                        var context = GetExpressionEvaluationContext();
                        var converter = new ExpressionConverter(new(model));
                        var expression = converter.ConvertExpression(variable.DeclaringVariable.Value);

                        return Result.For(expression.EvaluateExpression(context));
                    }
                    catch (Exception ex)
                    {
                        return Result.For(DiagnosticBuilder.ForPosition(variable.DeclaringVariable.Value)
                            .FailedToEvaluateVariable(variable.Name, ex.Message));
                    }
                });

        private ExpressionEvaluationContext GetExpressionEvaluationContext()
        {
            var helper = new TemplateExpressionEvaluationHelper();
            helper.OnGetVariable = (name, _) =>
            {
                return EvaluateVariable(variablesByName[name]).Value ?? throw new InvalidOperationException($"Variable {name} has an invalid value");
            };
            helper.OnGetParameter = (name, _) =>
            {
                return EvaluateParameter(paramsByName[name]).Value ?? throw new InvalidOperationException($"Parameter {name} has an invalid value");
            };

            return helper.EvaluationContext;
        }
    }
}
