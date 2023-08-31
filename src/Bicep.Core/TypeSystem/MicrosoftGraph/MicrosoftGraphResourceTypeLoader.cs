// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Bicep.Types;
using Bicep.Core.Resources;
using Microsoft.Graph.Bicep.Types;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.MicrosoftGraph
{
    public class MicrosoftGraphResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly MicrosoftGraphResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, TypeLocation> availableTypes;

        public MicrosoftGraphResourceTypeLoader()
        {
            this.typeLoader = new MicrosoftGraphTypeLoader();
            this.resourceTypeFactory = new MicrosoftGraphResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            this.availableTypes = indexedTypes.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }
    }
}
