// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Bicep.Types.Concrete;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Functions;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem.Types;
using ExpressionEvaluationContext = Azure.Deployments.Expression.Intermediate.ExpressionEvaluationContext;
using FunctionEvaluator = Azure.Deployments.Expression.Intermediate.FunctionEvaluator;
using IntermediateFunctionExpression = Azure.Deployments.Expression.Intermediate.FunctionExpression;
using FunctionExpression = Azure.Deployments.Expression.Expressions.FunctionExpression;
using Azure.Deployments.Expression.Engines;

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

        public static FunctionOverload.ArmExpressionEvaluatorDelegate GetArmExpressionEvaluator(NamespaceFunctionType functionDefinition)
        {
            return expression =>
            {
                var evaluationContext = new ExpressionEvaluationContext (
                [
                    new NamespaceFunctionEvaluationScope(functionDefinition, expression),
                    Azure.Deployments.Expression.Expressions.ExpressionBuiltInFunctions.Functions,
                ]);
                var parsed = ExpressionParser.ParseLanguageExpression(functionDefinition.EvaluatedLanguageExpression);
                return evaluationContext.EvaluateExpression(parsed).ToLanguageExpression();
            };
        }

        private class NamespaceFunctionEvaluationScope : IExpressionEvaluationScope
        {
            private readonly Dictionary<string, FunctionEvaluator> functionTable;

            public NamespaceFunctionEvaluationScope(NamespaceFunctionType functionDefinition, FunctionExpression expression)
            {
                this.functionTable = new(StringComparer.OrdinalIgnoreCase)
                {
                    [LanguageConstants.ExternalInputBicepFunctionName] = EvaluateExternalInput,
                    [ExpressionConstants.ParametersFunction] = new ParametersFunction(functionDefinition, expression).Evaluate
                };
            }

            public FunctionEvaluator? TryGetFunctionEvaluator(string functionName)
                => this.functionTable.GetValueOrDefault(functionName);

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

        //private class ExternalInputFunction : BinaryExpressionFunction<StringExpression, IValueExpression>
        //{
        //    public override string Name => LanguageConstants.ExternalInputBicepFunctionName;
        //    protected override ITemplateLanguageExpression? Evaluate(string functionName, StringExpression kind, IValueExpression config, IPositionalMetadataHolder positionalMetadata)
        //    {
        //        return new IntermediateFunctionExpression(
        //            LanguageConstants.ExternalInputBicepFunctionName,
        //            [kind, config],
        //            positionalMetadata);
        //    }
        //}

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

            protected override ITemplateLanguageExpression? Evaluate(string functionName, StringExpression parameterName, IPositionalMetadataHolder positionalMetadata)
            {
                if (!parameterLookup.TryGetValue(parameterName.Value, out var parameterIdx))
                {
                    throw new InvalidOperationException($"Parameter '{parameterName.Value}' not found in function '{functionName}'.");
                }

                // map the LanguageExpression to ITemplateLanguageExpression
                var evalHelper = new TemplateExpressionEvaluationHelper();
                var evaluatedParam = FunctionExpression.Parameters[parameterIdx].EvaluateExpression(evalHelper.EvaluationContext);
                return ExpressionParser.ParseLanguageExpression(evaluatedParam);
            }
        }
    }
}
