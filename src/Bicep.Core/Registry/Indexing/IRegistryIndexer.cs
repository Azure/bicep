// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Core.Registry.Indexing;

/// <summary>
/// Retrieves module metadata from OCI registries (public or private)
/// </summary>
public interface IRegistryIndexer
{
    // Aways returns an instance, either from cache or newly created
    IRegistryModuleMetadataProvider GetRegistry(string registry, CloudConfiguration cloud);

    IRegistryModuleMetadataProvider? TryGetCachedRegistry(string registry);
}
