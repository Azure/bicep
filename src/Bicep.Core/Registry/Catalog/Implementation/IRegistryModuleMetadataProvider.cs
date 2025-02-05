// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Oci;
using Microsoft.Win32;
using static Bicep.Core.Registry.Catalog.Implementation.IRegistryModuleMetadata;

namespace Bicep.Core.Registry.Catalog.Implementation;

public interface IRegistryModuleMetadata
{
    public string Registry { get; init; } // e.g. "mcr.microsoft.com"
    public string ModuleName { get; init; } // e.g. "bicep/avm/app/dapr-containerapp"

    public Task<RegistryMetadataDetails> TryGetDetailsAsync();

    // In order returned by the registry, generally oldest first
    public Task<ImmutableArray<RegistryModuleVersionMetadata>> TryGetVersionsAsync();

    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedVersions();
}

/// <summary>
/// Retrieves metadata about modules from a specific OCI registry (public or private).
/// Use IRegistryCatalog to retrieve an instance for a specific registry.
/// </summary>
public interface IRegistryModuleMetadataProvider
{
    public string Registry { get; }

    bool IsCached { get; }

    string? DownloadError { get; }

    void StartCache();

    Task TryAwaitCache(bool forceUpdate = false);

    Task<ImmutableArray<IRegistryModuleMetadata>> TryGetModulesAsync();

    ImmutableArray<IRegistryModuleMetadata> GetCachedModules();
}

