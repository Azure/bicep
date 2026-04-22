// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.IO.Abstraction;
using Azure.Bicep.Types;

namespace Bicep.McpServer.Core.Extensions;

public class ExtensionTypeLoaderProvider
{
    private record SimpleOciAddress(string Registry, string Repository, string? Tag, string? Digest) : IOciArtifactAddressComponents
    {
        public string ArtifactId => Tag is not null
            ? $"{Registry}/{Repository}:{Tag}"
            : $"{Registry}/{Repository}@{Digest}";
    }

    private readonly IContainerRegistryClientFactory clientFactory;
    private readonly IFileExplorer fileExplorer;

    private readonly ConcurrentDictionary<string, ITypeLoader> loaderCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IReadOnlyList<string>> tagCache = new(StringComparer.OrdinalIgnoreCase);

    public ExtensionTypeLoaderProvider(
        IContainerRegistryClientFactory clientFactory,
        IFileExplorer fileExplorer)
    {
        this.clientFactory = clientFactory;
        this.fileExplorer = fileExplorer;
    }

    /// <summary>
    /// Lists available tags (versions) for a well-known extension by querying MCR. Results are cached.
    /// </summary>
    /// <param name="extension">The well-known extension to query.</param>
    public async Task<IReadOnlyList<string>> GetAvailableTagsAsync(WellKnownExtension extension)
    {
        if (tagCache.TryGetValue(extension.Name, out var cached))
        {
            return cached;
        }

        var tags = await GetRepositoryTagsViaOciAsync(extension.Registry, extension.Repository);

        tagCache.TryAdd(extension.Name, tags);
        return tags;
    }

    /// <summary>
    /// Lists tags for a repository using the standard OCI Distribution /v2/{repo}/tags/list endpoint.
    /// The Azure SDK uses ACR-specific APIs that MCR doesn't support, so we call the standard endpoint directly.
    /// </summary>
    private static async Task<IReadOnlyList<string>> GetRepositoryTagsViaOciAsync(string registry, string repository)
    {
        using var httpClient = new HttpClient();
        var url = $"https://{registry}/v2/{repository}/tags/list";
        var response = await httpClient.GetStringAsync(url);
        using var tagsResponse = JsonDocument.Parse(response);
        return [.. tagsResponse.RootElement.GetProperty("tags")
            .EnumerateArray()
            .Select(t => t.GetString()!)];
    }

    /// <summary>
    /// Gets an <see cref="ITypeLoader"/> for a specific extension and tag.
    /// Uses a three-tier cache: in-memory, file-system (~/.bicep/br/...), then MCR pull.
    /// </summary>
    /// <param name="extension">The well-known extension to load.</param>
    /// <param name="tag">The version tag (e.g., "1.0.0").</param>
    public async Task<ITypeLoader> GetTypeLoaderAsync(WellKnownExtension extension, string tag)
    {
        var cacheKey = $"{extension.OciReference}:{tag}";

        if (loaderCache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        var typeLoader = TryLoadFromFileSystemCache(extension, tag)
            ?? await PullAndCacheAsync(extension, tag);

        loaderCache.TryAdd(cacheKey, typeLoader);
        return typeLoader;
    }

    private async Task<ArchivedTypeLoader> PullAndCacheAsync(WellKnownExtension extension, string tag)
    {
        var cloud = GetCloudConfiguration();
        var acrManager = new AzureContainerRegistryManager(clientFactory);
        var address = new SimpleOciAddress(extension.Registry, extension.Repository, tag, null);

        var result = await acrManager.PullArtifactAsync(cloud, address);
        var mainLayer = result.GetMainLayer();

        // Write to file-system cache using the same locking pattern as OciArtifactRegistry/ExternalArtifactRegistry
        await WriteToCacheWithLockAsync(extension, tag, mainLayer.Data);

        return ArchivedTypeLoader.FromStream(mainLayer.Data.ToStream());
    }

    private ArchivedTypeLoader? TryLoadFromFileSystemCache(WellKnownExtension extension, string tag)
    {
        try
        {
            var cacheDir = GetCacheDirectory(extension, tag);
            if (cacheDir is null)
            {
                return null;
            }

            var typesTgzFile = cacheDir.GetFile("types.tgz");
            if (typesTgzFile.Exists())
            {
                return ArchivedTypeLoader.FromFileHandle(typesTgzFile);
            }

            return null;
        }
        catch
        {
            // If the cached file is corrupted or inaccessible, fall through to MCR pull
            return null;
        }
    }

    /// <summary>
    /// Writes the extension artifact to the file-system cache using file-based locking
    /// via <see cref="ArtifactCacheHelper"/>, ensuring consistent concurrency handling with the Bicep compiler.
    /// </summary>
    private async Task WriteToCacheWithLockAsync(WellKnownExtension extension, string tag, BinaryData data)
    {
        try
        {
            var cacheDir = GetCacheDirectory(extension, tag);
            if (cacheDir is null)
            {
                return;
            }

            cacheDir.EnsureExists();
            var lockFile = cacheDir.GetFile("lock");
            var typesTgzFile = cacheDir.GetFile("types.tgz");

            await ArtifactCacheHelper.WriteWithLockAsync(
                lockFile,
                isWriteRequired: () => !typesTgzFile.Exists(),
                writeContent: () => typesTgzFile.Write(data));
        }
        catch
        {
            // Best-effort caching — don't fail the operation if disk write fails
        }
    }

    /// <summary>
    /// Gets the cache directory for an extension artifact, matching the compiler's cache path convention.
    /// Uses <see cref="ArtifactCacheHelper.EncodeCachePathSegments"/> for consistent path encoding with OciArtifactRegistry.
    /// </summary>
    private IDirectoryHandle? GetCacheDirectory(WellKnownExtension extension, string tag)
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userProfile))
        {
            // User profile may not be available in container environments
            return null;
        }

        var relativePath = ArtifactCacheHelper.EncodeCachePathSegments(
            extension.Registry, extension.Repository, tag, digest: null);

        var cacheRoot = fileExplorer.GetDirectory(IOUri.FromFilePath(
            Path.Combine(userProfile, ".bicep", ArtifactReferenceSchemes.Oci)));

        return cacheRoot.GetDirectory(relativePath);
    }

    private static CloudConfiguration GetCloudConfiguration()
    {
        var builtInConfig = IConfigurationManager.GetBuiltInConfiguration();
        return builtInConfig.Cloud;
    }
}
