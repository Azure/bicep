// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Bicep.Types.Az;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeLoader : IAzResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly AzResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, TypeLocation> availableTypes;

        public AzResourceTypeLoader()
        {
            this.typeLoader = new TypeLoader();
            this.resourceTypeFactory = new AzResourceTypeFactory();
            this.availableTypes = typeLoader.GetIndexedTypes().Types.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value,
                ResourceTypeReferenceComparer.Instance);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public ResourceType LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }
    }
}
