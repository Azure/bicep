// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.K8s
{
    public class K8sResourceTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        public const string MetadataPropertyName = "metadata";
        public const string NamePropertyName = "name";
        public const string NamespaceProperty = "namespace";

        public static readonly TypeSymbol Tags = new ObjectType(nameof(Tags), TypeSymbolValidationFlags.Default, [], new TypeProperty(LanguageConstants.String));

        private readonly K8sResourceTypeLoader resourceTypeLoader;
        private readonly ResourceTypeCache definedTypeCache;
        private readonly ResourceTypeCache generatedTypeCache;

        public static readonly FrozenSet<string> UniqueIdentifierProperties = FrozenSet.ToFrozenSet(
        [
            MetadataPropertyName,
        ]);

        public K8sResourceTypeProvider(K8sResourceTypeLoader resourceTypeLoader)
            : base([.. resourceTypeLoader.GetAvailableTypes()])
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
                    bodyType = SetBicepResourceProperties(bodyObjectType, resourceType.ValidParentScopes, resourceType.TypeReference, flags);
                    break;
                default:
                    // we exhaustively test deserialization of every resource type during CI, and this happens in a deterministic fashion,
                    // so this exception should never occur in the released product
                    throw new ArgumentException($"Resource {resourceType.TypeReference.FormatName()} has unexpected body type {bodyType.GetType()}");
            }

            return resourceType with { Body = bodyType };
        }

        private static ObjectType SetBicepResourceProperties(ObjectType objectType, ResourceScope validParentScopes, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            // Local function.
            static NamedTypeProperty UpdateFlags(NamedTypeProperty typeProperty, TypePropertyFlags flags) =>
                new(typeProperty.Name, typeProperty.TypeReference, flags, typeProperty.Description);

            var properties = objectType.Properties;
            var isExistingResource = flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource);

            if (!isExistingResource)
            {
                // TODO: remove 'dependsOn' from the type library
                properties = properties.SetItem(LanguageConstants.ResourceDependsOnPropertyName, new NamedTypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny));
            }

            // add the loop variant flag to the name property (if it exists)
            if (properties.TryGetValue(MetadataPropertyName, out var metadataProperty))
            {
                // TODO apply this to all unique properties
                properties = properties.SetItem(MetadataPropertyName, UpdateFlags(metadataProperty, metadataProperty.Flags | TypePropertyFlags.LoopVariant));
            }

            return new ObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                isExistingResource && objectType.AdditionalProperties is not null
                    ? objectType.AdditionalProperties with { Flags = ConvertToReadOnly(objectType.AdditionalProperties.Flags) }
                    : objectType.AdditionalProperties,
                functions: null);
        }

        private static IEnumerable<NamedTypeProperty> ConvertToReadOnly(IEnumerable<NamedTypeProperty> properties)
        {
            foreach (var property in properties)
            {
                // "name", "scope" & "parent" can be set for existing resources - everything else should be read-only
                // existing Kubernetes resources can also declare "metadata.name" and "metadata.namespace"
                if (property.Name == MetadataPropertyName && property.TypeReference.Type is ObjectType metadataType)
                {
                    var updatedProperties = new List<NamedTypeProperty>();

                    foreach (var metadataProperty in metadataType.Properties.Values)
                    {
                        if (metadataProperty.Name == NamePropertyName || metadataProperty.Name == NamespaceProperty)
                        {
                            updatedProperties.Add(metadataProperty);
                        }
                        else
                        {
                            updatedProperties.Add(new NamedTypeProperty(metadataProperty.Name, metadataProperty.TypeReference, ConvertToReadOnly(metadataProperty.Flags), metadataProperty.Description));
                        }
                    }

                    var updatedMetadataType = new ObjectType(
                        metadataType.Name,
                        metadataType.ValidationFlags,
                        updatedProperties,
                        metadataType.AdditionalProperties is not null ? metadataType.AdditionalProperties with { Flags = ConvertToReadOnly(metadataType.AdditionalProperties.Flags) } : null,
                        functions: null);

                    yield return new NamedTypeProperty(property.Name, updatedMetadataType, property.Flags, property.Description);
                }
                else
                {
                    yield return new NamedTypeProperty(property.Name, property.TypeReference, ConvertToReadOnly(property.Flags), property.Description);
                }
            }
        }

        private static TypePropertyFlags ConvertToReadOnly(TypePropertyFlags typePropertyFlags)
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
                UniqueIdentifierProperties);
        }

        public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            var resourceType = generatedTypeCache.GetOrAdd(flags, typeReference, () =>
            {
                var resourceType = new ResourceTypeComponents(
                    typeReference,
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource,
                    ResourceScope.None,
                    ResourceFlags.None,
                    new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, [], new TypeProperty(LanguageConstants.Any)));

                return SetBicepResourceProperties(resourceType, flags);
            });

            return new(
                declaringNamespace,
                resourceType.TypeReference,
                resourceType.ValidParentScopes,
                resourceType.ReadOnlyScopes,
                resourceType.Flags,
                resourceType.Body,
                UniqueIdentifierProperties);
        }

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => availableResourceTypes.Contains(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes;
    }
}
