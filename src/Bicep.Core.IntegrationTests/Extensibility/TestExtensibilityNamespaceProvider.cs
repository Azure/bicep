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

    public TestExtensibilityNamespaceProvider(IResourceTypeProviderFactory azResourceTypeLoaderFactory)
    {
        defaultNamespaceProvider = new DefaultNamespaceProvider(azResourceTypeLoaderFactory);
    }

    public IEnumerable<string> AvailableNamespaces => defaultNamespaceProvider.AvailableNamespaces.Concat(new[] {
        FooNamespaceType.BuiltInName,
        BarNamespaceType.BuiltInName,
    });

    public NamespaceType? TryGetNamespace(
        TypesProviderDescriptor typesProviderDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider featureProvider,
        BicepSourceFileKind sourceFileKind)
    {

        if (typesProviderDescriptor.Name == AzNamespaceType.BuiltInName && typesProviderDescriptor.Version == "0.0.0")
        {
            typesProviderDescriptor = new(typesProviderDescriptor.Name);
        }

        var namespaceType = defaultNamespaceProvider.TryGetNamespace(
           typesProviderDescriptor,
           resourceScope,
           featureProvider,
           sourceFileKind);


        return typesProviderDescriptor.Name switch
        {
            FooNamespaceType.BuiltInName
                => FooNamespaceType.Create(typesProviderDescriptor.Alias),
            BarNamespaceType.BuiltInName
                => BarNamespaceType.Create(typesProviderDescriptor.Alias),
            _
                => namespaceType,
        };
    }
}
