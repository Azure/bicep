// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Bicep.Types;
using Azure.Bicep.Types.K8s;
using Bicep.Core.Resources;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.K8s
{
    public class K8sResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly K8sResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, TypeLocation> availableTypes;

        public K8sResourceTypeLoader()
        {
            this.typeLoader = new K8sTypeLoader();
            this.resourceTypeFactory = new K8sResourceTypeFactory();
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
