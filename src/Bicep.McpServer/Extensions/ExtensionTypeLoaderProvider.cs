// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
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

namespace Bicep.McpServer.Extensions;

public class ExtensionTypeLoaderProvider
{
    private record SimpleOciAddress(string Registry, string Repository, string? Tag, string? Digest) : IOciArtifactAddressComponents
    {
        public string ArtifactId => Tag is not null
            ? $"{Registry}/{Repository}:{Tag}"
            : $"{Registry}/{Repository}@{Digest}";
    }

    // Matches ExternalArtifactRegistry timeout/retry for file-system locking
    private static readonly TimeSpan CacheContentionTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan CacheContentionRetryInterval = TimeSpan.FromMilliseconds(300);

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
    /// <param name="extensionName">The extension name (e.g., "MicrosoftGraphBeta"). Must match a <see cref="WellKnownExtension"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="extensionName"/> is not a well-known extension.</exception>
    public async Task<IReadOnlyList<string>> GetAvailableTagsAsync(string extensionName)
    {
        if (tagCache.TryGetValue(extensionName, out var cached))
        {
            return cached;
        }

        var extension = WellKnownExtension.TryGet(extensionName)
            ?? throw new ArgumentException($"Unknown extension '{extensionName}'. Well-known extensions: {string.Join(", ", WellKnownExtension.All.Select(e => e.Name))}");

        var tags = await GetRepositoryTagsViaOciAsync(extension.Registry, extension.Repository);

        tagCache.TryAdd(extensionName, tags);
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
    /// <param name="extensionName">The extension name (e.g., "MicrosoftGraphBeta"). Must match a <see cref="WellKnownExtension"/>.</param>
    /// <param name="tag">The version tag (e.g., "1.0.0").</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="extensionName"/> is not a well-known extension.</exception>
    public async Task<ITypeLoader> GetTypeLoaderAsync(string extensionName, string tag)
    {
        var extension = WellKnownExtension.TryGet(extensionName)
            ?? throw new ArgumentException($"Unknown extension '{extensionName}'. Well-known extensions: {string.Join(", ", WellKnownExtension.All.Select(e => e.Name))}");

        var cacheKey = $"{extensionName}:{tag}";

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
    /// Writes the extension artifact to the file-system cache using file-based locking,
    /// matching the concurrency pattern from ExternalArtifactRegistry.
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
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < CacheContentionTimeout)
            {
                using (var @lock = lockFile.TryLock())
                {
                    if (@lock is not null)
                    {
                        // Double-check: another process may have already written
                        if (typesTgzFile.Exists())
                        {
                            return;
                        }

                        typesTgzFile.Write(data);
                        return;
                    }
                }

                await Task.Delay(CacheContentionRetryInterval);
            }

            // Timeout exceeded — best-effort, don't fail the operation
        }
        catch
        {
            // Best-effort caching — don't fail the operation if disk write fails
        }
    }

    /// <summary>
    /// Gets the cache directory for an extension artifact, matching the compiler's cache path convention.
    /// Uses the same path encoding as OciArtifactRegistry (TagEncoder, registry/repo char replacement).
    /// </summary>
    private IDirectoryHandle? GetCacheDirectory(WellKnownExtension extension, string tag)
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userProfile))
        {
            // User profile may not be available in container environments
            return null;
        }

        // Match the compiler's cache path convention from OciArtifactRegistry.GetArtifactDirectory:
        // ~/.bicep/br/{registry with : replaced by $, lowered}/{repo with / replaced by $}/{TagEncoder.Encode(tag)}
        var registry = extension.Registry.Replace(':', '$').ToLowerInvariant();
        var repository = extension.Repository.Replace('/', '$');
        var encodedTag = TagEncoder.Encode(tag);

        var cacheRoot = fileExplorer.GetDirectory(IOUri.FromFilePath(
            Path.Combine(userProfile, ".bicep", ArtifactReferenceSchemes.Oci)));

        return cacheRoot.GetDirectory($"{registry}/{repository}/{encodedTag}");
    }

    private static CloudConfiguration GetCloudConfiguration()
    {
        var builtInConfig = IConfigurationManager.GetBuiltInConfiguration();
        return builtInConfig.Cloud;
    }
}
