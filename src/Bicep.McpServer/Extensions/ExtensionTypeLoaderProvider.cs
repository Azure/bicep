// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Extensibility;
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

    private readonly IContainerRegistryClientFactory clientFactory;

    private readonly ConcurrentDictionary<string, ITypeLoader> loaderCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IReadOnlyList<string>> tagCache = new(StringComparer.OrdinalIgnoreCase);

    public ExtensionTypeLoaderProvider(
        IContainerRegistryClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    /// <summary>
    /// Lists available tags (versions) for a published extension by querying MCR. Results are cached.
    /// </summary>
    /// <param name="extensionName">The extension name (e.g., "microsoftgraph/beta"). Must match a <see cref="PublishedExtension"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="extensionName"/> is not a published extension.</exception>
    public async Task<IReadOnlyList<string>> GetAvailableTagsAsync(string extensionName)
    {
        if (tagCache.TryGetValue(extensionName, out var cached))
        {
            return cached;
        }

        var extension = PublishedExtension.TryGet(extensionName)
            ?? throw new ArgumentException($"Unknown extension '{extensionName}'. Published extensions: {string.Join(", ", PublishedExtension.All.Select(e => e.Name))}");

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
        var tagsResponse = System.Text.Json.JsonDocument.Parse(response);
        return [.. tagsResponse.RootElement.GetProperty("tags")
            .EnumerateArray()
            .Select(t => t.GetString()!)];
    }

    /// <summary>
    /// Gets an <see cref="ITypeLoader"/> for a specific extension and tag.
    /// Uses a three-tier cache: in-memory, file-system (~/.bicep/br/...), then MCR pull.
    /// </summary>
    /// <param name="extensionName">The extension name (e.g., "microsoftgraph/beta"). Must match a <see cref="PublishedExtension"/>.</param>
    /// <param name="tag">The version tag (e.g., "1.0.0").</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="extensionName"/> is not a published extension.</exception>
    public async Task<ITypeLoader> GetTypeLoaderAsync(string extensionName, string tag)
    {
        var extension = PublishedExtension.TryGet(extensionName)
            ?? throw new ArgumentException($"Unknown extension '{extensionName}'. Published extensions: {string.Join(", ", PublishedExtension.All.Select(e => e.Name))}");

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

    private async Task<ArchivedTypeLoader> PullAndCacheAsync(PublishedExtension extension, string tag)
    {
        var cloud = GetCloudConfiguration();
        var acrManager = new AzureContainerRegistryManager(clientFactory);
        var address = new SimpleOciAddress(extension.Registry, extension.Repository, tag, null);

        var result = await acrManager.PullArtifactAsync(cloud, address);
        var mainLayer = result.GetMainLayer();

        // Write to file-system cache
        WriteToFileSystemCache(extension, tag, mainLayer.Data);

        return ArchivedTypeLoader.FromStream(mainLayer.Data.ToStream());
    }

    private static ArchivedTypeLoader? TryLoadFromFileSystemCache(PublishedExtension extension, string tag)
    {
        var cachePath = GetFileSystemCachePath(extension, tag);
        if (cachePath is null)
        {
            return null;
        }

        var typesTgzPath = Path.Combine(cachePath, "types.tgz");

        if (File.Exists(typesTgzPath))
        {
            using var stream = File.OpenRead(typesTgzPath);
            return ArchivedTypeLoader.FromStream(stream);
        }

        return null;
    }

    private static void WriteToFileSystemCache(PublishedExtension extension, string tag, BinaryData data)
    {
        try
        {
            var cachePath = GetFileSystemCachePath(extension, tag);
            if (cachePath is null)
            {
                return;
            }

            Directory.CreateDirectory(cachePath);
            var typesTgzPath = Path.Combine(cachePath, "types.tgz");
            File.WriteAllBytes(typesTgzPath, data.ToArray());
        }
        catch
        {
            // Best-effort caching — don't fail the operation if disk write fails
        }
    }

    private static string? GetFileSystemCachePath(PublishedExtension extension, string tag)
    {
        // Match the compiler's cache path convention:
        // ~/.bicep/br/{registry with : replaced by $, lowered}/{repo with / replaced by $}/{tag}
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userProfile))
        {
            // User profile may not be available in container environments
            return null;
        }

        var registry = extension.Registry.Replace(':', '$').ToLowerInvariant();
        var repository = extension.Repository.Replace('/', '$');
        return Path.Combine(userProfile, ".bicep", "br", registry, repository, tag);
    }

    private static CloudConfiguration GetCloudConfiguration()
    {
        var builtInConfig = IConfigurationManager.GetBuiltInConfiguration();
        return builtInConfig.Cloud;
    }
}
