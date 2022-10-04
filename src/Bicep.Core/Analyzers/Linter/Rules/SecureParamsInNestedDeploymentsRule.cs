// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParamsInNestedDeploymentsRule : LinterRuleBase
    {
        // Finds insecure uses of parameters or list* functions inside outer-scpped nested deployments (resources of
        //   type "microsoft.resources/deployments").  Modules are inner-scoped so do not have this issue.
        //
        // These nested deployments will evaluate expressions prior to being sent to the deployment engine.This means that all
        // properties are in the request in an evaluated state (clear text) and persisted on the deployment object.

        public new const string Code = "secure-params-in-nested-deploy";

        public SecureParamsInNestedDeploymentsRule() : base(
            code: Code,
            description: CoreResources.SecureParamsInNestedDeployRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Warning)
        { }

        public override string FormatMessage(params object[] values)
        {
            string problem = (string)values[0];
            return $"{problem} {CoreResources.SecureParamsInNestedDeployRule_Solution}";
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var diagnosticLevel = GetDiagnosticLevel(model);
            foreach (ResourceSymbol resource in model.Root.ResourceDeclarations)
            {
                if (GetPropertiesIfOuterScopedDeployment(resource) is ObjectSyntax propertiesObject)
                {
                    var templateObject = propertiesObject.TryGetPropertyByName("template")?.Value as ObjectSyntax;
                    if (templateObject is not null)
                    {
                        // Look for secure params
                        var secureParams = FindSecureParametersReferencedInNestedDeployment(model, resource);
                        if (secureParams.Any())
                        {
                            string secureParamsAsString = String.Join(", ", secureParams.Select(p => $"'{p.Name}'"));
                            var message = string.Format(
                                CoreResources.SecureParamsInNestedDeployRule_Message_SecureParams,
                                resource.Name,
                                secureParamsAsString);
                            yield return CreateDiagnosticForSpan(diagnosticLevel, resource.NameSyntax.Span, message);
                        }

                        // Look for list* functions
                        var listFunctionReference = FindListFunctionReferencesInSyntaxTree(model, resource.DeclaringSyntax).FirstOrDefault();
                        if (listFunctionReference is not null)
                        {
                            var message = string.Format(
                                CoreResources.SecureParamsInNestedDeployRule_Message_ListFunction,
                                resource.Name,
                                listFunctionReference.ToText());
                            yield return CreateDiagnosticForSpan(diagnosticLevel, resource.NameSyntax.Span, message);
                        }
                    }
                }
            }
        }

        private static ObjectSyntax? GetPropertiesIfOuterScopedDeployment(ResourceSymbol resource)
        {
            if (resource.TryGetResourceType() is not ResourceType resourceType)
            {
                return null;
            }

            if (!resourceType.TypeReference.FormatType().Equals("microsoft.resources/deployments", LanguageConstants.ResourceTypeComparison))
            {
                // Not a deployment resource
                return null;
            }

            if (resource.TryGetBodyProperty(LanguageConstants.ResourcePropertiesPropertyName)?.Value is not ObjectSyntax propertiesObject)
            {
                // No properties object (not valid)
                return null;
            }

            // It's a valid deployment resource
            if (propertiesObject is not null
                && propertiesObject.TryGetPropertyByNameRecursive(new[] { "expressionEvaluationOptions", "scope" })?.Value
                is StringSyntax scope)
            {
                if (scope.TryGetLiteralValue()?.Equals("inner", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    // Scope is explicitly marked as "inner"
                    return null;
                }
            }

            // Scope is considered outer if not explicitly marked as "inner"
            return propertiesObject;
        }

        private static Symbol[] FindSecureParametersReferencedInNestedDeployment(SemanticModel model, ResourceSymbol deploymentResource)
        {
            var secureParamsReferencedInDeployment = model.Root.ParameterDeclarations
                .Where(p => p.NameSyntax.IsValid && p.IsSecure())
                .Where(p => model.FindReferences(p, deploymentResource.DeclaringSyntax).Any());
            return secureParamsReferencedInDeployment.ToArray();
        }

        private static IEnumerable<SyntaxBase> FindListFunctionReferencesInSyntaxTree(SemanticModel model, SyntaxBase syntaxTree)
        {
            return SyntaxAggregator.Aggregate(syntaxTree, current =>
            {
                if (SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, current) is FunctionCallSyntaxBase functionCall
                    && functionCall.Name.IdentifierName.StartsWith(LanguageConstants.ListFunctionPrefix))
                {
                    return true;
                }

                return false;
            });
        }
    }
}
