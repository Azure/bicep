// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class AzResourceTypeLoader : IResourceTypeLoader
    {
        private readonly ITypeLoader typeLoader;
        private readonly AzResourceTypeFactory resourceTypeFactory;
        private readonly ImmutableDictionary<ResourceTypeReference, CrossFileTypeReference> availableTypes;
        private readonly ImmutableDictionary<string, ImmutableDictionary<string, ImmutableArray<CrossFileTypeReference>>> availableFunctions;

        public AzResourceTypeLoader(ITypeLoader typeLoader)
        {
            this.typeLoader = typeLoader;
            resourceTypeFactory = new AzResourceTypeFactory();
            var indexedTypes = typeLoader.LoadTypeIndex();
            availableTypes = indexedTypes.Resources.ToImmutableDictionary(
                kvp => ResourceTypeReference.Parse(kvp.Key),
                kvp => kvp.Value);
            availableFunctions = indexedTypes.ResourceFunctions.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToImmutableDictionary(
                    x => x.Key,
                    x => x.Value.ToImmutableArray(),
                    StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<ResourceTypeReference> GetAvailableTypes() => availableTypes.Keys;

        public bool HasType(ResourceTypeReference reference) => availableTypes.ContainsKey(reference);

        public ResourceTypeComponents LoadType(ResourceTypeReference reference)
        {
            var typeLocation = availableTypes[reference];

            if (!availableFunctions.TryGetValue(reference.FormatType(), out var apiFunctions) ||
                reference.ApiVersion is null ||
                !apiFunctions.TryGetValue(reference.ApiVersion, out var functions))
            {
                functions = [];
            }

            var functionOverloads = functions.SelectMany(typeLocation => resourceTypeFactory.GetResourceFunctionOverloads(typeLoader.LoadResourceFunctionType(typeLocation)));

            var serializedResourceType = typeLoader.LoadResourceType(typeLocation);
            return resourceTypeFactory.GetResourceType(serializedResourceType, functionOverloads);
        }
    }
}
