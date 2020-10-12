// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.SemanticModel;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        public EmitterContext(SemanticModel.SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
            this.VariablesToInline = InlineDependencyVisitor.GetVariablesToInline(semanticModel);
            this.ResourceDependencies = ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        public SemanticModel.SemanticModel SemanticModel { get; }

        public ImmutableArray<VariableSymbol> VariablesToInline { get; }

        public ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> ResourceDependencies { get; }
    }
}
