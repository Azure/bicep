// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Registry.Catalog;

namespace Bicep.Core.Artifacts.Restore;

public class ArtifactRestorer : IArtifactRestorer
{
    private readonly RestoreFailureCache failureCache = new();

    private readonly ImmutableArray<IArtifactRegistry> registries;

    private readonly IPublicModuleMetadataProvider publicModuleMetadataProvider;

    public ArtifactRestorer(IEnumerable<IArtifactRegistry> registries, IPublicModuleMetadataProvider publicModuleMetadataProvider)
    {
        this.registries = [.. registries];
        this.publicModuleMetadataProvider = publicModuleMetadataProvider;
    }

    public ArtifactRestoreState GetRestoreState(ExternalArtifactReference reference)
    {
        if (this.failureCache.TryGetFailure(reference, out var error))
        {
            return new ArtifactRestoreState.RestoreFailed(error);
        }

        return this.GetRegistry(reference).TryGetEntryPoint(reference) is { } entryPoint
            ? new ArtifactRestoreState.Restored(entryPoint)
            : new ArtifactRestoreState.NotRestored();
    }

    public async Task<bool> RestoreAsync(IEnumerable<ExternalArtifactReference> references, RestoreMode mode)
    {
        try
        {
            return await this.RestoreCoreAsync(references, mode);
        }
        finally
        {
            this.failureCache.RemoveExpired();
        }
    }

    private async Task<bool> RestoreCoreAsync(IEnumerable<ExternalArtifactReference> references, RestoreMode mode)
    {
        var force = mode == RestoreMode.Force;

        // WARNING: The operations on references here rely on the custom Equals() implementations and NOT on object identity.
        // Many declarations can point to the same artifact.
        var uniqueReferences = references.Distinct().ToArray();

        if (force)
        {
            // Forcing a restore retries artifacts that failed recently, rather than reporting the earlier failure.
            foreach (var reference in uniqueReferences)
            {
                this.failureCache.Remove(reference);
            }
        }

        var publicModuleMetadataTask = this.StartPublicModuleMetadataDownload(force);

        if (!force && uniqueReferences.All(reference => this.GetRestoreState(reference) is ArtifactRestoreState.Restored))
        {
            // All the artifacts have already been restored - no need to do anything.
            return false;
        }

        foreach (var referencesForRegistry in uniqueReferences.GroupBy(this.GetRegistry))
        {
            foreach (var (failedReference, failureBuilder) in await referencesForRegistry.Key.RestoreAsync(referencesForRegistry, mode))
            {
                this.failureCache.Add(failedReference, failureBuilder);
            }
        }

        try
        {
            await publicModuleMetadataTask;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Downloading the public module metadata failed: {ex.Message}");
        }

        return true;
    }

    /// <summary>
    /// Downloads the public module metadata alongside the restore, so that linter rules can read it from the cache
    /// instead of downloading anything during analysis. Nothing is downloaded when restore is skipped (<c>--no-restore</c>).
    /// </summary>
    private Task StartPublicModuleMetadataDownload(bool force)
    {
        try
        {
            return this.publicModuleMetadataProvider.TryAwaitCache(force);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Downloading the public module metadata failed: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    private IArtifactRegistry GetRegistry(ExternalArtifactReference reference) =>
        this.registries.FirstOrDefault(registry => registry.CanHandle(reference)) ??
        throw new ArgumentException($"No artifact registry is configured for references of type '{reference.GetType().Name}'.");
}
