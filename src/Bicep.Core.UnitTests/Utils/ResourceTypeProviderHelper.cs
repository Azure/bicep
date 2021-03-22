// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Bicep.Types.Az;
using Azure.Bicep.Types.Az.Index;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Moq;

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

            public IEnumerable<ResourceTypeReference> GetAvailableTypes()
                => typeDictionary.Keys;

            public ResourceType GetType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
                => AzResourceTypeProvider.SetBicepResourceProperties(typeDictionary[reference], flags);

            public bool HasType(ResourceTypeReference typeReference)
                => typeDictionary.ContainsKey(typeReference);
        }

        public static IResourceTypeProvider CreateMockTypeProvider(IEnumerable<ResourceType> definedTypes)
            => new MockResourceTypeProvider(definedTypes);

        public static ResourceType CreateCustomResourceType(string fullyQualifiedType, string apiVersion, TypeSymbolValidationFlags validationFlags, params TypeProperty[] customProperties)
        {
            var reference = ResourceTypeReference.Parse($"{fullyQualifiedType}@{apiVersion}");

            var resourceProperties = LanguageConstants.GetCommonResourceProperties(reference)
                .Concat(new TypeProperty("properties", new ObjectType("properties", validationFlags, customProperties, null), TypePropertyFlags.Required));

            var bodyType = new ObjectType(reference.FormatName(), validationFlags, resourceProperties, null);
            return new ResourceType(reference, ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource, bodyType);
        }

        public static AzResourceTypeProvider CreateAzResourceTypeProvider(Action<Azure.Bicep.Types.Concrete.TypeFactory> typeFactoryFunc)
        {
            var factory = new Azure.Bicep.Types.Concrete.TypeFactory(Enumerable.Empty<Azure.Bicep.Types.Concrete.TypeBase>());
            typeFactoryFunc(factory);

            var typeDict = new Dictionary<string, TypeLocation>();
            var resourceDict = new Dictionary<TypeLocation, Azure.Bicep.Types.Concrete.ResourceType>();
            foreach (var resourceType in factory.GetTypes().OfType<Azure.Bicep.Types.Concrete.ResourceType>())
            {
                var typeLocation = new TypeLocation();
                typeDict[resourceType.Name] = typeLocation;
                resourceDict[typeLocation] = resourceType;
            }

            var typeLoader = new Mock<ITypeLoader>();
            typeLoader.Setup(x => x.GetIndexedTypes()).Returns(new TypeIndex(typeDict));
            typeLoader.Setup(x => x.LoadResourceType(It.IsAny<TypeLocation>())).Returns<TypeLocation>(typeLocation => resourceDict[typeLocation]);

            return new AzResourceTypeProvider(typeLoader.Object);
        }

        public static ObjectType CreateObjectType(string name, params (string name, ITypeReference type)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type)),
                null,
                TypePropertyFlags.None);

        public static DiscriminatedObjectType CreateDiscriminatedObjectType(string name, string key, params ITypeReference[] members)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                key,
                members);
    }
}