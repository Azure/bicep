// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;

namespace Bicep.Core.Emit
{
    public static class EmitLimitationCalculator
    {
        public static EmitLimitationInfo Calculate(SemanticModel model)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var moduleScopeData = GetModuleScopeInfo(model, diagnosticWriter);
            var resourceScopeData = GetResoureScopeInfo(model, diagnosticWriter);
            DeployTimeConstantVisitor.ValidateDeployTimeConstants(model, diagnosticWriter);

            diagnosticWriter.WriteMultiple(DetectDuplicateNames(model, resourceScopeData, moduleScopeData));

            return new EmitLimitationInfo(diagnosticWriter.GetDiagnostics(), moduleScopeData, resourceScopeData);
        }

        public static ImmutableDictionary<ResourceSymbol, ScopeHelper.ScopeData> GetResoureScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            void logInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes)
                => diagnosticWriter.Write(positionable, x => x.UnsupportedResourceScope(suppliedScope, supportedScopes));

            var scopeInfo = new Dictionary<ResourceSymbol, ScopeHelper.ScopeData>();

            foreach (var resourceSymbol in semanticModel.Root.ResourceDeclarations)
            {
                var scopeProperty = resourceSymbol.SafeGetBodyProperty(LanguageConstants.ResourceScopePropertyName);
                var scopeData = ScopeHelper.ValidateScope(semanticModel, logInvalidScopeDiagnostic, scopeProperty);

                if (scopeData is null)
                {
                    continue;
                }

                scopeInfo[resourceSymbol] = scopeData;
            }

            return scopeInfo.ToImmutableDictionary();
        }

        private static ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> GetModuleScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            void logInvalidScopeDiagnostic(IPositionable positionable, ResourceScope suppliedScope, ResourceScope supportedScopes)
                => diagnosticWriter.Write(positionable, x => x.UnsupportedModuleScope(suppliedScope, supportedScopes));

            var scopeInfo = new Dictionary<ModuleSymbol, ScopeHelper.ScopeData>();

            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                var scopeProperty = moduleSymbol.SafeGetBodyProperty(LanguageConstants.ResourceScopePropertyName);
                var scopeData = ScopeHelper.ValidateScope(semanticModel, logInvalidScopeDiagnostic, scopeProperty);

                if (scopeData is null)
                {
                    scopeData = new ScopeHelper.ScopeData { RequestedScope = semanticModel.TargetScope };
                }

                if (!ScopeHelper.ValidateNestedTemplateScopeRestrictions(semanticModel, scopeData))
                {
                    if (scopeProperty is null)
                    {
                        // if there's a scope mismatch, the scope property will be required.
                        // this means a missing scope property will have already been flagged as an error by type validation.
                        continue;
                    }

                    switch (semanticModel.TargetScope)
                    {
                        case ResourceScope.Tenant:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForTenantScope());
                            break;
                        case ResourceScope.ManagementGroup:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForManagementScope());
                            break;
                        case ResourceScope.Subscription:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForSubscriptionScope());
                            break;
                        case ResourceScope.ResourceGroup:
                            diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForResourceGroup());
                            break;
                        default:
                            throw new InvalidOperationException($"Unrecognized target scope {semanticModel.TargetScope}");
                    }
                    continue;
                }

                scopeInfo[moduleSymbol] = scopeData;
            }

            return scopeInfo.ToImmutableDictionary();
        }

        private static IEnumerable<Diagnostic> DetectDuplicateNames(SemanticModel semanticModel, ImmutableDictionary<ResourceSymbol, ScopeHelper.ScopeData> resourceScopeData, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
            // This method only checks, if in one deployment we do not have 2 or more resources with this same name in one deployment to avoid template validation error
            // This will not check resource constraints such as necessity of having unique virtual network names within resource group

            var duplicateResources = GetResourceDefinitions(semanticModel, resourceScopeData)
                .GroupBy(x => x, ResourceDefinition.EqualityComparer)
                .Where(group => group.Count() > 1);

            foreach (var duplicatedResourceGroup in duplicateResources)
            {
                var duplicatedResourceNames = duplicatedResourceGroup.Select(x => x.ResourceName).ToArray();
                foreach (var duplicatedResource in duplicatedResourceGroup)
                {
                    yield return DiagnosticBuilder.ForPosition(duplicatedResource.ResourceNamePropertyValue).ResourceMultipleDeclarations(duplicatedResourceNames);
                }
            }

            var duplicateModules = GetModuleDefinitions(semanticModel, moduleScopeData)
                .GroupBy(x => x, ModuleDefinition.EqualityComparer)
                .Where(group => group.Count() > 1);

            foreach (var duplicatedModuleGroup in duplicateModules)
            {
                var duplicatedModuleNames = duplicatedModuleGroup.Select(x => x.ModuleName).ToArray();
                foreach (var duplicatedModule in duplicatedModuleGroup)
                {
                    yield return DiagnosticBuilder.ForPosition(duplicatedModule.ModulePropertyNameValue).ModuleMultipleDeclarations(duplicatedModuleNames);
                }
            }
        }

        private static IEnumerable<ModuleDefinition> GetModuleDefinitions(SemanticModel semanticModel, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
        {
            foreach (var module in semanticModel.Root.ModuleDeclarations)
            {
                if (!moduleScopeData.TryGetValue(module, out var scopeData))
                {
                    //module has invalid scope provided, ignoring from duplicate check
                    continue;
                }
                if (module.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) is not StringSyntax propertyNameValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                var propertyScopeValue = (module.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName) as FunctionCallSyntax)?.Arguments.Select(x => x.Expression as StringSyntax).ToImmutableArray();

                yield return new ModuleDefinition(module.Name, scopeData.RequestedScope, propertyScopeValue, propertyNameValue);
            }
        }

        private static IEnumerable<ResourceDefinition> GetResourceDefinitions(SemanticModel semanticModel, ImmutableDictionary<ResourceSymbol, ScopeHelper.ScopeData> resourceScopeData)
        {
            foreach (var resource in semanticModel.Root.ResourceDeclarations)
            {
                if (resource.DeclaringResource.IsExistingResource())
                {
                    // 'existing' resources are not being deployed so duplicates are allowed
                    continue;
                }

                if (resource.Type is not ResourceType resourceType || resource.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) is not StringSyntax namePropertyValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                var scopeProperty = resource.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName);
                var resourceScope = scopeProperty != null ? semanticModel.GetSymbolInfo(scopeProperty) as ResourceSymbol : null;

                yield return new ResourceDefinition(resource.Name, resourceScope, resourceType.TypeReference.FullyQualifiedType, namePropertyValue);
            }
        }
    }
}
