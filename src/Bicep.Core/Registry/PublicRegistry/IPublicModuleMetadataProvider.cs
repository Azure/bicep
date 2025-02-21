// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.PublicRegistry;

public readonly record struct PublicModuleMetadata(
    string Name, // e.g. "avm/app/dapr-containerapp" (note: the actual repo name has "bicep/" at the beginning, but the "public" alias takes care of that)
    string? Description,
    string? DocumentationUri);

public readonly record struct PublicModuleVersionMetadata(
    string Version,
    string? Description,
    string? DocumentationUri);

public interface IPublicModuleMetadataProvider
{
    public bool IsCached { get; }

    public string? DownloadError { get; }

    public Task TryAwaitCache(bool forceUpdate = false);

    public void StartUpdateCache(bool forceUpdate = false);

    ImmutableArray<PublicModuleMetadata> GetModulesMetadata();

    ImmutableArray<PublicModuleVersionMetadata> GetModuleVersionsMetadata(string modulePath);
}
