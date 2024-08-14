// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.ThirdParty
{
    public class ThirdPartyResourceTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        public static readonly TypeSymbol Tags = new ObjectType(nameof(Tags), TypeSymbolValidationFlags.Default, [], LanguageConstants.String, TypePropertyFlags.None);

        private readonly ThirdPartyResourceTypeLoader resourceTypeLoader;
        private readonly ResourceTypeCache definedTypeCache;
        private readonly ResourceTypeCache generatedTypeCache;

        public ThirdPartyResourceTypeProvider(ThirdPartyResourceTypeLoader resourceTypeLoader)
            : base(resourceTypeLoader.GetAvailableTypes().ToImmutableHashSet())
        {
            this.resourceTypeLoader = resourceTypeLoader;
            definedTypeCache = new ResourceTypeCache();
            generatedTypeCache = new ResourceTypeCache();
        }

        private static ResourceTypeComponents SetBicepResourceProperties(ResourceTypeComponents resourceType, ResourceTypeGenerationFlags flags)
        {
            var bodyType = resourceType.Body.Type;

            switch (bodyType)
            {
                case ObjectType bodyObjectType:
                    bodyType = SetBicepResourceProperties(bodyObjectType, flags);
                    break;

                case DiscriminatedObjectType bodyDiscriminatedType:
                    bodyType = SetBicepResourceProperties(bodyDiscriminatedType, flags);
                    break;

                default:
                    throw new ArgumentException($"Resource {resourceType.TypeReference.FormatName()} has unexpected body type {bodyType.GetType()}");
            }

            return resourceType with { Body = bodyType };
        }

        private static ObjectType SetBicepResourceProperties(ObjectType objectType, ResourceTypeGenerationFlags flags)
        {
            var properties = objectType.Properties;
            var isExistingResource = flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource);

            if (!isExistingResource)
            {
                // TODO: Support "dependsOn" for "existing" resources
                properties = properties.Add(LanguageConstants.ResourceDependsOnPropertyName, new TypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny));
            }

            return new ObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                objectType.AdditionalPropertiesType,
                isExistingResource ? ConvertToReadOnly(objectType.AdditionalPropertiesFlags) : objectType.AdditionalPropertiesFlags,
                functions: objectType.MethodResolver.functionOverloads);
        }

        private static DiscriminatedObjectType SetBicepResourceProperties(DiscriminatedObjectType objectType, ResourceTypeGenerationFlags flags)
        {
            var unionMembersByKey = objectType.UnionMembersByKey
                .ToDictionary(x => x.Key, x => SetBicepResourceProperties(x.Value, flags));

            return new DiscriminatedObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                objectType.DiscriminatorKey,
                unionMembersByKey.Values);
        }

        private static IEnumerable<TypeProperty> ConvertToReadOnly(IEnumerable<TypeProperty> properties)
        {
            foreach (var property in properties)
            {
                if (!property.Flags.HasFlag(TypePropertyFlags.ResourceIdentifier))
                {
                    // this property should be read-only for an "existing" resource
                    yield return new TypeProperty(property.Name, property.TypeReference, ConvertToReadOnly(property.Flags), property.Description);
                    continue;
                }

                if (property.TypeReference.Type is ObjectType objectType)
                {
                    // this property is required to identify the resource, but we have to recurse to make non-identifier sub-properties read-only
                    objectType = new ObjectType(
                        objectType.Name,
                        objectType.ValidationFlags,
                        ConvertToReadOnly(objectType.Properties.Values),
                        objectType.AdditionalPropertiesType,
                        ConvertToReadOnly(objectType.AdditionalPropertiesFlags),
                        functions: null);

                    yield return new TypeProperty(property.Name, objectType, property.Flags, property.Description);
                    continue;
                }

                // this property is required to identify the resource and should be left as-is for an "existing" resource
                yield return property;
                continue;
            }
        }

        private static TypePropertyFlags ConvertToReadOnly(TypePropertyFlags typePropertyFlags)
            // Add "ReadOnly" flag and remove "Required" flag
            => (typePropertyFlags | TypePropertyFlags.ReadOnly) & ~TypePropertyFlags.Required;

        public ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            if (!HasDefinedType(typeReference))
            {
                return null;
            }

            // It's important to cache this result because generating the resource type is an expensive operation
            var resourceType = definedTypeCache.GetOrAdd(flags, typeReference, () =>
            {
                var resourceType = resourceTypeLoader.LoadType(typeReference);

                return SetBicepResourceProperties(resourceType, flags);
            });

            return new(
                declaringNamespace,
                resourceType.TypeReference,
                resourceType.ValidParentScopes,
                resourceType.ReadOnlyScopes,
                resourceType.Flags,
                resourceType.Body,
                []);
        }

        public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            var loadedFallbackType = resourceTypeLoader.LoadFallbackResourceType();

            if (loadedFallbackType != null)
            {
                var resourceType = generatedTypeCache.GetOrAdd(flags, typeReference, () =>
                {
                    var resourceType = new ResourceTypeComponents(
                        typeReference,
                        ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource,
                        ResourceScope.None,
                        ResourceFlags.None,
                        loadedFallbackType.Body);

                    return resourceType;
                });

                return new(
                    declaringNamespace,
                    resourceType.TypeReference,
                    resourceType.ValidParentScopes,
                    resourceType.ReadOnlyScopes,
                    resourceType.Flags,
                    resourceType.Body,
                    []);
            }

            return null;
        }

        public ThirdPartyResourceTypeLoader.NamespaceConfiguration? GetNamespaceConfiguration()
        {
            return resourceTypeLoader.LoadNamespaceConfiguration();
        }

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => availableResourceTypes.Contains(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes;
    }
}
