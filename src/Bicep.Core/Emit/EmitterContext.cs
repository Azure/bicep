// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.DataFlow;
using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        public EmitterContext(SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
            this.DataFlowAnalyzer = new(semanticModel);
            this.VariablesToInline = InlineDependencyVisitor.GetVariablesToInline(semanticModel);
            this.ResourceDependencies = ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        public SemanticModel SemanticModel { get; }

        public DataFlowAnalyzer DataFlowAnalyzer { get; }

        public ImmutableHashSet<VariableSymbol> VariablesToInline { get; }

        public ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> ResourceDependencies { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData => SemanticModel.EmitLimitationInfo.ModuleScopeData;

        public ImmutableDictionary<ResourceSymbol, ScopeHelper.ScopeData> ResourceScopeData => SemanticModel.EmitLimitationInfo.ResourceScopeData;
    }
}
