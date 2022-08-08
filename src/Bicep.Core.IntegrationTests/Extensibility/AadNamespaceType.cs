// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.Semantics;

namespace Bicep.Core.IntegrationTests.Extensibility
{
    public static class AadNamespaceType
    {
        public const string BuiltInName = "aad";

        public static readonly ImmutableHashSet<string> UniqueIdentifierProperties = new[]
        {
            "uniqueName",
        }.ToImmutableHashSet();

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: null,
            ArmTemplateProviderName: "AAD",
            ArmTemplateProviderVersion: "1.0");

        private class AadTypeProvider : IResourceTypeProvider
        {
            private readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> resourceTypes = new[] {
                new ResourceTypeComponents(
                    ResourceTypeReference.Parse("application"),
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                    ResourceScope.None,
                    ResourceFlags.None,
                    new ObjectType("application", TypeSymbolValidationFlags.Default, new[]
                    {
                        new TypeProperty("uniqueName", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.SystemProperty),
                        new TypeProperty("appId", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    }, null)),
            }.ToImmutableDictionary(x => x.TypeReference, ResourceTypeReferenceComparer.Instance);

            public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
                => null;

            public ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            {
                if (resourceTypes.TryGetValue(reference) is not { } resourceType)
                {
                    return null;
                }

                return new(
                    declaringNamespace,
                    resourceType.TypeReference,
                    resourceType.ValidParentScopes,
                    resourceType.ReadOnlyScopes,
                    resourceType.Flags,
                    resourceType.Body,
                    UniqueIdentifierProperties);
            }

            public bool HasDefinedType(ResourceTypeReference typeReference)
                => resourceTypes.ContainsKey(typeReference);

            public IEnumerable<ResourceTypeReference> GetAvailableTypes()
                => resourceTypes.Keys;
        }

        public static NamespaceType Create(string aliasName)
        {
            return new NamespaceType(
                aliasName,
                Settings,
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new AadTypeProvider());
        }
    }
}
