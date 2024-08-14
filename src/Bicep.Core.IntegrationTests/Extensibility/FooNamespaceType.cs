// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.IntegrationTests.Extensibility;

public static class FooNamespaceType
{
    public const string BuiltInName = "foo";

    public static readonly ImmutableHashSet<string> UniqueIdentifierProperties =
    [
        "uniqueName",
    ];

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: true,
        BicepProviderName: BuiltInName,
        ConfigurationType: null,
        ArmTemplateProviderName: "Foo",
        ArmTemplateProviderVersion: "1.2.3");

    private class FooTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private static readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> ResourceTypes = new[] {
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
        }.ToImmutableDictionary(x => x.TypeReference);

        public FooTypeProvider()
            : base(ResourceTypes.Keys.ToImmutableHashSet())
        {
        }

        public ResourceType? TryGenerateFallbackType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            => null;

        public ResourceType? TryGetDefinedType(NamespaceType declaringNamespace, ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
        {
            if (ResourceTypes.TryGetValue(reference) is not { } resourceType)
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
            => ResourceTypes.ContainsKey(typeReference);

        public IEnumerable<ResourceTypeReference> GetAvailableTypes()
            => ResourceTypes.Keys;

        public string Version { get; } = "1.2.3";
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
            new FooTypeProvider());
    }
}
