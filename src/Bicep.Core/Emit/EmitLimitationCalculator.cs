// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public static class EmitLimitationCalculator
    {
        public static EmitLimitationInfo Calculate(SemanticModel.SemanticModel semanticModel)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();

            var moduleScopeData = GetSupportedScopeInfo(semanticModel, diagnosticWriter);

            return new EmitLimitationInfo(diagnosticWriter.GetDiagnostics(), moduleScopeData);
        }

        public static ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> GetSupportedScopeInfo(SemanticModel.SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var moduleScopeData = new Dictionary<ModuleSymbol, ScopeHelper.ScopeData>();

            foreach (var moduleSymbol in semanticModel.Root.ModuleDeclarations)
            {
                var scopeProperty = (moduleSymbol.DeclaringModule.Body as ObjectSyntax)?.SafeGetPropertyByName(LanguageConstants.ModuleScopePropertyName);
                if (scopeProperty == null)
                {
                    // no scope provided - assume the parent scope
                    moduleScopeData[moduleSymbol] = new ScopeHelper.ScopeData { RequestedScope = semanticModel.TargetScope };
                    continue;
                }

                var scopeType = semanticModel.GetTypeInfo(scopeProperty.Value);
                var scopeData = ScopeHelper.TryGetScopeData(semanticModel.TargetScope, scopeType);

                if (scopeData != null)
                {
                    moduleScopeData[moduleSymbol] = scopeData;
                    continue;
                }

                switch (semanticModel.TargetScope)
                {
                    case ResourceScopeType.TenantScope:
                        diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForTenantScope());
                        break;
                    case ResourceScopeType.ManagementGroupScope:
                        diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForManagementScope());
                        break;
                    case ResourceScopeType.SubscriptionScope:
                        diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForSubscriptionScope());
                        break;
                    case ResourceScopeType.ResourceGroupScope:
                        diagnosticWriter.Write(scopeProperty.Value, x => x.InvalidModuleScopeForResourceGroup());
                        break;
                    default:
                        throw new InvalidOperationException($"Unrecognized target scope {semanticModel.TargetScope}");
                }
            }

            return moduleScopeData.ToImmutableDictionary();
        }
    }
}
