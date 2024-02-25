// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Azure.Bicep.Types.K8s;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.K8s
{
    public class K8sResourceTypeLoader : IResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly ExtensibilityResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;

        public K8sResourceTypeLoader()
        {
            typeLoader = new K8sTypeLoader();
            resourceTypeFactory = new ExtensibilityResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            availableTypes = indexedTypes.Resources.ToImmutableDictionary(
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
