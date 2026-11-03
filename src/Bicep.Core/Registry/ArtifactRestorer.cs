// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

public class ArtifactRestorer : IArtifactRestorer
{
    private readonly RestoreFailureCache failureCache = new();

    private readonly IArtifactRegistryProvider registryProvider;

    public ArtifactRestorer(IArtifactRegistryProvider registryProvider)
    {
        this.registryProvider = registryProvider;
    }

    public ArtifactRestoreState GetRestoreState(ArtifactReference reference)
    {
        if (this.failureCache.TryGetFailure(reference, out var error))
        {
            return new ArtifactRestoreState.RestoreFailed(error);
        }

        if (this.GetRegistry(reference).IsArtifactRestoreRequired(reference))
        {
            return new ArtifactRestoreState.NotRestored();
        }

        return new ArtifactRestoreState.Restored();
    }

    public ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPoint(ArtifactReference reference)
    {
        if (this.failureCache.TryGetFailure(reference, out var error))
        {
            return new(error);
        }

        return reference.TryGetEntryPointFileHandle();
    }

    public async Task<bool> RestoreAsync(IEnumerable<ArtifactReference> references, RestoreMode mode)
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

    private async Task<bool> RestoreCoreAsync(IEnumerable<ArtifactReference> references, RestoreMode mode)
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

        // Call OnRestoreArtifacts on each registry. Can (currently at least) be done in parallel to restore.
        var onRestoreArtifactsTasks = new List<Task>();
        foreach (var scheme in this.registryProvider.SupportedSchemes)
        {
            var registry = this.registryProvider.GetRegistry(scheme);

            try
            {
                onRestoreArtifactsTasks.Add(registry.OnRestoreArtifacts(force));
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{nameof(IArtifactRegistry.OnRestoreArtifacts)} failed: {ex.Message}");
            }
        }

        if (!force && uniqueReferences.All(reference => this.GetRestoreState(reference) is ArtifactRestoreState.Restored))
        {
            // All the artifacts have already been restored - no need to do anything.
            return false;
        }

        var referencesByRegistry = uniqueReferences.ToLookup(this.GetRegistry);

        foreach (var registry in referencesByRegistry.Select(byRegistry => byRegistry.Key))
        {
            if (force)
            {
                var invalidateFailures = await registry.InvalidateArtifactsCache(referencesByRegistry[registry]);

                foreach (var (failedReference, failureBuilder) in invalidateFailures)
                {
                    this.failureCache.Add(failedReference, failureBuilder);
                }
            }

            var restoreFailures = await registry.RestoreArtifacts(referencesByRegistry[registry]);

            foreach (var (failedReference, failureBuilder) in restoreFailures)
            {
                this.failureCache.Add(failedReference, failureBuilder);
            }

            try
            {
                await Task.WhenAll(onRestoreArtifactsTasks);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{nameof(IArtifactRegistry.OnRestoreArtifacts)} failed: {ex.Message}");
            }
        }

        return true;
    }

    private IArtifactRegistry GetRegistry(ArtifactReference reference) => this.registryProvider.GetRegistry(reference.Scheme);
}
