// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.TypeSystem;
using Bicep.Core.Registry;

namespace Bicep.Core.Semantics.Namespaces;

public class RegistryAwareNamespaceProvider : INamespaceProvider
{
    private readonly INamespaceProvider baseProvider;
    private readonly OciModuleRegistry moduleRegistry;

    public RegistryAwareNamespaceProvider(DefaultNamespaceProvider baseProvider, IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, IFeatureProvider features)
    {
        this.baseProvider = baseProvider;
        this.moduleRegistry = new(fileResolver, clientFactory, features);
    }

    public IEnumerable<string> AvailableNamespaces
        => baseProvider.AvailableNamespaces;

    public bool AllowImportStatements
        => true;

    public NamespaceType? TryGetNamespace(string providerName, string? providerVersion, string aliasName, ResourceScope resourceScope)
        => baseProvider.TryGetNamespace(providerName, providerVersion, aliasName, resourceScope);
}
