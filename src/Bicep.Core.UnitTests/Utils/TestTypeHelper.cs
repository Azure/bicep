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
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.Providers;
using Moq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TestTypeHelper
    {
        private class TestResourceTypeLoader : IAzResourceTypeLoader
        {
            private readonly ImmutableDictionary<ResourceTypeReference, ResourceType> resourceTypes;

            public TestResourceTypeLoader(IEnumerable<ResourceType> resourceTypes)
            {
                this.resourceTypes = resourceTypes.ToImmutableDictionary(
                    x => x.TypeReference,
                    x => x,
                    ResourceTypeReferenceComparer.Instance);
            }

            public ResourceType LoadType(ResourceTypeReference reference)
                => resourceTypes[reference];

            public IEnumerable<ResourceTypeReference> GetAvailableTypes()
                => resourceTypes.Keys;
        }

        public static IAzResourceTypeLoader CreateEmptyAzResourceTypeLoader()
            => new TestResourceTypeLoader(Enumerable.Empty<ResourceType>());

        public static IAzResourceTypeLoader CreateAzResourceTypeLoaderWithTypes(IEnumerable<ResourceType> resourceTypes)
            => new TestResourceTypeLoader(resourceTypes);

        public static INamespaceProvider CreateProviderWithTypes(IEnumerable<ResourceType> resourceTypes)
            => new DefaultNamespaceProvider(CreateAzResourceTypeLoaderWithTypes(resourceTypes), BicepTestConstants.Features);

        public static INamespaceProvider CreateEmptyProvider()
            => CreateProviderWithTypes(Enumerable.Empty<ResourceType>());

        public static INamespaceProvider CreateWithAzTypes()
            => new DefaultNamespaceProvider(new AzResourceTypeLoader(), BicepTestConstants.Features);

        public static ResourceType CreateCustomResourceType(string fullyQualifiedType, string apiVersion, TypeSymbolValidationFlags validationFlags, params TypeProperty[] customProperties)
        {
            var reference = ResourceTypeReference.Parse($"{fullyQualifiedType}@{apiVersion}");

            var resourceProperties = LanguageConstants.GetCommonResourceProperties(reference)
                .Concat(new TypeProperty("properties", new ObjectType("properties", validationFlags, customProperties, null), TypePropertyFlags.Required));

            var bodyType = new ObjectType(reference.FormatName(), validationFlags, resourceProperties, null);
            return new ResourceType(reference, ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource, bodyType);
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
