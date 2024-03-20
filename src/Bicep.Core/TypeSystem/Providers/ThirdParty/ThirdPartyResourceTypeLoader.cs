// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;
using ObjectType = Azure.Bicep.Types.Concrete.ObjectType;

namespace Bicep.Core.TypeSystem.Providers.ThirdParty
{
    public class ThirdPartyResourceTypeLoader : IResourceTypeLoader
    {
        public record NamespaceConfiguration(string Name, string Version, bool IsSingleton, TypeSymbol? ConfigurationObject);

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

            return null;
        }

        public NamespaceConfiguration? LoadNamespaceConfiguration()
        {
            if (typeSettings != null)
            {
                var name = typeSettings.Name;
                var version = typeSettings.Version;
                var isSingleton = typeSettings.IsSingleton;

                TypeSymbol? configurationType = null;

                if (typeSettings.ConfigurationType != null)
                {
                    var reference = typeSettings.ConfigurationType;

                    if (typeLoader.LoadType(reference) is not ObjectType objectType)
                    {
                        throw new ArgumentException($"Unable to locate resource object type at index {reference.Index} in \"{reference.RelativePath}\" resource");
                    }

                    var bodyType = resourceTypeFactory.GetObjectType(objectType);

                    configurationType = bodyType;
                }

                return new NamespaceConfiguration(name, version, isSingleton, configurationType);
            }

            return null;
        }
    }
}
