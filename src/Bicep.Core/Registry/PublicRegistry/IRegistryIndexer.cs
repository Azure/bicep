// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.PublicRegistry;

/// <summary>
/// Retrieves module metadata from OCI registries (public or private)
/// </summary>
public interface IRegistryIndexer
{
    IRegistryModuleMetadataProvider GetRegistry(string registry);

    void StartUpCache(bool forceUpdate = false);
}
