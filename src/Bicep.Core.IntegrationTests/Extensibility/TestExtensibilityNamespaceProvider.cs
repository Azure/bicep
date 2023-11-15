// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Workspaces;

namespace Bicep.Core.IntegrationTests.Extensibility;

public class TestExtensibilityNamespaceProvider : INamespaceProvider
{
    private readonly INamespaceProvider defaultNamespaceProvider;

    public TestExtensibilityNamespaceProvider(IResourceTypeProviderFactory azResourceTypeProviderFactory)
    {
        defaultNamespaceProvider = new DefaultNamespaceProvider(azResourceTypeProviderFactory);
    }

    public IEnumerable<string> AvailableNamespaces => defaultNamespaceProvider.AvailableNamespaces.Concat(new[] {
        FooNamespaceType.BuiltInName,
        BarNamespaceType.BuiltInName,
    });

    public NamespaceType? TryGetNamespace(
        ResourceTypesProviderDescriptor providerDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider featureProvider,
        BicepSourceFileKind sourceFileKind)
    {
        var namespaceType = defaultNamespaceProvider.TryGetNamespace(
           providerDescriptor,
           resourceScope,
           featureProvider,
           sourceFileKind);

        return providerDescriptor.Name switch
        {
            FooNamespaceType.BuiltInName
                => FooNamespaceType.Create(providerDescriptor.Alias),
            BarNamespaceType.BuiltInName
                => BarNamespaceType.Create(providerDescriptor.Alias),
            _
                => namespaceType,
        };
    }
}
