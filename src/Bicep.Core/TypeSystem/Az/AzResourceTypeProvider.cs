// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Azure.Bicep.Types.Az;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        private readonly ITypeLoader typeLoader;
        private readonly AzResourceTypeFactory resourceTypeFactory;
        private readonly IReadOnlyDictionary<ResourceScopeType, IReadOnlyDictionary<ResourceTypeReference, TypeLocation>> availableResourceTypes;
        private readonly IDictionary<ResourceTypeReference, ResourceType> loadedTypeCache;

        public AzResourceTypeProvider()
            : this(new TypeLoader())
        {
        }

        private static IReadOnlyDictionary<ResourceScopeType, IReadOnlyDictionary<ResourceTypeReference, TypeLocation>> GetAvailableResourceTypes(ITypeLoader typeLoader)
        {
            IReadOnlyDictionary<ResourceTypeReference, TypeLocation> ToResourceReferenceDictionary(IReadOnlyDictionary<string, TypeLocation> typeDict)
                => typeDict.ToDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value,
                ResourceTypeReferenceComparer.Instance);

            var availableResourceTypes = new Dictionary<ResourceScopeType, IReadOnlyDictionary<ResourceTypeReference, TypeLocation>>();
            var indexedTypes = typeLoader.GetIndexedTypes();
            availableResourceTypes[ResourceScopeType.TenantScope] = ToResourceReferenceDictionary(indexedTypes.Tenant);
            availableResourceTypes[ResourceScopeType.ManagementGroupScope] = ToResourceReferenceDictionary(indexedTypes.ManagementGroup);
            availableResourceTypes[ResourceScopeType.SubscriptionScope] = ToResourceReferenceDictionary(indexedTypes.Subscription);
            availableResourceTypes[ResourceScopeType.ResourceGroupScope] = ToResourceReferenceDictionary(indexedTypes.ResourceGroup);
            availableResourceTypes[ResourceScopeType.ResourceScope] = ToResourceReferenceDictionary(indexedTypes.Extension);

            return availableResourceTypes;
        }

        public AzResourceTypeProvider(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            this.resourceTypeFactory = new AzResourceTypeFactory();
            this.availableResourceTypes = GetAvailableResourceTypes(typeLoader);
            this.loadedTypeCache = new Dictionary<ResourceTypeReference, ResourceType>(ResourceTypeReferenceComparer.Instance);
        }

        public ResourceType GetType(ResourceScopeType scopeType, ResourceTypeReference typeReference)
        {
            if (loadedTypeCache.TryGetValue(typeReference, out var resourceType))
            {
                return resourceType;
            }

            if (availableResourceTypes[scopeType].TryGetValue(typeReference, out var typeLocation))
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

        public bool HasType(ResourceScopeType scopeType, ResourceTypeReference typeReference)
            => availableResourceTypes[scopeType].ContainsKey(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes(ResourceScopeType scopeType)
            => availableResourceTypes[scopeType].Keys;
    }
}