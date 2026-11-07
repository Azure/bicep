// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Cache;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Diagnostics;
using Bicep.Core.Tracing;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Registries
{
    /// <summary>
    /// Restores one kind of remote artifact into the local cache. Where an artifact is cached is up to its cache entry.
    /// </summary>
    public abstract class RemoteArtifactRegistry<TArtifactReference> : IRemoteArtifactRegistry
        where TArtifactReference : RemoteArtifactReference
    {
        public abstract string Scheme { get; }

        public bool CanHandle(RemoteArtifactReference reference) => reference is TArtifactReference;

        public IFileHandle? TryGetEntryPoint(RemoteArtifactReference reference) => this.TryGetEntryPoint(ConvertReference(reference));

        public async Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreAsync(IEnumerable<RemoteArtifactReference> references, RestoreMode mode)
        {
            var typedReferences = references.Select(ConvertReference).ToArray();

            if (mode != RestoreMode.Force)
            {
                return await this.RestoreArtifacts(typedReferences);
            }

            var failures = new Dictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>(await this.InvalidateArtifactsCache(typedReferences));

            foreach (var (reference, error) in await this.RestoreArtifacts(typedReferences))
            {
                // A failure to delete the cached copy is reported instead of the restore failure that follows it.
                failures.TryAdd(reference, error);
            }

            return failures;
        }

        protected abstract ArtifactCacheEntry GetCacheEntry(TArtifactReference reference);

        protected virtual IFileHandle? TryGetEntryPoint(TArtifactReference reference) =>
            this.GetCacheEntry(reference) is { IsComplete: true } cacheEntry ? cacheEntry.EntryPoint : null;

        protected abstract Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<TArtifactReference> references);

        private async Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<TArtifactReference> references)
        {
            var failures = new Dictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Delete artifact {reference.FullyQualifiedReference} from cache");
                try
                {
                    var cacheEntry = this.GetCacheEntry(reference);
                    if (cacheEntry.CacheDirectory.Exists())
                    {
                        await cacheEntry.DeleteAsync();
                    }
                }
                catch (Exception exception)
                {
                    if (exception.Message is { } message)
                    {
                        failures.Add(reference, x => x.ArtifactDeleteFailedWithMessage(reference.FullyQualifiedReference, message));
                        timer.OnFail($"Unexpected exception {exception}: {message}");

                        return failures;
                    }

                    failures.Add(reference, x => x.ArtifactDeleteFailed(reference.FullyQualifiedReference));
                    timer.OnFail($"Unexpected exception {exception}.");
                }
            }

            return failures;
        }

        private static TArtifactReference ConvertReference(RemoteArtifactReference reference) => reference switch
        {
            TArtifactReference typed => typed,
            _ => throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported."),
        };
    }
}