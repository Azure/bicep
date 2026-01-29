// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Types;
using NamedTypeProperties = System.Collections.Immutable.ImmutableSortedDictionary<string, Bicep.Core.TypeSystem.Types.NamedTypeProperty>;

namespace Bicep.Core.TypeSystem.Providers.Extensibility
{
    public class ExtensionResourceTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private readonly ExtensionResourceTypeLoader resourceTypeLoader;
        private readonly ResourceTypeCache definedTypeCache;
        private readonly ResourceTypeCache generatedTypeCache;

        public ExtensionResourceTypeProvider(ExtensionResourceTypeLoader resourceTypeLoader)
            : base([.. resourceTypeLoader.GetAvailableTypes()])
        {
            this.resourceTypeLoader = resourceTypeLoader;
            definedTypeCache = new ResourceTypeCache();
            generatedTypeCache = new ResourceTypeCache();
        }

        private static ResourceTypeComponents SetBicepResourceProperties(NamespaceType namespaceType, ResourceTypeComponents resourceType, ResourceTypeGenerationFlags flags)
        {
            var bodyType = resourceType.Body.Type;

            switch (bodyType)
            {
                case ObjectType bodyObjectType:
                    bodyType = SetBicepResourceProperties(namespaceType, resourceType, bodyObjectType, flags);
                    break;

                case DiscriminatedObjectType bodyDiscriminatedType:
                    bodyType = SetBicepResourceProperties(namespaceType, resourceType, bodyDiscriminatedType, flags);
                    break;

                default:
                    throw new ArgumentException($"Resource {resourceType.TypeReference.FormatName()} has unexpected body type {bodyType.GetType()}");
            }

            return resourceType with { Body = bodyType };
        }

        private static ObjectType SetBicepResourceProperties(NamespaceType namespaceType, ResourceTypeComponents resourceType, ObjectType objectType, ResourceTypeGenerationFlags flags)
        {
            var properties = objectType.Properties;
            var isExistingResource = flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource);

            properties = MicrosoftGraphExtensionCompatibilityManager.PatchResourceProperties(namespaceType, resourceType, properties);

            if (!isExistingResource)
            {
                // TODO: Support "dependsOn" for "existing" resources
                properties = properties.Add(LanguageConstants.ResourceDependsOnPropertyName, new NamedTypeProperty(LanguageConstants.ResourceDependsOnPropertyName, LanguageConstants.ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny));
            }

            foreach (var (propertyName, propertyType) in properties)
            {
                if (propertyType.Flags.HasFlag(TypePropertyFlags.ResourceIdentifier | TypePropertyFlags.Required))
                {
                    // Add LoopVariant flag to required identifier properties.
                    properties = properties.SetItem(propertyName, UpdateFlags(propertyType, propertyType.Flags | TypePropertyFlags.SystemProperty | TypePropertyFlags.LoopVariant));
                }
            }

