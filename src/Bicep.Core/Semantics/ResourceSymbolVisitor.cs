// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Semantics
{
    public class ResourceSymbolVisitor(List<ResourceSymbol> resources) : SymbolVisitor
    {
        public static ImmutableArray<ResourceSymbol> GetAllResources(Symbol symbol)
        {
            var resources = new List<ResourceSymbol>();
            var visitor = new ResourceSymbolVisitor(resources);
            visitor.Visit(symbol);

            return resources.ToImmutableArray();
        }

        private readonly List<ResourceSymbol> resources = resources;

        public override void VisitResourceSymbol(ResourceSymbol symbol)
        {
            resources.Add(symbol);
            base.VisitResourceSymbol(symbol);
        }
    }
}
