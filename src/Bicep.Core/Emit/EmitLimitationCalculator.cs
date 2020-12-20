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

namespace Bicep.Core.Emit
{
    public static class EmitLimitationCalculator
    {
        public static EmitLimitationInfo Calculate(SemanticModel semanticModel)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var moduleScopeData = GetSupportedScopeInfo(semanticModel, diagnosticWriter);
            var resourceScopeData = GetResoureScopeInfo(semanticModel, diagnosticWriter);

            diagnosticWriter.WriteMultiple(DetectDuplicateNames(semanticModel, resourceScopeData, moduleScopeData));

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

            var duplicateResources = semanticModel.Root.AllDeclarations
                .OfType<ResourceSymbol>()
                .Where(res => res.Type is ResourceType && res.DeclaringResource.Body is ObjectSyntax)
                .Select(res =>
                {
                    var namePropSyntax = ((ObjectSyntax)res.DeclaringResource.Body).SafeGetPropertyByName(LanguageConstants.ResourceNamePropertyName);
                    var resourceScope = resourceScopeData[res];
                    return new
                    {
                        ResourceName = res.Name,
                        ResourceScope = resourceScope,
                        ResourceTypeFQDN = ((ResourceType)res.Type).TypeReference.FullyQualifiedType,
                        ResourceNamePropertyValue = namePropSyntax?.Value as StringSyntax, //TODO: currently limiting check to property values that are strings, although it can be references or other syntaxes
                        ResourceNamePropertySyntax = namePropSyntax
                    };
                })
                .Where(x => x.ResourceNamePropertySyntax is not null && x.ResourceNamePropertyValue is not null)
                .GroupBy(x => (x.ResourceTypeFQDN, x.ResourceScope, x.ResourceNamePropertyValue!), ResourceComparer.Instance)
                .Where(group => group.Count() > 1);

            foreach (var duplicatedResourceGroup in duplicateResources)
            {
                var duplicatedResourceNames = string.Join(", ", duplicatedResourceGroup.Select(x => x.ResourceName));
                foreach (var duplicatedResource in duplicatedResourceGroup)
                {
                    yield return DiagnosticBuilder.ForPosition(duplicatedResource.ResourceNamePropertySyntax!).ResourceMultipleDeclarations(duplicatedResourceNames);
                }
            }

            var duplicateModules = semanticModel.Root.AllDeclarations
                .OfType<ModuleSymbol>()
                .Where(res => res.Type is ModuleType && res.DeclaringModule is ModuleDeclarationSyntax && moduleScopeData.ContainsKey(res))
                .Select(res =>
                {
                    var namePropSyntax = ((ObjectSyntax)res.DeclaringModule.Body).SafeGetPropertyByName(LanguageConstants.ResourceNamePropertyName);
                    var scopeType = moduleScopeData[res].RequestedScope;
                    var scopeProperty = ((ObjectSyntax)res.DeclaringModule.Body).SafeGetPropertyByName(LanguageConstants.ResourceScopePropertyName);

                    var r = new
                    {
                        ModuleName = res.Name,
                        ModulePropertyScopeType = scopeType,
                        ModulePropertyScopeValue = ((scopeProperty?.Value as FunctionCallSyntax)?.Arguments.FirstOrDefault()?.Expression as StringSyntax),
                        ModulePropertyNameValue = namePropSyntax?.Value as StringSyntax, //TODO: currently limiting check to property values that are strings, although it can be references or other syntaxes
                        ModulePropertyNameSyntax = namePropSyntax
                    };
                    return r;
                })
                .Where(x => x.ModulePropertyNameSyntax is not null && x.ModulePropertyNameValue is not null)
                .GroupBy(x => (x.ModulePropertyScopeType, x.ModulePropertyScopeValue, x.ModulePropertyNameValue!), ModuleComparer.Instance)
                .Where(group => group.Count() > 1);

