// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.IntegrationTests.Extensibility;

public class TestExtensibilityNamespaceProvider : INamespaceProvider
{
    private readonly INamespaceProvider defaultNamespaceProvider;

    public TestExtensibilityNamespaceProvider(IResourceTypeLoaderFactory azResourceTypeLoaderFactory)
    {
        defaultNamespaceProvider = new DefaultNamespaceProvider(azResourceTypeLoaderFactory);
    }

    public IEnumerable<string> AvailableNamespaces => defaultNamespaceProvider.AvailableNamespaces.Concat(new[] {
        FooNamespaceType.BuiltInName,
        BarNamespaceType.BuiltInName,
    });

    public NamespaceType? TryGetNamespace(
        string providerName,
        string aliasName,
        ResourceScope resourceScope,
        IFeatureProvider featureProvider,
        BicepSourceFileKind sourceFileKind,
        string? version = null)
    {
        if (defaultNamespaceProvider.TryGetNamespace(providerName, aliasName, resourceScope, featureProvider, sourceFileKind) is { } namespaceType)
        {
            return namespaceType;
        }

        switch (providerName)
        {
            case FooNamespaceType.BuiltInName:
                return FooNamespaceType.Create(aliasName);
            case BarNamespaceType.BuiltInName:
                return BarNamespaceType.Create(aliasName);
        }

        return default;
    }
}
