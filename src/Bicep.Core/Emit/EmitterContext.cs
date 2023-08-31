// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.DataFlow;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;
using System.Collections.Immutable;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        public EmitterContext(SemanticModel semanticModel)
        {
            Settings = new(semanticModel);
            SemanticModel = semanticModel;
            DataFlowAnalyzer = new(semanticModel);
            VariablesToInline = InlineDependencyVisitor.GetVariablesToInline(semanticModel);
            ResourceDependencies = ResourceDependencyVisitor.GetResourceDependencies(semanticModel, new() { IncludeExisting = Settings.EnableSymbolicNames });
            FunctionVariables = FunctionVariableGeneratorVisitor.GetFunctionVariables(semanticModel);
        }

        public EmitterSettings Settings { get; }

        public SemanticModel SemanticModel { get; }

        public DataFlowAnalyzer DataFlowAnalyzer { get; }

        public ImmutableHashSet<VariableSymbol> VariablesToInline { get; }

        public ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> ResourceDependencies { get; }

        public ImmutableDictionary<FunctionCallSyntaxBase, FunctionVariable> FunctionVariables { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData => SemanticModel.EmitLimitationInfo.ModuleScopeData;

        public ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> ResourceScopeData => SemanticModel.EmitLimitationInfo.ResourceScopeData;
    }
}
