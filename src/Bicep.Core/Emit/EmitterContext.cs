// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.DataFlow;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        public EmitterContext(SemanticModel semanticModel)
        {
            Settings = semanticModel.EmitterSettings;
            SemanticModel = semanticModel;
            DataFlowAnalyzer = new(semanticModel);
            VariablesToInline = InlineDependencyVisitor.GetVariablesToInline(semanticModel);
            ResourceDependencies = ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
            FunctionVariables = FunctionVariableGeneratorVisitor.GetFunctionVariables(semanticModel);
            ExternalInputReferences = ExternalInputFunctionReferenceVisitor.CollectExternalInputReferences(semanticModel);
        }

        public EmitterSettings Settings { get; }

        public SemanticModel SemanticModel { get; }

        public DataFlowAnalyzer DataFlowAnalyzer { get; }

        public ImmutableHashSet<VariableSymbol> VariablesToInline { get; }

        public ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> ResourceDependencies { get; }

        public ImmutableDictionary<FunctionCallSyntaxBase, FunctionVariable> FunctionVariables { get; }

        public ImmutableDictionary<ModuleSymbol, ScopeHelper.ScopeData> ModuleScopeData => SemanticModel.EmitLimitationInfo.ModuleScopeData;

        public ImmutableDictionary<DeclaredResourceMetadata, ScopeHelper.ScopeData> ResourceScopeData => SemanticModel.EmitLimitationInfo.ResourceScopeData;

        public ExternalInputReferences ExternalInputReferences { get; }
    }
}
