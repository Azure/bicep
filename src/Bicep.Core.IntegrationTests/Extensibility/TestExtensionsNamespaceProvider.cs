// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;

namespace Bicep.Core.IntegrationTests.Extensibility;

public class TestExtensionsNamespaceProvider : NamespaceProvider
{
    public delegate NamespaceType? NamespaceTypeCreator(string extensionName, string aliasName);

    public static INamespaceProvider CreateWithDefaults()
        => Create((extensionName, aliasName) => extensionName switch
        {
            FooNamespaceType.BuiltInName => FooNamespaceType.Create(aliasName),
            BarNamespaceType.BuiltInName => BarNamespaceType.Create(aliasName),
            BazNamespaceType.BuiltInName => BazNamespaceType.Create(aliasName),
            _ => null,
        });

    public static INamespaceProvider Create(NamespaceTypeCreator namespaceCreatorFunc)
        => new TestExtensionsNamespaceProvider(BicepTestConstants.ResourceTypeProviderFactory, namespaceCreatorFunc);

    public TestExtensionsNamespaceProvider(
        IResourceTypeProviderFactory resourceTypeProviderFactory,
        NamespaceTypeCreator namespaceCreatorFunc)
        : base(resourceTypeProviderFactory)
    {
        this.namespaceCreatorFunc = namespaceCreatorFunc;
    }

    private readonly NamespaceTypeCreator namespaceCreatorFunc;

    protected override TypeSymbol GetNamespaceTypeForConfigManagedExtension(BicepSourceFile sourceFile, ResourceScope targetScope, ArtifactResolutionInfo? artifact, ExtensionDeclarationSyntax? syntax, string extensionName)
    {
        var aliasName = syntax?.TryGetSymbolName() ?? extensionName;
        if (namespaceCreatorFunc(extensionName, aliasName) is { } namespaceType)
        {
            return namespaceType;
        }

        return base.GetNamespaceTypeForConfigManagedExtension(sourceFile, targetScope, artifact, syntax, extensionName);
    }
}
