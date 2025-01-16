// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Oci;
using Microsoft.Win32;
using static Bicep.Core.Registry.Indexing.IRegistryModuleMetadata;

namespace Bicep.Core.Registry.Indexing;

//asdfg move?
public interface IRegistryModuleMetadata
{ //asdfg better name?   asdfg combine    asdfg interface?
    public string Registry { get; init; } // e.g. "mcr.microsoft.com"
    public string ModuleName { get; init; } // e.g. "bicep/avm/app/dapr-containerapp"

    public Task<RegistryMetadataDetails> TryGetDetailsAsync();

    public Task<ImmutableArray<RegistryModuleVersionMetadata>> TryGetVersionsAsync();

    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedVersions();
}

//asdfg move?
public readonly record struct RegistryMetadataDetails(
    string? Description,
    string? DocumentationUri);

//asdfg move?
public readonly record struct RegistryModuleVersionMetadata(
    string Version,
    RegistryMetadataDetails Details
);

/// <summary>
/// Retrieves metadata about modules from a specific OCI registry (public or private).
/// Use IRegistryIndexer to retrieve an instance for a specific registry.
/// </summary>
public interface IRegistryModuleMetadataProvider //asdfg rename?  see GetRegistry()
{
    public string Registry { get; }

    bool IsCached { get; }

    string? DownloadError { get; }

    Task TryAwaitCache(bool forceUpdate = false);

    void StartCache(bool forceUpdate = false);

    Task<ImmutableArray<IRegistryModuleMetadata>> TryGetModulesAsync();




    //asdfg Task<ModuleMetadata> TryGetModuleMetadataAsync(string modulePath);

    //asdfg Task<ImmutableArray<RegistryModuleVersionMetadata>> TryGetModuleVersionsAsync(string modulePath);

    //asdfg Task<RegistryModuleVersionMetadata?> TryGetModuleVersionMetadataAsync(string modulePath, string version); //asdfg not null?

    ImmutableArray<IRegistryModuleMetadata> GetCachedModules();

    //asdfg ImmutableArray<RegistryModuleVersionMetadata> GetCachedModuleVersions(string modulePath);
}

