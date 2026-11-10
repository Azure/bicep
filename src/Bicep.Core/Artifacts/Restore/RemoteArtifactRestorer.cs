// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Artifacts.Catalog.Brm;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;

namespace Bicep.Core.Artifacts.Restore;

public class RemoteArtifactRestorer : IRemoteArtifactRestorer
{
    private readonly RestoreFailureCache failureCache = new();

    private readonly ImmutableArray<IRemoteArtifactRegistry> registries;

    private readonly IBrmMetadataProvider publicModuleMetadataProvider;

    public RemoteArtifactRestorer(IEnumerable<IRemoteArtifactRegistry> registries, IBrmMetadataProvider publicModuleMetadataProvider)
    {
        this.registries = [.. registries];
        this.publicModuleMetadataProvider = publicModuleMetadataProvider;
    }

    public RemoteArtifactRestoreState GetRestoreState(RemoteArtifactReference reference)
    {
        if (this.failureCache.TryGetFailure(reference, out var error))
        {
            return new RemoteArtifactRestoreState.RestoreFailed(error);
        }

        return this.GetRegistry(reference).TryGetEntryPoint(reference) is { } entryPoint
            ? new RemoteArtifactRestoreState.Restored(entryPoint)
            : new RemoteArtifactRestoreState.NotRestored();
    }

    public async Task<bool> RestoreAsync(IEnumerable<RemoteArtifactReference> references, RestoreMode mode)
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

    private async Task<bool> RestoreCoreAsync(IEnumerable<RemoteArtifactReference> references, RestoreMode mode)
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

        var brmMetadataTask = this.StartBrmMetadataDownload(force);

        if (!force && uniqueReferences.All(reference => this.GetRestoreState(reference) is RemoteArtifactRestoreState.Restored))
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
            await brmMetadataTask;
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
    private Task StartBrmMetadataDownload(bool force)
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

    private IRemoteArtifactRegistry GetRegistry(RemoteArtifactReference reference) =>
        this.registries.FirstOrDefault(registry => registry.CanHandle(reference)) ??
        throw new ArgumentException($"No artifact registry is configured for references of type '{reference.GetType().Name}'.");
}
