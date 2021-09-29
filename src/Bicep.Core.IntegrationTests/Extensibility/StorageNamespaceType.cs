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
    public static class StorageNamespaceType
    {
        public const string BuiltInName = "storage";

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: false,
            BicepProviderName: BuiltInName,
            ConfigurationType: GetConfigurationType(),
            ArmTemplateProviderName: "AzureStorage",
            ArmTemplateProviderVersion: "1.0");

        private static ObjectType GetConfigurationType()
        {
            return new ObjectType("configuration", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("connectionString", LanguageConstants.String, TypePropertyFlags.Required),
            }, null);
        }

        private class StorageTypeProvider : IResourceTypeProvider
        {
            private readonly ImmutableDictionary<ResourceTypeReference, ResourceType> resourceTypes = new [] {
                new ResourceType(
                    ResourceTypeReference.Parse("AzureStorage/service@2020-01-01"),
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                    new ObjectType("Service properties", TypeSymbolValidationFlags.Default, new[]
                    {
                        new TypeProperty("staticWebsiteEnabled", LanguageConstants.Bool),
                        new TypeProperty("staticWebsiteIndexDocument", LanguageConstants.String),
                        new TypeProperty("staticWebsiteErrorDocument404Path", LanguageConstants.String),
                    }, null)),
                new ResourceType(
                    ResourceTypeReference.Parse("AzureStorage/containers@2020-01-01"),
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                    new ObjectType("Container properties", TypeSymbolValidationFlags.Default, new[]
                    {
                        new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                    }, null)),
                new ResourceType(
                    ResourceTypeReference.Parse("AzureStorage/blobs@2020-01-01"),
                    ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                    new ObjectType("Blob properties", TypeSymbolValidationFlags.Default, new[]
                    {
                        new TypeProperty("containerName", LanguageConstants.String, TypePropertyFlags.Required),
                        new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                        new TypeProperty("base64Content", LanguageConstants.String, TypePropertyFlags.Required),
                    }, null)),
            }.ToImmutableDictionary(x => x.TypeReference, ResourceTypeReferenceComparer.Instance);

            public ResourceType? TryGenerateDefaultType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
                => null;

            public ResourceType? TryGetDefinedType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
            {
                return resourceTypes.TryGetValue(reference);
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
                new StorageTypeProvider());
        }
    }
}