// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.ThirdParty
{
    public class ThirdPartyResourceTypeLoader : IResourceTypeLoader
    {
        public record NamespaceConfiguration(string Name, string Version, bool IsSingleton, ObjectType? ConfigurationObject);

        private readonly ITypeLoader typeLoader;
        private readonly ExtensibilityResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;
        private readonly TypeSettings? typeSettings;
        private readonly CrossFileTypeReference? fallbackResourceType;

        public ThirdPartyResourceTypeLoader(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            resourceTypeFactory = new ExtensibilityResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            availableTypes = indexedTypes.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);

            typeSettings = indexedTypes.Settings;
            fallbackResourceType = indexedTypes.FallbackResourceType;
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);

            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }

        public ResourceTypeComponents? LoadFallbackResourceType()
        {
            if (fallbackResourceType != null)
            {
                var serializedResourceType = typeLoader.LoadResourceType(fallbackResourceType);

                return resourceTypeFactory.GetResourceType(serializedResourceType);
            }

            // No fallback type provided in JSON
            return null;
        }

        public NamespaceConfiguration? LoadNamespaceConfiguration()
        {
            if (typeSettings == null)
            {
                throw new ArgumentException($"Please provide the following Settings properties: Name, Version, & IsSingleton.");
            }

            ObjectType? configurationType = null;
            if (typeSettings.ConfigurationType is { } reference)
            {

                if (typeLoader.LoadType(reference) is not Azure.Bicep.Types.Concrete.ObjectType concreteObjectType ||
                    resourceTypeFactory.GetObjectType(concreteObjectType) is not ObjectType objectType)
                {
                    throw new ArgumentException($"Unable to locate resource object type at index {reference.Index} in \"{reference.RelativePath}\" resource");
                }

                configurationType = objectType;
            }

            return new(
                typeSettings.Name,
                typeSettings.Version,
                typeSettings.IsSingleton,
                configurationType);
        }
    }
}
