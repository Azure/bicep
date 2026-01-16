// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.ExtensionHost.Configuration;

/// <summary>
/// Configuration for an extension OCI artifact to be downloaded and hosted.
/// </summary>
public record ExtensionConfig
{
    /// <summary>
    /// The unique name/key for this extension (used in API routing).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The ACR registry URI (e.g., "https://myacr.azurecr.io").
    /// </summary>
    public required string RegistryUri { get; init; }

    /// <summary>
    /// The repository name within the registry.
    /// </summary>
    public required string Repository { get; init; }

    /// <summary>
    /// The tag or digest of the artifact to pull.
    /// </summary>
    public required string Tag { get; init; }
}

/// <summary>
/// Root configuration for the Extension Host application.
/// </summary>
public record ExtensionHostConfig
{
    public const string SectionName = "ExtensionHost";

    /// <summary>
    /// List of extensions to download and host.
    /// </summary>
    public List<ExtensionConfig> Extensions { get; init; } = [];

    /// <summary>
    /// Path where extension binaries will be extracted.
    /// Defaults to a temp directory if not specified.
    /// </summary>
    public string? ExtensionStoragePath { get; init; }
}
