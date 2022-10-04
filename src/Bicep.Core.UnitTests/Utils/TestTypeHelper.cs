// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TestTypeHelper
    {
        private class TestResourceTypeLoader : IAzResourceTypeLoader
        {
            private readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> resourceTypes;

            public TestResourceTypeLoader(IEnumerable<ResourceTypeComponents> resourceTypes)
            {
                this.resourceTypes = resourceTypes.ToImmutableDictionary(
                    x => x.TypeReference,
                    x => x,
                    ResourceTypeReferenceComparer.Instance);
            }

            public ResourceTypeComponents LoadType(ResourceTypeReference reference)
                => resourceTypes[reference];

            public IEnumerable<ResourceTypeReference> GetAvailableTypes()
                => resourceTypes.Keys;
        }

        public static IAzResourceTypeLoader CreateEmptyAzResourceTypeLoader()
            => new TestResourceTypeLoader(Enumerable.Empty<ResourceTypeComponents>());

        public static IAzResourceTypeLoader CreateAzResourceTypeLoaderWithTypes(IEnumerable<ResourceTypeComponents> resourceTypes)
            => new TestResourceTypeLoader(resourceTypes);

        public static INamespaceProvider CreateEmptyProvider()
            => new DefaultNamespaceProvider(CreateAzResourceTypeLoaderWithTypes(Enumerable.Empty<ResourceTypeComponents>()));

        public static ResourceTypeComponents CreateCustomResourceType(string fullyQualifiedType, string apiVersion, TypeSymbolValidationFlags validationFlags, params TypeProperty[] customProperties)
            => CreateCustomResourceTypeWithTopLevelProperties(fullyQualifiedType, apiVersion, validationFlags, null, customProperties);

        public static ResourceTypeComponents CreateCustomResourceType(
            string fullyQualifiedType,
            string apiVersion,
            TypeSymbolValidationFlags validationFlags,
            ResourceScope scopes,
            ResourceScope readOnlyScopes,
            ResourceFlags flags,
            params TypeProperty[] customProperties
        ) => CreateCustomResourceTypeWithTopLevelProperties(fullyQualifiedType, apiVersion, validationFlags, null, scopes, readOnlyScopes, flags, customProperties);

        public static ResourceTypeComponents CreateCustomResourceTypeWithTopLevelProperties(string fullyQualifiedType, string apiVersion, TypeSymbolValidationFlags validationFlags, IEnumerable<TypeProperty>? additionalTopLevelProperties = null, params TypeProperty[] customProperties)
            => CreateCustomResourceTypeWithTopLevelProperties(
                fullyQualifiedType,
                apiVersion,
                validationFlags,
                additionalTopLevelProperties,
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource,
                ResourceScope.None,
                ResourceFlags.None,
                customProperties);

        public static ResourceTypeComponents CreateCustomResourceTypeWithTopLevelProperties(
            string fullyQualifiedType,
            string apiVersion,
            TypeSymbolValidationFlags validationFlags,
            IEnumerable<TypeProperty>? additionalTopLevelProperties,
            ResourceScope scopes,
            ResourceScope readOnlyScopes,
            ResourceFlags flags,
            params TypeProperty[] customProperties
        )
        {
            var reference = ResourceTypeReference.Parse($"{fullyQualifiedType}@{apiVersion}");

            var resourceProperties = AzResourceTypeProvider.GetCommonResourceProperties(reference)
                .Concat(additionalTopLevelProperties ?? Enumerable.Empty<TypeProperty>())
                .Concat(new TypeProperty("properties", new ObjectType("properties", validationFlags, customProperties, null), TypePropertyFlags.None));

            var bodyType = new ObjectType(reference.FormatName(), validationFlags, resourceProperties, null);
            return new ResourceTypeComponents(reference, scopes, readOnlyScopes, flags, bodyType);
        }

        public static ObjectType CreateObjectType(string name, params (string name, ITypeReference type)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type)),
                null,
                TypePropertyFlags.None);
        public static ObjectType CreateObjectType(string name, params (string name, ITypeReference type, TypePropertyFlags flags)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type, val.flags)),
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
