// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.Workspaces;

namespace Bicep.Core.IntegrationTests.Extensibility;

public class TestExtensibilityNamespaceProvider : INamespaceProvider
{
    public static INamespaceProvider CreateWithDefaults()
        => Create(result => result switch
        {
            { ProviderName: FooNamespaceType.BuiltInName } => result with { Type = FooNamespaceType.Create(result.Name) },
            { ProviderName: BarNamespaceType.BuiltInName } => result with { Type = BarNamespaceType.Create(result.Name) },
            _ => result,
        });

    public static INamespaceProvider Create(Func<NamespaceResult, NamespaceResult> namespaceCreatorFunc)
        => new TestExtensibilityNamespaceProvider(BicepTestConstants.ResourceTypeProviderFactory, namespaceCreatorFunc);

    public TestExtensibilityNamespaceProvider(
        IResourceTypeProviderFactory resourceTypeProviderFactory,
        Func<NamespaceResult, NamespaceResult> namespaceCreatorFunc)
    {
        baseProvider = new NamespaceProvider(resourceTypeProviderFactory);
        this.namespaceCreatorFunc = namespaceCreatorFunc;
    }

    private readonly INamespaceProvider baseProvider;
    private readonly Func<NamespaceResult, NamespaceResult> namespaceCreatorFunc;

    public IEnumerable<NamespaceResult> GetNamespaces(RootConfiguration rootConfig, IFeatureProvider features, IArtifactFileLookup artifactFileLookup, BicepSourceFile sourceFile, ResourceScope targetScope)
    {
        foreach (var result in baseProvider.GetNamespaces(rootConfig, features, artifactFileLookup, sourceFile, targetScope))
        {
            yield return namespaceCreatorFunc(result);
        }
    }
}
