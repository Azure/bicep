using System.Collections.Immutable;

namespace Bicep.Core.SemanticModel
{
    public class SymbolGraph
    {
        public SymbolGraph(ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceSymbol>> resourceGraph)
        {
            ResourceGraph = resourceGraph;
        }

        public ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceSymbol>> ResourceGraph { get; } 
    }
}