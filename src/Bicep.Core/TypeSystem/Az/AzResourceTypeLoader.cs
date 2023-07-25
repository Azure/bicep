// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;

namespace Bicep.Core.TypeSystem.Az
{
    public class AzResourceTypeLoader : IAzResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly AzResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, TypeLocation> availableTypes;
        private readonly ImmutableDictionary<string, ImmutableDictionary<string, ImmutableArray<TypeLocation>>> availableFunctions;

        private static TypeLoader defaultAzTypeLoader = new AzTypeLoader();

        public AzResourceTypeLoader() : this(AzResourceTypeLoader.defaultAzTypeLoader) { }

        public AzResourceTypeLoader(TypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            this.resourceTypeFactory = new AzResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            this.availableTypes = indexedTypes.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);
            this.availableFunctions = indexedTypes.Functions.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToImmutableDictionary(
                    x => x.Key,
                    x => x.Value.ToImmutableArray(),
                    StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => availableTypes.Keys;

        public bool HasType(ResourceTypeReference reference)
            => availableTypes.ContainsKey(reference);
        
        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            if (!availableFunctions.TryGetValue(reference.FormatType(), out var apiFunctions) ||
                reference.ApiVersion is null ||
                !apiFunctions.TryGetValue(reference.ApiVersion, out var functions))
            {
                functions = ImmutableArray<TypeLocation>.Empty;
            }

            var functionOverloads = functions.SelectMany(typeLocation => resourceTypeFactory.GetResourceFunctionOverloads(typeLoader.LoadResourceFunctionType(typeLocation)));

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
            return resourceTypeFactory.GetResourceType(serializedResourceType, functionOverloads);
        }
    }
}
