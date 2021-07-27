// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics.Metadata
{
    public class ResourceMetadata
    {
        public ResourceMetadata(ResourceSymbol symbol)
        {
            Symbol = symbol;
        }

        public ResourceSymbol Symbol { get; }

        public ResourceTypeReference GetResourceTypeReference()
            => Symbol.GetResourceTypeReference();

        public bool IsExistingResource()
            => Symbol.DeclaringResource.IsExistingResource();
    }

    public class ResourceMetadataCache : SyntaxMetadataCacheBase<ResourceMetadata?>
    {
        private readonly SemanticModel semanticModel;
        private readonly ConcurrentDictionary<ResourceSymbol, ResourceMetadata> symbolLookup;

        public ResourceMetadataCache(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.symbolLookup = new();
        }

        protected override ResourceMetadata? Calculate(SyntaxBase syntax)
        {
            if (this.semanticModel.GetSymbolInfo(syntax) is ResourceSymbol symbol)
            {
                // Avoid duplicating metadata for the same symbol, so that ResourceMetadata can be compared by reference.
                return symbolLookup.GetOrAdd(
                    symbol,
                    symbol => new ResourceMetadata(symbol));
            }

            return null;
        }
    }
}
