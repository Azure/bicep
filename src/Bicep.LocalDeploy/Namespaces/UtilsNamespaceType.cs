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

public static class UtilsNamespaceType
{
    public const string BuiltInName = "utils";

    public static readonly ImmutableHashSet<string> UniqueIdentifierProperties = ImmutableHashSet<string>.Empty;

    public static NamespaceSettings Settings { get; } = new(
        IsSingleton: true,
        BicepProviderName: BuiltInName,
        ConfigurationType: null,
        ArmTemplateProviderName: "Utils",
        ArmTemplateProviderVersion: "0.0.1");

    private static DiscriminatedObjectType GetScriptBodyType()
    {
        var commonProps = new[] {
            new TypeProperty("script", LanguageConstants.String, TypePropertyFlags.WriteOnly),
            new TypeProperty("exitCode", LanguageConstants.Int, TypePropertyFlags.ReadOnly),
            new TypeProperty("stdout", LanguageConstants.String, TypePropertyFlags.ReadOnly),
            new TypeProperty("stderr", LanguageConstants.String, TypePropertyFlags.ReadOnly),
        };

        var bashOptions = new ObjectType("BashScript", TypeSymbolValidationFlags.Default, new[]
        {
            new TypeProperty("type", TypeFactory.CreateStringLiteralType("bash")),
        }.Concat(commonProps), null);

        var pwshOptions = new ObjectType("PowerShellScript", TypeSymbolValidationFlags.Default, new[]
        {
            new TypeProperty("type", TypeFactory.CreateStringLiteralType("powershell")),
        }.Concat(commonProps), null);

        return new DiscriminatedObjectType("ScriptBody", TypeSymbolValidationFlags.Default, "type", new[] {
            bashOptions,
            pwshOptions,
        });
    }

    private class TypeProvider : ResourceTypeProviderBase, IResourceTypeProvider
    {
        private static readonly ImmutableDictionary<ResourceTypeReference, ResourceTypeComponents> ResourceTypes = new[] {
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("Wait"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("body", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("durationMs", LanguageConstants.Int, TypePropertyFlags.WriteOnly),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("Assert"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("body", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                    new TypeProperty("condition", LanguageConstants.Bool, TypePropertyFlags.WriteOnly),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("Log"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                new ObjectType("body", TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("message", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                }, null)),
            new ResourceTypeComponents(
                ResourceTypeReference.Parse("Script"),
                ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                GetScriptBodyType()),
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