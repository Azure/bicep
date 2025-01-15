// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Oci;
using Microsoft.Win32;

namespace Bicep.Core.Registry.Indexing;

public readonly record struct RegistryModuleMetadata( //asdfg better name?   asdfg combine
    string Registry, // e.g. "mcr.microsoft.com"
    string ModuleName, // e.g. "bicep/avm/app/dapr-containerapp"
    string? Description,//asdfgf
    string? DocumentationUri/*asdfg*/);

public readonly record struct RegistryModuleVersionMetadata(
    string Version,
    string? Description,
    string? DocumentationUri);

/// <summary>
/// Retrieves metadata about modules from a specific OCI registry (public or private).
/// Use IRegistryIndexer to retrieve an instance for a specific registry.
/// </summary>
public interface IRegistryModuleMetadataProvider
{
    public string Registry { get; }

    bool IsCached { get; }

    string? DownloadError { get; }

    Task TryAwaitCache(bool forceUpdate = false);

    void StartUpdateCache(bool forceUpdate = false);

    Task<ImmutableArray<RegistryModuleMetadata>> TryGetModulesAsync();

    Task<ModuleMetadata> TryGetModuleMetadataAsync(string modulePath);

    Task<ImmutableArray<string>> TryGetModuleVersionsAsync(string modulePath);

    Task<RegistryModuleVersionMetadata?> TryGetModuleVersionMetadataAsync(string modulePath, string version); //asdfg not null?

    ImmutableArray<RegistryModuleMetadata> GetCachedModules();

    ImmutableArray<RegistryModuleVersionMetadata> GetCachedModuleVersions(string modulePath);
}
