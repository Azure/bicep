// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.PublicRegistry;

public record RegistryModule(
    string Name, // e.g. "avm/app/dapr-containerapp" (note: the actual repo name has "bicep/" at the beginning, but the "public" alias takes care of that)
    string? Description,
    string? DocumentationUri);

public record RegistryModuleVersion(
    string Version,
    string? Description,
    string? DocumentationUri);

public interface IPublicRegistryModuleMetadataProvider
{
    public bool IsCached { get; }
    public string? DownloadError { get; }

    public Task TryAwaitCache(bool forceUpdate = false);
    public void StartUpdateCache(bool forceUpdate = false);

    RegistryModule[] GetCachedModules();
    RegistryModuleVersion[] GetCachedModuleVersions(string modulePath);
}
