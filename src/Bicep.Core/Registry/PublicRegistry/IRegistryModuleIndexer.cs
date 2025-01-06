// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.Win32;

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

public interface IRegistryModuleMetadataProvider //asdfg?           asdfg rename IOci...?         rename so it's clear it's intended to have multiple service implementations?
{
    bool IsCached { get; }

    string? DownloadError { get; }

    Task TryAwaitCache(bool forceUpdate = false);

    void StartUpdateCache(bool forceUpdate = false);

    Task<ImmutableArray<RegistryModuleMetadata>> GetModulesAsync(); //asdfg or not Task??  Was originally not

    Task<ImmutableArray<RegistryModuleVersionMetadata>> GetModuleVersionsAsync(string modulePath);
}
