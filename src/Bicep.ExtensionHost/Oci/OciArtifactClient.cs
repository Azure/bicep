// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Compression;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.ExtensionHost.Configuration;

namespace Bicep.ExtensionHost.Oci;

/// <summary>
/// Media types for Bicep extension OCI artifacts.
/// </summary>
public static class BicepMediaTypes
{
    public const string ManifestMediaType = "application/vnd.oci.image.manifest.v1+json";
    public const string ArtifactType = "application/vnd.ms.bicep.provider.artifact";
    public const string ConfigMediaType = "application/vnd.ms.bicep.provider.config.v1+json";
    public const string TypesLayerMediaType = "application/vnd.ms.bicep.provider.layer.v1.tar+gzip";
    public const string LinuxX64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.linux-x64.binary";
    public const string LinuxArm64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.linux-arm64.binary";
    public const string OsxX64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.osx-x64.binary";
    public const string OsxArm64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.osx-arm64.binary";
    public const string WinX64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.win-x64.binary";
    public const string WinArm64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.win-arm64.binary";
}

/// <summary>
/// Represents the result of downloading an extension binary.
/// </summary>
public record ExtensionBinaryInfo(
    string ExtensionName,
    string BinaryPath,
    string Platform);

/// <summary>
/// Downloads OCI artifacts from ACR (with anonymous pull support) and extracts platform-specific binaries.
/// </summary>
public class OciArtifactClient
{
    private readonly ILogger<OciArtifactClient> _logger;
    private readonly string _storagePath;

    public OciArtifactClient(ILogger<OciArtifactClient> logger, string storagePath)
    {
        _logger = logger;
        _storagePath = storagePath;

        // Ensure storage directory exists
        Directory.CreateDirectory(_storagePath);
    }

    /// <summary>
    /// Downloads an extension OCI artifact and extracts the appropriate binary for the current platform.
    /// </summary>
    public async Task<ExtensionBinaryInfo> DownloadExtensionAsync(ExtensionConfig config, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading extension '{Name}' from {Registry}/{Repository}:{Tag}",
            config.Name, config.RegistryUri, config.Repository, config.Tag);

        var registryUri = new Uri(config.RegistryUri);

        // Create anonymous client (no credentials needed for anonymous pull)
        var options = new ContainerRegistryClientOptions();
        var contentClient = new ContainerRegistryContentClient(registryUri, config.Repository, options);

        // Get the manifest
        var manifestResponse = await contentClient.GetManifestAsync(config.Tag, cancellationToken);
        var manifest = manifestResponse.Value;

        _logger.LogDebug("Retrieved manifest for {Name}, media type: {MediaType}",
            config.Name, manifest.MediaType);

        // Find the appropriate binary layer for current platform
        var targetMediaType = GetPlatformMediaType();
        _logger.LogInformation("Looking for binary layer with media type: {MediaType}", targetMediaType);

        OciDescriptor? binaryLayer = null;
        var ociManifest = manifest.Manifest.ToObjectFromJson<OciManifest>();
        if (ociManifest is not null)
        {
            _logger.LogInformation("Manifest has {Count} layers", ociManifest.Layers.Count);
            foreach (var layer in ociManifest.Layers)
            {
                _logger.LogInformation("  Layer: mediaType={MediaType}, digest={Digest}, size={Size}",
                    layer.MediaType, layer.Digest, layer.Size);
                if (layer.MediaType == targetMediaType)
                {
                    binaryLayer = layer;
                    break;
                }
            }
        }
        else
        {
            _logger.LogWarning("Failed to parse OCI manifest");
        }

        if (binaryLayer is null)
        {
            throw new InvalidOperationException(
                $"No binary layer found for platform '{targetMediaType}' in extension '{config.Name}'");
        }

        // Download the binary layer
        _logger.LogInformation("Downloading binary layer {Digest} ({Size} bytes)",
            binaryLayer.Digest, binaryLayer.Size);

        var blobResponse = await contentClient.DownloadBlobContentAsync(binaryLayer.Digest, cancellationToken);

        // Save the binary to disk
        var extensionDir = Path.Combine(_storagePath, config.Name);
        Directory.CreateDirectory(extensionDir);

        var binaryFileName = OperatingSystem.IsWindows() ? "extension.exe" : "extension.bin";
        var binaryPath = Path.Combine(extensionDir, binaryFileName);

        await using (var fileStream = File.Create(binaryPath))
        {
            await blobResponse.Value.Content.ToStream().CopyToAsync(fileStream, cancellationToken);
        }

        // Make binary executable on Unix platforms
        if (!OperatingSystem.IsWindows())
        {
            MakeExecutable(binaryPath);
        }

        _logger.LogInformation("Successfully downloaded extension '{Name}' binary to {Path}",
            config.Name, binaryPath);

        return new ExtensionBinaryInfo(config.Name, binaryPath, targetMediaType);
    }

    private static string GetPlatformMediaType()
    {
        if (OperatingSystem.IsWindows())
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSArchitecture switch
            {
                System.Runtime.InteropServices.Architecture.X64 => BicepMediaTypes.WinX64BinaryMediaType,
                System.Runtime.InteropServices.Architecture.Arm64 => BicepMediaTypes.WinArm64BinaryMediaType,
                _ => throw new PlatformNotSupportedException(
                    $"Windows architecture {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture} is not supported")
            };
        }

        if (OperatingSystem.IsLinux())
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSArchitecture switch
            {
                System.Runtime.InteropServices.Architecture.X64 => BicepMediaTypes.LinuxX64BinaryMediaType,
                System.Runtime.InteropServices.Architecture.Arm64 => BicepMediaTypes.LinuxArm64BinaryMediaType,
                _ => throw new PlatformNotSupportedException(
                    $"Linux architecture {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture} is not supported")
            };
        }

        if (OperatingSystem.IsMacOS())
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSArchitecture switch
            {
                System.Runtime.InteropServices.Architecture.X64 => BicepMediaTypes.OsxX64BinaryMediaType,
                System.Runtime.InteropServices.Architecture.Arm64 => BicepMediaTypes.OsxArm64BinaryMediaType,
                _ => throw new PlatformNotSupportedException(
                    $"macOS architecture {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture} is not supported")
            };
        }

        throw new PlatformNotSupportedException("Current operating system is not supported");
    }

    private static void MakeExecutable(string filePath)
    {
        // On Unix, we need to set the executable bit
        // Using chmod via File.SetUnixFileMode (available in .NET 7+)
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(filePath,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
        }
    }
}

/// <summary>
/// Represents an OCI manifest structure.
/// </summary>
internal record OciManifest
{
    [System.Text.Json.Serialization.JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("mediaType")]
    public string? MediaType { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("artifactType")]
    public string? ArtifactType { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("config")]
    public OciDescriptor? Config { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("layers")]
    public List<OciDescriptor> Layers { get; init; } = [];
}

/// <summary>
/// Represents an OCI descriptor (for config or layer references).
/// </summary>
internal record OciDescriptor
{
    [System.Text.Json.Serialization.JsonPropertyName("mediaType")]
    public required string MediaType { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("digest")]
    public required string Digest { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("size")]
    public long Size { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("annotations")]
    public Dictionary<string, string>? Annotations { get; init; }
}
