using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.SemanticModel
{
    public class SymbolDependencies
    {
        public SymbolDependencies(IEnumerable<ResourceSymbol> resouces)
        {
            Resources = resouces.ToImmutableArray();
        }

        public ImmutableArray<ResourceSymbol> Resources { get; }
    }

    public class SymbolDependencyGraph
    {
        public SymbolDependencyGraph(ImmutableDictionary<Symbol, SymbolDependencies> graph)
        {
            Graph = graph;
        }

        public ImmutableDictionary<Symbol, SymbolDependencies> Graph { get; }
    }
}