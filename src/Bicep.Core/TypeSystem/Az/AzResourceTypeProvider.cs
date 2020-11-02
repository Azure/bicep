// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Resources;
using Bicep.SerializedTypes.Az;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        private readonly ITypeLoader typeLoader;
        private readonly AzResourceTypeFactory resourceTypeFactory;
        private readonly IReadOnlyDictionary<ResourceTypeReference, TypeLocation> availableResourceTypes;
        private readonly IDictionary<ResourceTypeReference, ResourceType> loadedTypeCache;

        public AzResourceTypeProvider()
            : this(new TypeLoader())
        {
        }

        public AzResourceTypeProvider(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            this.resourceTypeFactory = new AzResourceTypeFactory();
            this.availableResourceTypes = typeLoader.ListAllAvailableTypes().ToDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value,
                ResourceTypeReferenceComparer.Instance);
            this.loadedTypeCache = new Dictionary<ResourceTypeReference, ResourceType>(ResourceTypeReferenceComparer.Instance);
        }

        public ResourceType GetType(ResourceTypeReference typeReference)
        {
            if (loadedTypeCache.TryGetValue(typeReference, out var resourceType))
            {
                return resourceType;
            }

            if (availableResourceTypes.TryGetValue(typeReference, out var typeLocation))
            {
                // It's important to cache this result because LoadResourceType is an expensive operation, and
                // duplicating types means the resourceTypeFactor won't be able to use its cache.
                var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
                resourceType = resourceTypeFactory.GetResourceType(serializedResourceType);
            }
            else
            {
                var resourceBodyType = new NamedObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(typeReference), null);
                resourceType = new ResourceType(typeReference, resourceBodyType);
            }

            loadedTypeCache[typeReference] = resourceType;
            return resourceType;
        }

        public bool HasType(ResourceTypeReference typeReference)
            => availableResourceTypes.ContainsKey(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes.Keys;
    }
}