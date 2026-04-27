// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using Azure.Bicep.Types;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;

namespace Bicep.McpServer.Core.Extensions;

public class ExtensionTypeLoaderProvider
{
    private readonly IModuleDispatcher moduleDispatcher;
    private readonly IFeatureProvider featureProvider;

    private readonly ConcurrentDictionary<string, ITypeLoader> loaderCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IReadOnlyList<string>> tagCache = new(StringComparer.OrdinalIgnoreCase);

    public ExtensionTypeLoaderProvider(
        IModuleDispatcher moduleDispatcher,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.moduleDispatcher = moduleDispatcher;

        // The dummy path is never opened or read — it's only used by GetFeatureProvider to resolve
        // the nearest bicepconfig.json (none is found, so built-in defaults are used).
        // We only need the resulting IFeatureProvider for its CacheRootDirectory (~/.bicep).
        this.featureProvider = featureProviderFactory.GetFeatureProvider(
            IOUri.FromFilePath(Path.Combine(Path.GetTempPath(), "dummy.bicep")));
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
    /// Uses <see cref="IModuleDispatcher"/> to restore the artifact from MCR (with caching handled by the compiler's OCI registry).
    /// Results are also cached in-memory for fast repeated access.
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

        var reference = new OciArtifactReference(
            featureProvider, IConfigurationManager.GetBuiltInConfiguration(), ArtifactType.Extension,
            extension.Registry, extension.Repository, tag, digest: null);

        // RestoreArtifacts is a no-op if already cached on disk
        await moduleDispatcher.RestoreArtifacts([reference], forceRestore: false);

        if (!moduleDispatcher.TryGetLocalArtifactEntryPointFileHandle(reference)
            .IsSuccess(out var fileHandle, out var errorBuilder))
        {
            var diagnostic = errorBuilder(DiagnosticBuilder.ForDocumentStart());
            throw new InvalidOperationException(
                $"Failed to load extension '{extension.Name}:{tag}': {diagnostic.Message}");
        }

        var typeLoader = ArchivedTypeLoader.FromFileHandle(fileHandle);
        loaderCache.TryAdd(cacheKey, typeLoader);

        return typeLoader;
    }
}