            foreach (var duplicatedModuleGroup in duplicateModules)
            {
                var duplicatedModuleNames = string.Join(", ", duplicatedModuleGroup.Select(x => x.ModuleName));
                foreach (var duplicatedModule in duplicatedModuleGroup)
                {
                    yield return DiagnosticBuilder.ForPosition(duplicatedModule.ModulePropertyNameSyntax!).ModuleMultipleDeclarations(duplicatedModuleNames);
                }
            }
        }


        // comparers below are very simple now, however in future it might be used to do more exact comparsion on property value to include interpolations
        // also, we expect StringSyntax as values it can be other types as well (function calls, variable accesses, etc.)
        private class ResourceComparer : IEqualityComparer<(string ResourceTypeFQDN, ResourceSymbol? ResourceScope, StringSyntax PropertyNameValue)>
        {
            public static readonly ResourceComparer Instance = new();

            public bool Equals((string ResourceTypeFQDN, ResourceSymbol? ResourceScope, StringSyntax PropertyNameValue) x, (string ResourceTypeFQDN, ResourceSymbol? ResourceScope, StringSyntax PropertyNameValue) y)
            {

                if (!string.Equals(x.ResourceTypeFQDN, y.ResourceTypeFQDN, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                
                if (x.ResourceScope != y.ResourceScope)
                {
                    return false;
                }

                var xv = x.PropertyNameValue.TryGetLiteralValue();
                var yv = y.PropertyNameValue.TryGetLiteralValue();

                //if literal value is null, we assume resources are not equal, as this indicates that interpolated value is used
                //and as for now we're unable to determine if they will have equal values or not.

                return xv is not null && yv is not null && string.Equals(xv, yv, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((string ResourceTypeFQDN, ResourceSymbol? ResourceScope,  StringSyntax PropertyNameValue) obj)
            {
                return HashCode.Combine(obj.ResourceTypeFQDN.ToLowerInvariant(), obj.ResourceScope, obj.PropertyNameValue.TryGetLiteralValue()?.ToLowerInvariant());
            }
        }

        private class ModuleComparer : IEqualityComparer<(ResourceScopeType ModuleScopeType, StringSyntax? ModuleScopeValue, StringSyntax PropertyNameValue)>
        {
            public static readonly ModuleComparer Instance = new();

            public bool Equals(
                (ResourceScopeType ModuleScopeType, StringSyntax? ModuleScopeValue, StringSyntax PropertyNameValue) x,
                (ResourceScopeType ModuleScopeType, StringSyntax? ModuleScopeValue, StringSyntax PropertyNameValue) y)
            {
                if (x.ModuleScopeType != y.ModuleScopeType)
                {
                    return false;
                }

                var xpnv = x.PropertyNameValue.TryGetLiteralValue();
                var ypnv = y.PropertyNameValue.TryGetLiteralValue();

                if (xpnv is null || ypnv is null || !string.Equals(xpnv, ypnv, StringComparison.OrdinalIgnoreCase))
                {
                    //no point in checking scope at least one of them is interpolated or their literal names differ
                    return false;
                }

                if (x.ModuleScopeValue is null && y.ModuleScopeValue is null)
                {
                    // this case indicates that modules are being deployed to scope without name specified (e.g. subscription(), resourceGroup(), etc.)
                    // and as we checked before that names are equal, we need to return true.
                    return true;
                }

                var xmsv = x.ModuleScopeValue?.TryGetLiteralValue();
                var ymsv = y.ModuleScopeValue?.TryGetLiteralValue();
                //null values indicates that either module used is parent scope or module name is interpolated.
                //as we checked case, when we have both modules in parent scope, we can safely return false when null is found

                return xmsv is not null && ymsv is not null && string.Equals(xmsv, ymsv, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((ResourceScopeType ModuleScopeType, StringSyntax? ModuleScopeValue, StringSyntax PropertyNameValue) obj)
            {
                return HashCode.Combine(
                    obj.ModuleScopeType,
                    obj.ModuleScopeValue?.TryGetLiteralValue()?.ToLowerInvariant(),
                    obj.PropertyNameValue.TryGetLiteralValue()?.ToLowerInvariant());
            }
        }
    }
}
