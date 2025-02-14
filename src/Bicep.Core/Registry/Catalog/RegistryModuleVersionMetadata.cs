// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Catalog;

public record RegistryModuleVersionMetadata(
    string Version,
    bool? IsBicepModule, // null = unknown
    RegistryMetadataDetails Details
)
{
    public static RegistryModuleVersionMetadata UnexpectedArtifactType(string version, string actualArtifactType)
        => new(version, false, new($"Not a valid Bicep module. Found artifact type {actualArtifactType}", null));

    public static RegistryModuleVersionMetadata InvalidModule(string version, Exception ex)
        => new(version, false, new($"Not a valid Bicep module. {ex.Message}", null));

    public static RegistryModuleVersionMetadata DownloadError(string version, Exception ex)
        => new(version, false, new(ex.Message, null));
}
