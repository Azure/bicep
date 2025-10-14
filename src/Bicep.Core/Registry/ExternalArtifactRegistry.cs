// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Tracing;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public abstract class ExternalArtifactRegistry<TArtifactReference, TArtifactEntity> : ArtifactRegistry<TArtifactReference>
        where TArtifactReference : ArtifactReference
    {
        // if we're unable to acquire a lock on the artifact directory in the cache, we will retry until this timeout is reached
        private static readonly TimeSpan ArtifactDirectoryContentionTimeout = TimeSpan.FromSeconds(5);

        // interval at which we will retry acquiring the lock on the artifact directory in the cache
        private static readonly TimeSpan ArtifactDirectoryContentionRetryInterval = TimeSpan.FromMilliseconds(300);

        protected abstract void WriteArtifactContentToCache(TArtifactReference reference, TArtifactEntity entity);

        protected abstract IDirectoryHandle GetArtifactDirectory(TArtifactReference reference);

        protected abstract IFileHandle GetArtifactLockFile(TArtifactReference reference);

        protected async Task WriteArtifactContentToCacheAsync(TArtifactReference reference, TArtifactEntity entity)
        {
            // this has to be after downloading the artifact content so we don't create directories for non-existent artifacts
            var artifactDirectory = this.GetArtifactDirectory(reference);

            // creating the directory doesn't require locking
            CreateArtifactDirectory(artifactDirectory);

            /*
             * We have already downloaded the artifact content from the registry.
             * The following sections will attempt to synchronize the artifact write with other
             * instances of the language server running on the same machine.
             *
             * We are not trying to prevent tampering with the artifact cache by the user.
             */

            var lockFile = this.GetArtifactLockFile(reference);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < ArtifactDirectoryContentionTimeout)
            {
                using (var @lock = lockFile.TryLock())
                {
                    // the placement of "if" inside "using" guarantees that even an exception thrown by the condition results in the lock being released
                    // (current condition can't throw, but this potentially avoids future regression)
                    if (@lock is not null)
                    {
                        // we have acquired the lock
                        if (!this.IsArtifactRestoreRequired(reference))
                        {
                            // the other instance has already written out the content to disk - we can discard the content we downloaded
                            return;
                        }

                        // write the contents to disk
                        this.WriteArtifactContentToCache(reference, entity);
                        return;
                    }
                }

                // we have not acquired the lock - let's give the instance that has the lock some time to finish writing the content to the directory
                // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
                await Task.Delay(ArtifactDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new ExternalArtifactException($"Exceeded the timeout of \"{ArtifactDirectoryContentionTimeout}\" to acquire the lock on file \"{lockFile.Uri}\".");
        }

        private void CreateArtifactDirectory(IDirectoryHandle artifactDirectory)
        {
            try
            {
                // ensure that the directory exists
                artifactDirectory.EnsureExists();
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException($"Unable to create the local artifact directory \"{artifactDirectory.Uri}\". {exception.Message}", exception);
            }
        }

        private void DeleteArtifactDirectory(IDirectoryHandle artifactDirectory)
        {
            try
            {
                // recursively delete the directory
                artifactDirectory.Delete();
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException($"Unable to delete the local artifact directory \"{artifactDirectory.Uri}\". {exception.Message}", exception);
            }
        }

        private async Task TryDeleteArtifactDirectoryAsync(TArtifactReference reference)
        {
            /*
             * The following sections will attempt to synchronize the artifact directory delete with other
             * instances of the language server running on the same machine.
             *
             * We are not trying to prevent tampering with the artifact cache by the user.
             */

            var lockFile = this.GetArtifactLockFile(reference);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < ArtifactDirectoryContentionTimeout)
            {
                if (!lockFile.Exists())
                {
                    // no lock exists, proceed
                    var artifactDirectory = this.GetArtifactDirectory(reference);

                    // delete the directory and its contents on disk
                    DeleteArtifactDirectory(artifactDirectory);

                    return;
                }
                else
                {
                    try
                    {
                        // Even if the FileLock is disposed, the file remain there. See comments in FileLock.cs
                        // saying there's a race condition on Linux with the DeleteOnClose flag on the FileStream.
                        // We will attempt to delete the file. If it throws, the lock is still open and will continue
                        // to wait until retry interval expires
                        lockFile.Delete();
                    }
                    catch (IOException) { break; }
                }

                // lock is still present - let's give the instance that has the lock some time to finish writing the content to the directory
                // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
                await Task.Delay(ArtifactDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new ExternalArtifactException($"Exceeded the timeout of \"{ArtifactDirectoryContentionTimeout}\" for the lock on file \"{lockFile.Uri}\" to be released.");
        }

        // base implementation for cache invalidation that should fit all external registries
        protected async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCacheInternal(IEnumerable<TArtifactReference> references)
        {
            var failures = new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Delete artifact {reference.FullyQualifiedReference} from cache");
                try
                {
                    if (GetArtifactDirectory(reference).Exists())
                    {
                        await this.TryDeleteArtifactDirectoryAsync(reference);
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
    }
}
