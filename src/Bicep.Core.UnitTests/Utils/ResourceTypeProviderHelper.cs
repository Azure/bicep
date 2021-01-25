// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ResourceTypeProviderHelper
    {
        private class MockResourceTypeProvider : IResourceTypeProvider
        {
            private readonly ImmutableDictionary<ResourceTypeReference, ResourceType> typeDictionary;

            public MockResourceTypeProvider(IEnumerable<ResourceType> definedTypes)
            {
                typeDictionary = definedTypes.ToImmutableDictionary(
                    x => x.TypeReference,
                    x => x,
                    ResourceTypeReferenceComparer.Instance);
            }

            public IEnumerable<ResourceTypeReference> GetAvailableTypes(ResourceScope scopeType)
                => typeDictionary.Keys;

            public ResourceType GetType(ResourceScope scopeType, ResourceTypeReference reference)
                => typeDictionary[reference];

            public bool HasType(ResourceScope scopeType, ResourceTypeReference typeReference)
                => typeDictionary.ContainsKey(typeReference);
        }

        public static IResourceTypeProvider CreateMockTypeProvider(IEnumerable<ResourceType> definedTypes)
            => new MockResourceTypeProvider(definedTypes);

        public static ResourceType CreateCustomResourceType(string fullyQualifiedType, string apiVersion, TypeSymbolValidationFlags validationFlags, params TypeProperty[] customProperties)
        {
            var reference = ResourceTypeReference.Parse($"{fullyQualifiedType}@{apiVersion}");

            var resourceProperties = LanguageConstants.GetCommonResourceProperties(reference)
                .Concat(new TypeProperty("properties", new NamedObjectType("properties", validationFlags, customProperties, null), TypePropertyFlags.Required));

            return new ResourceType(reference, new NamedObjectType(reference.FormatName(), validationFlags, resourceProperties, null));
        }
    }
}