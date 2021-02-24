// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

            var moduleScopeData = ScopeHelper.GetModuleScopeInfo(model, diagnosticWriter);
            var resourceScopeData = ScopeHelper.GetResoureScopeInfo(model, diagnosticWriter);
            DeployTimeConstantVisitor.ValidateDeployTimeConstants(model, diagnosticWriter);

            ForSyntaxValidatorVisitor.Validate(model, diagnosticWriter);

            diagnosticWriter.WriteMultiple(DetectDuplicateNames(model, resourceScopeData, moduleScopeData));

            return new EmitLimitationInfo(diagnosticWriter.GetDiagnostics(), moduleScopeData, resourceScopeData);
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

                if (!resourceScopeData.TryGetValue(resource, out var scopeData))
                {
                    scopeData = null;
                }

                if (resource.Type is not ResourceType resourceType || resource.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) is not StringSyntax namePropertyValue)
                {
                    //currently limiting check to 'name' property values that are strings, although it can be references or other syntaxes
                    continue;
                }

                yield return new ResourceDefinition(resource.Name, scopeData?.ResourceScopeSymbol, resourceType.TypeReference.FullyQualifiedType, namePropertyValue);
            }
        }
    }
}
