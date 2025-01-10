// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Core.Registry.PublicRegistry;

/// <summary>
/// Retrieves module metadata from OCI registries (public or private)
/// </summary>
public interface IRegistryIndexer
{
    IRegistryModuleMetadataProvider GetRegistry(string registry, CloudConfiguration cloud);
}