            return new ObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                isExistingResource ? ConvertToReadOnly(properties.Values) : properties.Values,
                isExistingResource && objectType.AdditionalProperties is not null
                    ? objectType.AdditionalProperties with { Flags = ConvertToReadOnly(objectType.AdditionalProperties.Flags) }
                    : objectType.AdditionalProperties,
                functions: objectType.MethodResolver.functionOverloads);
        }

        private static DiscriminatedObjectType SetBicepResourceProperties(NamespaceType namespaceType, ResourceTypeComponents resourceType, DiscriminatedObjectType objectType, ResourceTypeGenerationFlags flags)
        {
            var unionMembersByKey = objectType.UnionMembersByKey
                .ToDictionary(x => x.Key, x => SetBicepResourceProperties(namespaceType, resourceType, x.Value, flags));

            return new DiscriminatedObjectType(
                objectType.Name,
                objectType.ValidationFlags,
                objectType.DiscriminatorKey,
                unionMembersByKey.Values);
        }

        private static IEnumerable<NamedTypeProperty> ConvertToReadOnly(IEnumerable<NamedTypeProperty> properties)
        {
            foreach (var property in properties)
            {
                if (!property.Flags.HasFlag(TypePropertyFlags.ResourceIdentifier))
                {
                    // this property should be read-only for an "existing" resource
                    yield return new NamedTypeProperty(property.Name, property.TypeReference, ConvertToReadOnly(property.Flags), property.Description);
                    continue;
                }

                if (property.TypeReference.Type is ObjectType objectType)
                {
                    // this property is required to identify the resource, but we have to recurse to make non-identifier sub-properties read-only
                    objectType = new ObjectType(
                        objectType.Name,
                        objectType.ValidationFlags,
                        ConvertToReadOnly(objectType.Properties.Values),
                        objectType.AdditionalProperties is not null ? objectType.AdditionalProperties with { Flags = ConvertToReadOnly(objectType.AdditionalProperties.Flags) } : null,
                        functions: null);

                    yield return new NamedTypeProperty(property.Name, objectType, property.Flags, property.Description);
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

                return SetBicepResourceProperties(declaringNamespace, resourceType, flags);
            });

            return new(
                declaringNamespace,
                resourceType.TypeReference,
                resourceType.ValidParentScopes,
                resourceType.ReadOnlyScopes,
                resourceType.Flags,
                resourceType.Body,
                resourceType.GetUniqueIdentifierPropertyNames());
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

        public ExtensionResourceTypeLoader.NamespaceConfiguration? GetNamespaceConfiguration()
        {
            return resourceTypeLoader.LoadNamespaceConfiguration();
        }

        public NamespaceSettings GetNamespaceSettings()
        {
            var namespaceConfiguration = resourceTypeLoader.LoadNamespaceConfiguration();
            var namespaceSettings = new NamespaceSettings(
                IsSingleton: namespaceConfiguration.IsSingleton,
                BicepExtensionName: namespaceConfiguration.Name,
                ConfigurationType: namespaceConfiguration.ConfigurationType,
                TemplateExtensionName: namespaceConfiguration.Name,
                TemplateExtensionVersion: namespaceConfiguration.Version);

            return MicrosoftGraphExtensionCompatibilityManager.PatchNamespaceSettings(namespaceSettings);
        }

        public IEnumerable<(FunctionOverload overload, BicepSourceFileKind? visibility)> GetFunctionOverloads()
        {
            return resourceTypeLoader.LoadNamespaceFunctions();
        }

        public bool HasDefinedType(ResourceTypeReference typeReference)
            => availableResourceTypes.Contains(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableResourceTypes;

        private static NamedTypeProperty UpdateFlags(NamedTypeProperty typeProperty, TypePropertyFlags flags) => new(typeProperty.Name, typeProperty.TypeReference, flags, typeProperty.Description);

        private static class MicrosoftGraphExtensionCompatibilityManager
        {
            private const string BackwardCompatibleExtensionVersion = "0.1.8-preview";
            private const string UniqueNamePropertyName = "uniqueName";
            private const string NamePropertyName = "name";
            private const string AppIdPropertyName = "appId";

            public static NamedTypeProperties PatchResourceProperties(NamespaceType namespaceType, ResourceTypeComponents resourceType, NamedTypeProperties properties)
            {
                if (!namespaceType.Settings.TemplateExtensionName.Equals(MicrosoftGraphExtensionFacts.TemplateExtensionName, StringComparison.Ordinal))
                {
                    return properties;
                }

                // 0.1.8-preview is the first Microsoft Graph extension version. Flags such as Identifier were not added until later versions.
                if (namespaceType.Settings.TemplateExtensionVersion.Equals(BackwardCompatibleExtensionVersion, StringComparison.Ordinal))
                {
                    if (properties.TryGetValue(UniqueNamePropertyName, out var uniqueNameProperty))
                    {
                        properties = properties.SetItem(UniqueNamePropertyName, UpdateFlags(uniqueNameProperty, uniqueNameProperty.Flags | TypePropertyFlags.ResourceIdentifier));
                    }

                    if (resourceType.TypeReference.Type.Equals("Microsoft.Graph/applications/federatedIdentityCredentials", StringComparison.Ordinal) &&
                        properties.TryGetValue(NamePropertyName, out var nameProperty))
                    {
                        properties = properties.SetItem(NamePropertyName, UpdateFlags(nameProperty, nameProperty.Flags | TypePropertyFlags.ResourceIdentifier));
                    }

                    if (resourceType.TypeReference.Type.Equals("Microsoft.Graph/servicePrincipals", StringComparison.Ordinal) &&
                        properties.TryGetValue(AppIdPropertyName, out var appIdProperty))
                    {
                        properties = properties.SetItem(AppIdPropertyName, UpdateFlags(appIdProperty, appIdProperty.Flags | TypePropertyFlags.ResourceIdentifier));
                    }
                }

                return properties;
            }

            public static NamespaceSettings PatchNamespaceSettings(NamespaceSettings namespaceSettings)
            {
                if (namespaceSettings.TemplateExtensionName.Equals(MicrosoftGraphExtensionFacts.BicepExtensionBetaName) ||
                    namespaceSettings.TemplateExtensionName.Equals(MicrosoftGraphExtensionFacts.BicepExtensionV1Name))
                {
                    return namespaceSettings with
                    {
                        TemplateExtensionName = MicrosoftGraphExtensionFacts.TemplateExtensionName,
                    };
                }

                return namespaceSettings;
            }
        }

    }
}
