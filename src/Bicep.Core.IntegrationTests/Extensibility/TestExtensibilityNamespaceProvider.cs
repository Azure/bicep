// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
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

    public ResultWithDiagnostic<NamespaceType> TryGetNamespace(
        ResourceTypesProviderDescriptor providerDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider featureProvider,
        BicepSourceFileKind sourceFileKind)
    {
        return providerDescriptor.NamespaceIdentifier switch
        {
            FooNamespaceType.BuiltInName => new(FooNamespaceType.Create(providerDescriptor.Alias)),
            BarNamespaceType.BuiltInName => new(BarNamespaceType.Create(providerDescriptor.Alias)),
            _ => defaultNamespaceProvider.TryGetNamespace(providerDescriptor, resourceScope, featureProvider, sourceFileKind),
        };
    }
}
