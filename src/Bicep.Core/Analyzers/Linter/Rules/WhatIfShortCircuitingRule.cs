// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.ParsedEntities;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Newtonsoft.Json;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class WhatIfShortCircuitingRule : LinterRuleBase
    {
        public new const string Code = "what-if-short-circuiting";

        public const string TemplateEvaluatorCode = "expression-evaluation-failed";

        public WhatIfShortCircuitingRule() : base(
            code: Code,
            description: CoreResources.WhatIfShortCircuitingRuleDescription,
            LinterRuleCategory.PotentialCodeIssues,
            overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off) // Disabled by default while still experimental
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.WhatIfShortCircuitingRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            foreach (var module in model.Root.ModuleDeclarations)
            {
                if (module.DeclaringSyntax is ModuleDeclarationSyntax moduleSyntax && module.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel) && moduleSemanticModel is SemanticModel semanticModel)
                {
                    var moduleParamsPropertyObject = moduleSyntax.TryGetBody()?
                                    .TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName);
                    var moduleParams = (moduleParamsPropertyObject?.Value as ObjectSyntax)?.Properties;
                    if (moduleParams == null)
                    {
                        continue;
                    }

                    var moduleParamsInput = new Dictionary<string, ITemplateLanguageExpression>();
                    var moduleParamsHolder = new Dictionary<string, ObjectPropertySyntax>();
                    foreach (var param in moduleParams)
                    {
                        moduleParamsInput.Add(param.Key.ToString(), new FunctionExpression("sentinel-placeholder", [new StringExpression(param.Key.ToString(), null, null, null)], [], position: null));
                        moduleParamsHolder.Add(param.Key.ToString(), param);
                    }

                    var template = GetTemplate(semanticModel);
                    TemplateWithParsedExpressions? templateResult;

                    var metadata = new OrdinalInsensitiveDictionary<ITemplateLanguageExpression>
                    {
                        { DeploymentMetadata.TenantKey, new UnevaluableExpression(null, null, null) },
                        { DeploymentMetadata.ManagementGroupKey, new UnevaluableExpression(null, null, null) },
                        { DeploymentMetadata.SubscriptionKey, new UnevaluableExpression(null, null, null) },
                        { DeploymentMetadata.ResourceGroupKey, new UnevaluableExpression(null, null, null) },
                        { DeploymentMetadata.DeploymentKey, new UnevaluableExpression(null, null, null) },
                        { DeploymentMetadata.ProvidersKey, new UnevaluableExpression(null, null, null) },
                        { DeploymentMetadata.EnvironmentKey, new UnevaluableExpression(null, null, null) },
                    };

                    try
                    {
                        templateResult = TemplateEngine.ReduceTemplateLanguageExpressions(
                            managementGroupName: new UnevaluableExpression(null, null, null),
                            subscriptionId: new UnevaluableExpression(null, null, null),
                            resourceGroupName: new UnevaluableExpression(null, null, null),
                            template: template,
                            apiVersion: new StringExpression(EmitConstants.GetNestedDeploymentResourceApiVersion(model.Features), null, null, null),
                            suppliedParameterValues: moduleParamsInput,
                            parameterValuesPositionalMetadata: null,
                            metadata: metadata,
                            metricsRecorder: null);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Raise diagnostic when the template evaluation fails if the diagnostic will provide extra context to the failure
                        // Adding a diagnostic here without checking for other errors would currently result in duplicate errors for any error in module params
                        Trace.WriteLine($"Exception occurred while reducing template language expressions: {ex}");
                        continue;
                    }

                    var sentinelVisitor = new CrossModuleSentinelVisitor();

                    foreach (var resource in templateResult.Resources)
                    {
                        CheckResource(resource, sentinelVisitor);
                    }

                    var resourceTypeResolver = ResourceTypeResolver.Create(model);
                    foreach (var param in sentinelVisitor.parametersEncountered)
                    {
                        var paramSyntax = moduleParamsHolder[param];
                        if (!IsDeployTimeConstant(paramSyntax, model, resourceTypeResolver))
                        {
                            yield return CreateDiagnosticForSpan(diagnosticLevel, paramSyntax.Value.Span, paramSyntax.Value, module.Name);
                        }
                    }
                }
            }
        }

        private static bool IsDeployTimeConstant(ObjectPropertySyntax syntax, SemanticModel model, ResourceTypeResolver resolver)
        {
            var diagWriter = ToListDiagnosticWriter.Create();
            DeployTimeConstantValidator.CheckDeployTimeConstantViolations(syntax, syntax.Value, model, diagWriter, resolver);

            return diagWriter.GetDiagnostics().Count == 0;
        }

        private void CheckResource(TemplateResourceWithParsedExpressions resource, CrossModuleSentinelVisitor sentinelVisitor)
        {
            if (resource.Resources != null)
            {
                foreach (var nestedResource in resource.Resources)
                {
                    CheckResource(nestedResource, sentinelVisitor);
                }
            }

            resource.Name?.Accept(sentinelVisitor);
            resource.SubscriptionId?.Accept(sentinelVisitor);
            resource.ResourceGroup?.Accept(sentinelVisitor);
            resource.Condition?.Accept(sentinelVisitor);
            resource.Scope?.Accept(sentinelVisitor);
            resource.ApiVersion?.Accept(sentinelVisitor);
        }

        private static Template GetTemplate(SemanticModel model)
        {
            try
            {
                var textWriter = new StringWriter();
                using var writer = new SourceAwareJsonTextWriter(textWriter)
                {
                    // don't close the textWriter when writer is disposed
                    CloseOutput = false,
                    Formatting = Formatting.Indented
                };
                var (template, _) = new TemplateWriter(model).GetTemplate(writer);

                return template;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to generate template for {model.SourceFile.FileHandle.Uri}: {ex}");
                return new Template();
            }
        }

        private class CrossModuleSentinelVisitor : TemplateLanguageExpressionVisitor
        {
            public HashSet<string> parametersEncountered = new();

            override public void VisitFunctionExpression(FunctionExpression func)
            {
                if (func.Name == "sentinel-placeholder")
                {
                    if (func.Arguments.Length != 1 ||
                        func.Arguments.Single() is not StringExpression { Value: string parameterName })
                    {
                        throw new InvalidOperationException("Something's not right here...");
                    }

                    parametersEncountered.Add(parameterName);
                }

                base.VisitFunctionExpression(func);
            }
        }
    }
}
