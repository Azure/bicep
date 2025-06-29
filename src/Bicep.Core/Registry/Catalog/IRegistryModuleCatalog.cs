// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog.Implementation;

namespace Bicep.Core.Registry.Catalog;

/// <summary>
/// Retrieves module metadata from OCI registries (public or private)
/// </summary>
public interface IRegistryModuleCatalog
{
    // Aways returns a provider instance, either from cache or newly created, from which loading will fail if the registry
    // is not accessible or doesn't exist
    IRegistryModuleMetadataProvider GetProviderForRegistry(CloudConfiguration cloud, string registry);

    IRegistryModuleMetadataProvider? TryGetCachedRegistry(string registry);
}
