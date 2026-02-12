// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Bicep.Types.Concrete;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Functions;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem.Types;
using FunctionExpression = Azure.Deployments.Expression.Expressions.FunctionExpression;
using IntermediateFunctionExpression = Azure.Deployments.Expression.Intermediate.FunctionExpression;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ExtensionNamespaceTypeHelper
    {
        public static ImmutableArray<NamedTypeProperty> GetExtensionNamespaceObjectProperties(NamespaceSettings namespaceSettings, IFeatureProvider features)
        {
            if (!features.ModuleExtensionConfigsEnabled || namespaceSettings.ConfigurationType is null)
            {
                return [];
            }

            return
            [
                new NamedTypeProperty(LanguageConstants.ExtensionConfigPropertyName, namespaceSettings.ConfigurationType, TypePropertyFlags.ReadOnly, "The extension configuration."),
            ];
        }

        /// <summary>
        /// Creates a ARM LanguageExpression evaluator to be invoked when emitting extension namespace functions
        /// that specify how they should be reduced via the <see cref="NamespaceFunctionType.EvaluatedLanguageExpression"/>.
        /// </summary>
        public static FunctionOverload.ArmExpressionEvaluatorDelegate GetArmExpressionEvaluator(NamespaceFunctionType functionDefinition)
        {
            return expression =>
            {
                var evaluationContext = new ExpressionEvaluationContext (
                [
                    Azure.Deployments.Expression.Expressions.ExpressionBuiltInFunctions.Functions,
                    new NamespaceFunctionEvaluationScope(functionDefinition, expression),
                ]);
                var parsed = ExpressionParser.ParseLanguageExpression(functionDefinition.EvaluatedLanguageExpression);
                return evaluationContext.EvaluateExpression(parsed).ToLanguageExpression();
            };
        }

        private class NamespaceFunctionEvaluationScope(NamespaceFunctionType functionDefinition, FunctionExpression expression)
            : IExpressionEvaluationScope
        {
            private readonly Dictionary<string, FunctionEvaluator> functionTable = new(StringComparer.OrdinalIgnoreCase)
            {
                [LanguageConstants.ExternalInputBicepFunctionName] = EvaluateExternalInput,
                [ExpressionConstants.ParametersFunction] = new ParametersFunction(functionDefinition, expression).Evaluate
            };

            public FunctionEvaluator? TryGetFunctionEvaluator(string functionName) => this.functionTable.GetValueOrDefault(functionName);

            private static IntermediateFunctionExpression EvaluateExternalInput(
                ExpressionEvaluationContext context,
                string functionName,
                ImmutableArray<ITemplateLanguageExpression> arguments,
                IPositionalMetadataHolder positionalMetadata) => new(
                    functionName,
                    [.. arguments.Select(context.EvaluateExpression)],
                    positionalMetadata,
                    irreducible: true);
        }

        private class ParametersFunction : UnaryExpressionFunction<StringExpression>
        {
            private readonly Dictionary<string, int> parameterLookup;
            public ParametersFunction(NamespaceFunctionType functionDefinition, FunctionExpression functionExpression)
            {
                this.FunctionExpression = functionExpression;
                this.parameterLookup = functionDefinition.Parameters
                    .Select((p, index) => new { p.Name, Index = index })
                    .ToDictionary(x => x.Name, x => x.Index);
            }

            public override string Name => ExpressionConstants.ParametersFunction;

            private FunctionExpression FunctionExpression { get; }

            protected override ITemplateLanguageExpression Evaluate(string functionName, StringExpression parameterName, IPositionalMetadataHolder positionalMetadata)
            {
                if (!parameterLookup.TryGetValue(parameterName.Value, out var parameterIdx))
                {
                    throw new InvalidOperationException($"Parameter '{parameterName.Value}' not found in function '{functionName}'.");
                }

                // convert LanguageExpression to ITemplateLanguageExpression
                // is there a better way to do this?
                return ExpressionParser.ParseLanguageExpression(
                    ExpressionsEngine.SerializeExpression(FunctionExpression.Parameters[parameterIdx]));
            }
        }
    }
}
