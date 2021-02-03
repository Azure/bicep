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
            var typeDict = new Dictionary<ResourceTypeReference, TypeLocation>(ResourceTypeReferenceComparer.Instance);
            var indexedTypes = typeLoader.GetIndexedTypes();

            void ToResourceReferenceDictionary(IReadOnlyDictionary<string, TypeLocation> inputDict)
            {
                foreach (var (key, value) in inputDict)
                {
                    typeDict[ResourceTypeReference.Parse(key)] = value;
                }
            }

            ToResourceReferenceDictionary(indexedTypes.Tenant);
            ToResourceReferenceDictionary(indexedTypes.ManagementGroup);
            ToResourceReferenceDictionary(indexedTypes.Subscription);
            ToResourceReferenceDictionary(indexedTypes.ResourceGroup);
            ToResourceReferenceDictionary(indexedTypes.Extension);

            return typeDict;
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
                    // Extra fields should be added here.
                    // Deployments RP
                    if (string.Equals(bodyObjectType.Name, TemplateWriter.NestedDeploymentResourceType, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var properties = bodyObjectType.Properties.Values.ToList();
                        properties.Add(new TypeProperty("resourceGroup", LanguageConstants.String));
                        properties.Add(new TypeProperty("subscriptionId", LanguageConstants.String));
                        bodyObjectType = new NamedObjectType(
                            bodyObjectType.Name,
                            bodyObjectType.ValidationFlags,
                            properties,
                            bodyObjectType.AdditionalPropertiesType,
                            bodyObjectType.AdditionalPropertiesFlags);
                    }
                    bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, isExistingResource);
                    break;
                case DiscriminatedObjectType bodyDiscriminatedType:
                    var bodyTypes = bodyDiscriminatedType.UnionMembersByKey.Values.ToList()
                        .Select(x => x.Type as ObjectType ?? throw new ArgumentException($"Resource {resourceType.Name} has unexpected body type {bodyType.GetType()}"));
                    bodyTypes = bodyTypes.Select(x => SetBicepResourceProperties(x, resourceType.ValidParentScopes, isExistingResource));
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

        private static ObjectType SetBicepResourceProperties(ObjectType objectType, ResourceScope validParentScopes, bool isExistingResource)
        {
            var properties = objectType.Properties;

            var scopeRequiredFlag = TypePropertyFlags.WriteOnly;
            if (validParentScopes == ResourceScope.Resource)
            {
                // resource can only be deployed as an extension resource - scope should be required
                scopeRequiredFlag |= TypePropertyFlags.Required;
            }

            if (isExistingResource)
            {
                // we can refer to a resource at any scope if it is an existing resource not being deployed by this file
                var scopeReference = LanguageConstants.CreateResourceScopeReference(validParentScopes);
                properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, new TypeProperty(LanguageConstants.ResourceScopePropertyName, scopeReference, scopeRequiredFlag));

                return new NamedObjectType(
                    objectType.Name,
                    objectType.ValidationFlags,
                    ConvertToReadOnly(properties.Values),
                    objectType.AdditionalPropertiesType,
                    ConvertToReadOnly(objectType.AdditionalPropertiesFlags));
            }
            else
            {
                // we only support scope for extension resources (or resources where the scope is unknown and thus may be an extension resource)
                if (validParentScopes.HasFlag(ResourceScope.Resource))
                {
                    var scopeReference = LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource);
                    properties = properties.SetItem(LanguageConstants.ResourceScopePropertyName, new TypeProperty(LanguageConstants.ResourceScopePropertyName, scopeReference, scopeRequiredFlag));
                }

                // TODO: remove 'dependsOn' from the type library
                properties = properties.SetItem(LanguageConstants.ResourceDependsOnPropertyName, new TypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceRefArray, TypePropertyFlags.WriteOnly));

                return new NamedObjectType(
                    objectType.Name,
                    objectType.ValidationFlags,
                    properties.Values,
                    objectType.AdditionalPropertiesType,
                    objectType.AdditionalPropertiesFlags);
            }
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