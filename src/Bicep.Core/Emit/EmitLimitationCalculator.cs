// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public static ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> GetSupportedScopeInfo(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
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
    }
}
