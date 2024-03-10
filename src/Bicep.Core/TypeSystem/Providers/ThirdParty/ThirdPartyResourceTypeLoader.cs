// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Security.Cryptography.Xml;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;
using ObjectType = Azure.Bicep.Types.Concrete.ObjectType;

namespace Bicep.Core.TypeSystem.Providers.ThirdParty
{
    public class ThirdPartyResourceTypeLoader : IResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly ExtensibilityResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;

        public ThirdPartyResourceTypeLoader(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            resourceTypeFactory = new ExtensibilityResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            availableTypes = indexedTypes.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);
            //Where did functions go?

            if (indexedTypes.Settings != null)
            {
                Settings = indexedTypes.Settings;

                if (indexedTypes.Settings.ConfigurationType != null)
                {
                    //Find a way to avoid calling this again
                    var reference = indexedTypes.Settings.ConfigurationType;
                    //this part can be made it's own function
                    if (typeLoader.LoadType(reference) is not ObjectType objectType)
                    {
                        throw new ArgumentException($"Unable to locate resource object type at index {reference.Index} in \"{reference.RelativePath}\" resource");
                    }

                    ConfigurationType = objectType;
                }
            }

            //And this?
            if (indexedTypes.FallbackResourceType != null)
            {
                var serializedResourceType = typeLoader.LoadResourceType(indexedTypes.FallbackResourceType);

                FallbackResourceType = resourceTypeFactory.GetResourceType(serializedResourceType);
            }
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);

            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }

        public TypeSettings? Settings { get; }

        public ResourceTypeComponents? FallbackResourceType { get; }

        public ObjectType? ConfigurationType { get; }
    }
}
