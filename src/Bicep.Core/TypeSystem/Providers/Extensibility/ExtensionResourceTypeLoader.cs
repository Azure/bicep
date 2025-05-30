// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Configuration;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Extensibility
{
    public class ExtensionResourceTypeLoader : IResourceTypeLoader
    {
        public record NamespaceConfiguration(string Name, string Version, bool IsSingleton, ObjectLikeType? ConfigurationType);

        private readonly ITypeLoader typeLoader;
        private readonly ExtensionResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;
        private readonly TypeSettings? typeSettings;
        private readonly CrossFileTypeReference? fallbackResourceType;

        public ExtensionResourceTypeLoader(ITypeLoader typeLoader, TypeIndex? typeIndex = null)
        {
            typeIndex ??= typeLoader.LoadTypeIndex();
            this.typeLoader = typeLoader;
            this.resourceTypeFactory = new ExtensionResourceTypeFactory(typeIndex.Settings);
            this.availableTypes = typeIndex.Resources.ToImmutableDictionary(x => ResourceTypeReference.Parse(x.Key), x => x.Value);
            this.typeSettings = typeIndex.Settings;
            this.fallbackResourceType = typeIndex.FallbackResourceType;
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

        public NamespaceConfiguration LoadNamespaceConfiguration()
        {
            if (typeSettings == null)
            {
                throw new ArgumentException($"Please provide the following Settings properties: Name, Version, & IsSingleton.");
            }

            if (typeSettings.ConfigurationType is { } reference)
            {
                var serializedConfigurationType = typeLoader.LoadType(reference);

                if (resourceTypeFactory.GetConfigurationType(serializedConfigurationType) is not ObjectLikeType configurationType)
                {
                    throw new InvalidOperationException($"Extension configuration type at index {reference.Index} in \"{reference.RelativePath}\" is not a valid ObjectLikeType.");
                }

                return new(
                    typeSettings.Name,
                    typeSettings.Version,
                    typeSettings.IsSingleton,
                    configurationType);
            }

            return new(
                typeSettings.Name,
                typeSettings.Version,
                typeSettings.IsSingleton,
                null);
        }
    }
}
