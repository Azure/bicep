// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Azure.Deployments.Templates.Expressions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace Bicep.Core.Emit
{
    public class ParameterAssignmentEvaluator
    {
        public record Result(
            JToken? Value,
            IDiagnostic? Diagnostic);

        private ConcurrentDictionary<ParameterAssignmentSymbol, Result> Results = new();
        private readonly SemanticModel model;
        private readonly ImmutableDictionary<string, ParameterAssignmentSymbol> paramsByName;

        public ParameterAssignmentEvaluator(SemanticModel model)
        {
            this.model = model;
            this.paramsByName = model.Root.ParameterAssignments
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        public Result EvaluateParameter(ParameterAssignmentSymbol parameter)
            => Results.GetOrAdd(
                parameter,
                parameter => {
                    try
                    {
                        var context = GetExpressionEvaluationContext();
                        var converter = new ExpressionConverter(new(model));
                        var expression = converter.ConvertExpression(parameter.DeclaringParameterAssignment.Value);

                        return new(expression.EvaluateExpression(context), null);
                    }
                    catch (Exception ex)
                    {
                        var diagnostic = DiagnosticBuilder.ForPosition(parameter.DeclaringParameterAssignment.Value)
                            .FailedToEvaluateParameter(parameter.Name, ex.Message);

                        return new(null, diagnostic);
                    }
                });

        private ExpressionEvaluationContext GetExpressionEvaluationContext()
        {
            var helper = new TemplateExpressionEvaluationHelper();
            helper.OnGetParameter = (name, _) => {
                return EvaluateParameter(paramsByName[name]).Value ?? throw new InvalidOperationException($"Parameter {name} has an invalid value");
            };

            return helper.EvaluationContext;
        }
    }
}
