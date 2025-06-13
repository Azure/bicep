// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.IntegrationTests.Extensibility;

public static class BazNamespaceType
{
    public const string BuiltInName = "baz";

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: true,
        BicepExtensionName: BuiltInName,
        ConfigurationType: GetConfigurationType(),
        TemplateExtensionName: "Baz",
        TemplateExtensionVersion: "0.0.1");

    private static DiscriminatedObjectType GetConfigurationType()
    {
        return new DiscriminatedObjectType("BazConfiguration", TypeSymbolValidationFlags.Default, "kind", new ObjectType[]
        {
            new("BazConfigurationKindOne", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("kind", TypeFactory.CreateStringLiteralType("One"), TypePropertyFlags.Required),
                new NamedTypeProperty("connectionStringOne", LanguageConstants.SecureString, TypePropertyFlags.Required),
            }),
            new("BazConfigurationKindTwo", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("kind", TypeFactory.CreateStringLiteralType("Two"), TypePropertyFlags.Required),
                new NamedTypeProperty("connectionStringTwo", LanguageConstants.SecureString, TypePropertyFlags.Required),
            }),
        });
    }

    private class BazTypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private static readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> ResourceTypes = new[] {
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("service"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("Service properties", TypeSymbolValidationFlags.Default, new[]
                {
                    new NamedTypeProperty("staticWebsiteEnabled", LanguageConstants.Bool),
                    new NamedTypeProperty("staticWebsiteIndexDocument", LanguageConstants.String),
                    new NamedTypeProperty("staticWebsiteErrorDocument404Path", LanguageConstants.String),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("container"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("Container properties", TypeSymbolValidationFlags.Default, new[]
                {
                    new NamedTypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.ResourceIdentifier),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("blob"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("Blob properties", TypeSymbolValidationFlags.Default, new[]
                {
                    new NamedTypeProperty("containerName", LanguageConstants.String, TypePropertyFlags.Required),
                    new NamedTypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.ResourceIdentifier),
                    new NamedTypeProperty("base64Content", LanguageConstants.String, TypePropertyFlags.Required),
                }, null)),
        }.ToImmutableDictionary(x => x.TypeReference);

        public BazTypeProvider()
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
                resourceType.GetUniqueIdentifierPropertyNames());
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
            ImmutableArray<NamedTypeProperty>.Empty,
            ImmutableArray<FunctionOverload>.Empty,
            ImmutableArray<BannedFunction>.Empty,
            ImmutableArray<Decorator>.Empty,
            new BazTypeProvider());
    }
}
