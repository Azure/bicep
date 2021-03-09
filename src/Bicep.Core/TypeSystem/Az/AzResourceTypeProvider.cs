// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Bicep.Types.Az;
using Bicep.Core.Resources;
using Bicep.Core.Emit;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeProvider : IResourceTypeProvider
    {
        public const string ResourceTypeDeployments = "Microsoft.Resources/deployments";
        public const string ResourceTypeResourceGroup = "Microsoft.Resources/resourceGroups";

        private readonly ITypeLoader typeLoader;
        private readonly AzResourceTypeFactory resourceTypeFactory;
        private readonly IReadOnlyDictionary<ResourceTypeReference, TypeLocation> availableResourceTypes;
        private readonly IDictionary<ResourceTypeReference, ResourceType> loadedTypeCache;
        private readonly IDictionary<ResourceTypeReference, ResourceType> loadedExistingTypeCache;

        public AzResourceTypeProvider()
            : this(new TypeLoader())
        {
        }

        private static IReadOnlyDictionary<ResourceTypeReference, TypeLocation> GetAvailableResourceTypes(ITypeLoader typeLoader)
        {
            var indexedTypes = typeLoader.GetIndexedTypes();

            return indexedTypes.Types.ToDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value,
                ResourceTypeReferenceComparer.Instance);
        }

        public AzResourceTypeProvider(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            this.resourceTypeFactory = new AzResourceTypeFactory();
            this.availableResourceTypes = GetAvailableResourceTypes(typeLoader);
            this.loadedTypeCache = new Dictionary<ResourceTypeReference, ResourceType>(ResourceTypeReferenceComparer.Instance);
            this.loadedExistingTypeCache = new Dictionary<ResourceTypeReference, ResourceType>(ResourceTypeReferenceComparer.Instance);
        }

        private ResourceType GenerateResourceType(ResourceTypeReference typeReference, bool isExistingResource)
        {
            if (availableResourceTypes.TryGetValue(typeReference, out var typeLocation))
            {
                var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
                var resourceType = resourceTypeFactory.GetResourceType(serializedResourceType);

                return SetBicepResourceProperties(resourceType, isExistingResource);
            }
            else
            {
                var resourceBodyType = new NamedObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, LanguageConstants.CreateResourceProperties(typeReference), null);
                var resourceType = new ResourceType(typeReference, ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource, resourceBodyType);

                return SetBicepResourceProperties(resourceType, isExistingResource);
            }
        }

        public static ResourceType SetBicepResourceProperties(ResourceType resourceType, bool isExistingResource)
        {
            var bodyType = resourceType.Body.Type;

            switch (bodyType)
            {
                case ObjectType bodyObjectType:
                    bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, isExistingResource);
                    break;
                case DiscriminatedObjectType bodyDiscriminatedType:
                    var bodyTypes = bodyDiscriminatedType.UnionMembersByKey.Values.ToList()
                        .Select(x => x.Type as ObjectType ?? throw new ArgumentException($"Resource {resourceType.Name} has unexpected body type {bodyType.GetType()}"));
                    bodyTypes = bodyTypes.Select(x => SetBicepResourceProperties(x, resourceType.ValidParentScopes, resourceType.TypeReference, isExistingResource));
                    bodyType = new DiscriminatedObjectType(
                        bodyDiscriminatedType.Name,
                        bodyDiscriminatedType.ValidationFlags,
                        bodyDiscriminatedType.DiscriminatorKey,
                        bodyTypes);
                    break;
                default:
                    // we exhaustively test deserialization of every resource type during CI, and this happens in a deterministic fashion,
                    // so this exception should never occur in the released product
                    throw new ArgumentException($"Resource {resourceType.Name} has unexpected body type {bodyType.GetType()}");
            }

            return new ResourceType(resourceType.TypeReference, resourceType.ValidParentScopes, bodyType);
        }

        private static ObjectType SetBicepResourceProperties(ObjectType objectType, ResourceScope validParentScopes, ResourceTypeReference typeReference, bool isExistingResource)
        {
            var properties = objectType.Properties;

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant;
            if (validParentScopes == ResourceScope.Resource)
            {
                // resource can only be deployed as an extension resource - scope should be required
                scopePropertyFlags |= TypePropertyFlags.Required;
            }

            if (isExistingResource)
            {
                // we can refer to a resource at any scope if it is an existing resource not being deployed by this file
                var scopeReference = LanguageConstants.CreateResourceScopeReference(validParentScopes);
                properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, new TypeProperty(LanguageConstants.ResourceScopePropertyName, scopeReference, scopePropertyFlags));
            }
            else
            {
                // TODO: remove 'dependsOn' from the type library
                properties = properties.SetItem(LanguageConstants.ResourceDependsOnPropertyName, new TypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly));
                
                // we only support scope for extension resources (or resources where the scope is unknown and thus may be an extension resource)
                if (validParentScopes.HasFlag(ResourceScope.Resource))
                {
                    var scopeReference = LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource);
                    properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, new TypeProperty(LanguageConstants.ResourceScopePropertyName, scopeReference, scopePropertyFlags));
                }
            }

            // add the 'parent' property for child resource types
            if (!typeReference.IsRootType)
            {
                var parentType = LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource);
                var parentFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant;

                properties = properties.SetItem(LanguageConstants.ResourceParentPropertyName, new TypeProperty(LanguageConstants.ResourceParentPropertyName, parentType, parentFlags));
            }

            // Deployments RP
            if (StringComparer.OrdinalIgnoreCase.Equals(objectType.Name, ResourceTypeDeployments))
            {
                properties = properties.SetItem("resourceGroup", new TypeProperty("resourceGroup", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant));
                properties = properties.SetItem("subscriptionId", new TypeProperty("subscriptionId", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant));
            }
            return new NamedObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                objectType.AdditionalPropertiesType,
                isExistingResource ? ConvertToReadOnly(objectType.AdditionalPropertiesFlags) : objectType.AdditionalPropertiesFlags);
        }

        private static IEnumerable<TypeProperty> ConvertToReadOnly(IEnumerable<TypeProperty> properties)
        {
            foreach (var property in properties)
            {
                // "name" and "scope" can be set for existing resources - everything else should be read-only
                if (property.Name == LanguageConstants.ResourceNamePropertyName ||
                    property.Name == LanguageConstants.ResourceScopePropertyName)
                {
                    yield return property;
                }
                else
                {
                    yield return new TypeProperty(property.Name, property.TypeReference, ConvertToReadOnly(property.Flags));
                }
            }
        }

        private static TypePropertyFlags ConvertToReadOnly(TypePropertyFlags typePropertyFlags)
            => (typePropertyFlags | TypePropertyFlags.ReadOnly) & ~TypePropertyFlags.Required;

        public ResourceType GetType(ResourceTypeReference typeReference, bool isExistingResource)
        {
            var typeCache = isExistingResource ? loadedExistingTypeCache : loadedTypeCache;

            // It's important to cache this result because LoadResourceType is an expensive operation
            if (!typeCache.TryGetValue(typeReference, out var resourceType))
            {
                resourceType = GenerateResourceType(typeReference, isExistingResource);
                typeCache[typeReference] = resourceType;
            }

            return resourceType;
        }

        public bool HasType(ResourceTypeReference typeReference)
            => availableResourceTypes.ContainsKey(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes.Keys;
    }
}