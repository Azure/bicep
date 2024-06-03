// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.LocalDeploy.Namespaces;

public static class GithubNamespaceType
{
    public const string BuiltInName = "github";

    public static readonly ImmutableHashSet<string> UniqueIdentifierProperties = ImmutableHashSet<string>.Empty;

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: true,
        BicepProviderName: BuiltInName,
        ConfigurationType: GetConfigurationType(),
        ArmTemplateProviderName: "GitHub",
        ArmTemplateProviderVersion: "0.0.1");

    private static ObjectType GetConfigurationType()
    {
        return new ObjectType("configuration", TypeSymbolValidationFlags.Default, new[]
        {
            new TypeProperty("token", LanguageConstants.String, TypePropertyFlags.Required, "The GitHub access token for authentication."),
        }, null);
    }

    private class TypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private static readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> ResourceTypes = new[] {
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("Repository"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.ReadOnly,
                new ObjectType("body", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("owner", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant),
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("Collaborator"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.ReadOnly,
                new ObjectType("body", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("owner", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant),
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant),
                    new TypeProperty("user", LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant),
                }, null)),
        }.ToImmutableDictionary(x => x.TypeReference);

        public TypeProvider()
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

            if (!flags.HasFlag(ResourceTypeGenerationFlags.ExistingResource))
            {
                // only supporting read-only currently
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

        public string Version { get; } = Settings.ArmTemplateProviderVersion;
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
            new TypeProvider());
    }
}
