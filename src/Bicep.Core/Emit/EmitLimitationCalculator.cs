// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
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

            var moduleScopeData = GetSupportedScopeInfo(model, diagnosticWriter);
            var resourceScopeData = GetResoureScopeInfo(model, diagnosticWriter);
            DeployTimeConstantVisitor.ValidateDeployTimeConstants(model, diagnosticWriter);

            diagnosticWriter.WriteMultiple(DetectDuplicateNames(model, resourceScopeData, moduleScopeData));

            return new EmitLimitationInfo(diagnosticWriter.GetDiagnostics(), moduleScopeData, resourceScopeData);
        }

        public static ImmutableDictionary<ResourceSymbol, ResourceSymbol?> GetResoureScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var scopeInfo = new Dictionary<ResourceSymbol, ResourceSymbol?>();

            foreach (var resourceSymbol in semanticModel.Root.ResourceDeclarations)
            {
                var scopeValue = resourceSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName);
                if (scopeValue is null)
                {
                    scopeInfo[resourceSymbol] = null;
                    continue;
                }

                var scopeSymbol = semanticModel.GetSymbolInfo(scopeValue);
                if (scopeSymbol is not ResourceSymbol targetResourceSymbol)
                {
                    scopeInfo[resourceSymbol] = null;
                    diagnosticWriter.Write(scopeValue, x => x.InvalidExtensionResourceScope());
                    continue;
                }

                scopeInfo[resourceSymbol] = targetResourceSymbol;
            }

            return scopeInfo.ToImmutableDictionary();
        }

        private static ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> GetSupportedScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var moduleScopeData = new Dictionary<ModuleSymbol, ScopeHelper.ScopeData>();

            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                var scopeValue = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName);
                if (scopeValue == null)
                {
                    // no scope provided - assume the parent scope
                    moduleScopeData[moduleSymbol] = new ScopeHelper.ScopeData { RequestedScope = semanticModel.TargetScope };
                    continue;
                }

                var scopeType = semanticModel.GetTypeInfo(scopeValue);
                var scopeData = ScopeHelper.TryGetScopeData(semanticModel.TargetScope, scopeType);

                if (scopeData != null)
                {
                    moduleScopeData[moduleSymbol] = scopeData;
                    continue;
                }

                switch (semanticModel.TargetScope)
                {
                    case ResourceScopeType.TenantScope:
                        diagnosticWriter.Write(scopeValue, x => x.InvalidModuleScopeForTenantScope());
                        break;
                    case ResourceScopeType.ManagementGroupScope:
                        diagnosticWriter.Write(scopeValue, x => x.InvalidModuleScopeForManagementScope());
                        break;
                    case ResourceScopeType.SubscriptionScope:
                        diagnosticWriter.Write(scopeValue, x => x.InvalidModuleScopeForSubscriptionScope());
                        break;
                    case ResourceScopeType.ResourceGroupScope:
                        diagnosticWriter.Write(scopeValue, x => x.InvalidModuleScopeForResourceGroup());
                        break;
                    default:
                        throw new InvalidOperationException($"Unrecognized target scope {semanticModel.TargetScope}");
                }
            }

            return moduleScopeData.ToImmutableDictionary();
        }

        private static IEnumerable<Diagnostic> DetectDuplicateNames(SemanticModel semanticModel, ImmutableDictionary<ResourceSymbol, ResourceSymbol?> resourceScopeData, ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> moduleScopeData)
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
                if (!moduleScopeData.ContainsKey(module))
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

                yield return new ModuleDefinition(module.Name, moduleScopeData[module].RequestedScope, propertyScopeValue, propertyNameValue);
            }
        }

        private static IEnumerable<ResourceDefinition> GetResourceDefinitions(SemanticModel semanticModel, ImmutableDictionary<ResourceSymbol, ResourceSymbol?> resourceScopeData)
        {
            foreach (var resource in semanticModel.Root.ResourceDeclarations)
            {
                if (!resourceScopeData.ContainsKey(resource))
                {
                    //resource contains invlid scope data, ignoring from duplicate check
                    continue;
                }

                if (resource.Type is not ResourceType resourceType || resource.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) is not StringSyntax namePropertyValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                yield return new ResourceDefinition(resource.Name, resourceScopeData[resource], resourceType.TypeReference.FullyQualifiedType, namePropertyValue);
            }
        }
    }
}
