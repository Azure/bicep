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

public static class BarNamespaceType
{
    public const string BuiltInName = "bar";

    public static readonly ImmutableHashSet<string> UniqueIdentifierProperties =
    [
        "name",
    ];

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: false,
        BicepProviderName: BuiltInName,
        ConfigurationType: GetConfigurationType(),
        ArmTemplateProviderName: "Bar",
        ArmTemplateProviderVersion: "0.0.1");

    private static ObjectType GetConfigurationType()
    {
        return new ObjectType("configuration", TypeSymbolValidationFlags.Default, new[]
        {
            new TypeProperty("connectionString", LanguageConstants.String, TypePropertyFlags.Required),
        }, null);
    }

    private class BarTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private static readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> ResourceTypes = new[] {
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("service"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("Service properties", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("staticWebsiteEnabled", LanguageConstants.Bool),
                    new TypeProperty("staticWebsiteIndexDocument", LanguageConstants.String),
                    new TypeProperty("staticWebsiteErrorDocument404Path", LanguageConstants.String),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("container"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("Container properties", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("blob"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("Blob properties", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("containerName", LanguageConstants.String, TypePropertyFlags.Required),
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                    new TypeProperty("base64Content", LanguageConstants.String, TypePropertyFlags.Required),
                }, null)),
        }.ToImmutableDictionary(x => x.TypeReference);

        public BarTypeProvider()
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

        public string Version { get; } = "0.0.1";
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
            new BarTypeProvider());
    }
}
