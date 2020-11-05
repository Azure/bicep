// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        public EmitterContext(SemanticModel.SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
            this.VariablesToInline = InlineDependencyVisitor.GetVariablesToInline(semanticModel);
            this.ResourceDependencies = ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
            this.ScopeDataLookup = semanticModel.Root.ModuleDeclarations.ToImmutableDictionary(module => module, module => GetScopeData(semanticModel, module));
        }

        private static ScopeHelper.ScopeData GetScopeData(SemanticModel.SemanticModel semanticModel, ModuleSymbol moduleSymbol)
        {
            var scopeProperty = (moduleSymbol.DeclaringModule.Body as ObjectSyntax)?.SafeGetPropertyByName("scope");
            if (scopeProperty == null)
            {
                return new ScopeHelper.ScopeData { RequestedScope = semanticModel.TargetScope };
            }
            var scopeType = semanticModel.GetTypeInfo(scopeProperty.Value);

            return ScopeHelper.GetScopeData(semanticModel.TargetScope, scopeType);
        }

        public SemanticModel.SemanticModel SemanticModel { get; }

        public ImmutableHashSet<VariableSymbol> VariablesToInline { get; }

        public ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> ResourceDependencies { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ScopeDataLookup { get; }
    }
}
