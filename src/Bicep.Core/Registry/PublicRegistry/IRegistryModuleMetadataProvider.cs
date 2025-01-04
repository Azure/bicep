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

public interface IRegistryModuleMetadataProvider //asdfg?           asdfg rename IOci...?
{
    bool IsCached { get; }

    string? DownloadError { get; }

    Task TryAwaitCache(bool forceUpdate = false);

    void StartUpdateCache(bool forceUpdate = false);

    ImmutableArray<RegistryModuleMetadata> GetModules();

    ImmutableArray<RegistryModuleVersionMetadata> GetModuleVersions(string registry, string modulePath);

    public static ImmutableArray<RegistryModuleMetadata> GetModules(IRegistryModuleMetadataProvider[] providers)
    {
        return [.. providers.SelectMany(x => x.GetModules())];
    }

    public static ImmutableArray<RegistryModuleVersionMetadata> GetModuleVersions(IRegistryModuleMetadataProvider[] providers, string registry, string modulePath)
    {
        return [.. providers.SelectMany(x => x.GetModuleVersions(registry, modulePath))];
    }
}
