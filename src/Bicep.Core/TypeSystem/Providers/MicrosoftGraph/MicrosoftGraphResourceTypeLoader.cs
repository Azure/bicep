// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Index;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;
using static Bicep.Core.TypeSystem.Providers.ThirdParty.ThirdPartyResourceTypeLoader;

namespace Bicep.Core.TypeSystem.Providers.MicrosoftGraph
{
    public class MicrosoftGraphResourceTypeLoader : IResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly MicrosoftGraphResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;
        private readonly TypeSettings? typeSettings;
        private readonly CrossFileTypeReference? fallbackResourceType;

        public MicrosoftGraphResourceTypeLoader(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            resourceTypeFactory = new MicrosoftGraphResourceTypeFactory();
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
