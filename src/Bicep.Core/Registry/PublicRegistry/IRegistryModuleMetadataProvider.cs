// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.PublicRegistry;

public readonly record struct RegistryModuleMetadata(
    string Registry, // e.g. "mcr.microsoft.com"
    string ModuleName, // e.g. "bicep/avm/app/dapr-containerapp"    asdfg was "avm/app/dapr-containerapp" (asdfg note: the actual repo name has "bicep/" at the beginning, but the "public" alias takes care of that)
    string? Description,
    string? DocumentationUri);

public readonly record struct RegistryModuleVersionMetadata(
    string Version,
    string? Description,
    string? DocumentationUri);

public interface IRegistryModuleMetadataProvider //asdfg?
{
    public bool IsCached { get; }

    public string? DownloadError { get; }

    public Task TryAwaitCache(bool forceUpdate = false);

    public void StartUpdateCache(bool forceUpdate = false);

    ImmutableArray<RegistryModuleMetadata> GetModulesMetadata();

    ImmutableArray<RegistryModuleVersionMetadata> GetModuleVersionsMetadata(string modulePath);
}
