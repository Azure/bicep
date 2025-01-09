// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.Win32;

namespace Bicep.Core.Registry.PublicRegistry;

public readonly record struct RegistryModuleMetadata(
    string Registry, // e.g. "mcr.microsoft.com"  //asdfg needed?
    string ModuleName, // e.g. "bicep/avm/app/dapr-containerapp"
    string? Description,
    string? DocumentationUri);

public readonly record struct RegistryModuleVersionMetadata(
    string Version,
    string? Description,
    string? DocumentationUri);

/// <summary>
/// Retrieves metadata about modules from a specific OCI registry (public or private)
/// </summary>
public interface IRegistryModuleMetadataProvider
{
    bool IsCached { get; }

    string? DownloadError { get; }

    Task TryAwaitCache(bool forceUpdate = false);

    void StartUpdateCache(bool forceUpdate = false);

    Task<ImmutableArray<RegistryModuleMetadata>> GetModulesAsync();

    Task<ImmutableArray<RegistryModuleVersionMetadata>> GetModuleVersionsAsync(string modulePath);

    ImmutableArray<RegistryModuleMetadata> GetCachedModules();

    ImmutableArray<RegistryModuleVersionMetadata> GetCachedModuleVersions(string modulePath);

}
