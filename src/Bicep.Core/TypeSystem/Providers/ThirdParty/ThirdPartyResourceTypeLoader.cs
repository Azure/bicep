// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Azure.Bicep.Types.K8s;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.K8s;
using Bicep.Core.TypeSystem.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bicep.Core.TypeSystem.Providers.ThirdParty
{
    public class ThirdPartyResourceTypeLoader : IResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        //No need to create a 3rd party factory
        private readonly K8sResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, TypeLocation> availableTypes;
        private readonly ImmutableDictionary<string, ImmutableDictionary<string, ImmutableArray<TypeLocation>>> availableFunctions;

        public ThirdPartyResourceTypeLoader(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            resourceTypeFactory = new K8sResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            availableTypes = indexedTypes.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);
            //Harsh - do we need functions? what are they? 
            availableFunctions = indexedTypes.Functions.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToImmutableDictionary(
                    x => x.Key,
                    x => x.Value.ToImmutableArray(),
                    StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        //Will need to update loadType
        //Harsh - Do we need this function? 
        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);

            return resourceTypeFactory.GetResourceType(serializedResourceType);
        }
    }
}
