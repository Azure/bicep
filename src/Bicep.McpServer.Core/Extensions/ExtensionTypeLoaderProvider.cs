// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using Azure.Bicep.Types;
using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;

namespace Bicep.McpServer.Core.Extensions;

public class ExtensionTypeLoaderProvider
{
    private readonly IRemoteArtifactRestorer restorer;
    private readonly IFeatureProvider featureProvider;

    private readonly ConcurrentDictionary<string, ITypeLoader> loaderCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IReadOnlyList<string>> tagCache = new(StringComparer.OrdinalIgnoreCase);

    public ExtensionTypeLoaderProvider(
        IRemoteArtifactRestorer restorer,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.restorer = restorer;

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
    /// Uses <see cref="IRemoteArtifactRestorer"/> to restore the artifact from MCR (with caching handled by the compiler's OCI registry).
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
            OciArtifactContext.Create(BicepConfiguration.BuiltIn, featureProvider), ArtifactType.Extension,
            extension.Registry, extension.Repository, tag, digest: null);

        // RestoreAsync is a no-op if already cached on disk
        await restorer.RestoreAsync([reference], RestoreMode.IfMissing);

        var fileHandle = restorer.GetRestoreState(reference) switch
        {
            RemoteArtifactRestoreState.Restored restored => restored.EntryPoint,
            RemoteArtifactRestoreState.RestoreFailed failed => throw new InvalidOperationException(
                $"Failed to load extension '{extension.Name}:{tag}': {failed.Error(DiagnosticBuilder.ForDocumentStart()).Message}"),
            _ => throw new InvalidOperationException($"Failed to load extension '{extension.Name}:{tag}': the extension was not restored."),
        };

        var typeLoader = ArchivedTypeLoader.FromFileHandle(fileHandle);
        loaderCache.TryAdd(cacheKey, typeLoader);

        return typeLoader;
    }
}
