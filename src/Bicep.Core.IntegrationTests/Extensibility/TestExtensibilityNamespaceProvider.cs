// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.Workspaces;

namespace Bicep.Core.IntegrationTests.Extensibility;

public class TestExtensibilityNamespaceProvider : NamespaceProvider
{
    public delegate NamespaceType? NamespaceTypeCreator(string providerName, string aliasName);

    public static INamespaceProvider CreateWithDefaults()
        => Create((providerName, aliasName) => providerName switch
        {
            FooNamespaceType.BuiltInName => FooNamespaceType.Create(aliasName),
            BarNamespaceType.BuiltInName => BarNamespaceType.Create(aliasName),
            _ => null,
        });

    public static INamespaceProvider Create(NamespaceTypeCreator namespaceCreatorFunc)
        => new TestExtensibilityNamespaceProvider(BicepTestConstants.ResourceTypeProviderFactory, namespaceCreatorFunc);

    public TestExtensibilityNamespaceProvider(
        IResourceTypeProviderFactory resourceTypeProviderFactory,
        NamespaceTypeCreator namespaceCreatorFunc)
        : base(resourceTypeProviderFactory)
    {
        this.namespaceCreatorFunc = namespaceCreatorFunc;
    }

    private readonly NamespaceTypeCreator namespaceCreatorFunc;

    protected override TypeSymbol GetNamespaceTypeForConfigManagedProvider(RootConfiguration rootConfig, IFeatureProvider features, BicepSourceFile sourceFile, ResourceScope targetScope, ArtifactResolutionInfo? artifact, ProviderDeclarationSyntax? syntax, string providerName)
    {
        var aliasName = syntax?.Alias?.IdentifierName ?? providerName;
        if (namespaceCreatorFunc(providerName, aliasName) is { } namespaceType)
        {
            return namespaceType;
        }

        return base.GetNamespaceTypeForConfigManagedProvider(rootConfig, features, sourceFile, targetScope, artifact, syntax, providerName);
    }
}
