// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        public EmitterContext(SemanticModel.SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
            this.VariablesToInline = InlineDependencyVisitor.GetVariablesToInline(semanticModel);
            this.ResourceDependencies = ResourceDependencyVisitor.GetAllResourceDependencies(semanticModel);
        }

        public SemanticModel.SemanticModel SemanticModel { get; }

        public ImmutableHashSet<VariableSymbol> VariablesToInline { get; }

        public ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> ResourceDependencies { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData => SemanticModel.EmitLimitationInfo.ModuleScopeData;
    }
}
