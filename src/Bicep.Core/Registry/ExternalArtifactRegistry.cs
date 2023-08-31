// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public abstract class ExternalArtifactRegistry<TArtifactReference, TArtifactEntity> : ArtifactRegistry<TArtifactReference>
        where TArtifactReference : ArtifactReference
    {
        // if we're unable to acquire a lock on the artifact directory in the cache, we will retry until this timeout is reached
        private static readonly TimeSpan ArtifactDirectoryContentionTimeout = TimeSpan.FromSeconds(5);

        // interval at which we will retry acquiring the lock on the artifact directory in the cache
        private static readonly TimeSpan ArtifactDirectoryContentionRetryInterval = TimeSpan.FromMilliseconds(300);

        protected ExternalArtifactRegistry(IFileResolver fileResolver)
        {
            this.FileResolver = fileResolver;
        }

        protected IFileResolver FileResolver { get; }

        protected abstract void WriteArtifactContent(TArtifactReference reference, TArtifactEntity entity);

        protected abstract string GetArtifactDirectoryPath(TArtifactReference reference);

        protected abstract Uri GetArtifactLockFileUri(TArtifactReference reference);

        protected async Task TryWriteArtifactContentAsync(TArtifactReference reference, TArtifactEntity entity)
        {
            // this has to be after downloading the artifact content so we don't create directories for non-existent artifacts
            var artifactDirectoryPath = this.GetArtifactDirectoryPath(reference);

            // creating the directory doesn't require locking
            CreateArtifactDirectory(artifactDirectoryPath);

            /*
             * We have already downloaded the artifact content from the registry.
             * The following sections will attempt to synchronize the artifact write with other
             * instances of the language server running on the same machine.
             *
             * We are not trying to prevent tampering with the artifact cache by the user.
             */

            var lockFileUri = this.GetArtifactLockFileUri(reference);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < ArtifactDirectoryContentionTimeout)
            {
                using (var @lock = this.FileResolver.TryAcquireFileLock(lockFileUri))
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
                        this.WriteArtifactContent(reference, entity);
                        return;
                    }
                }

                // we have not acquired the lock - let's give the instance that has the lock some time to finish writing the content to the directory
                // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
                await Task.Delay(ArtifactDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new ExternalArtifactException($"Exceeded the timeout of \"{ArtifactDirectoryContentionTimeout}\" to acquire the lock on file \"{lockFileUri}\".");
        }

        private static void CreateArtifactDirectory(string artifactDirectoryPath)
        {
            try
            {
                // ensure that the directory exists
                Directory.CreateDirectory(artifactDirectoryPath);
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException($"Unable to create the local artifact directory \"{artifactDirectoryPath}\". {exception.Message}", exception);
            }
        }

        private static void DeleteArtifactDirectory(string artifactDirectoryPath)
        {
            try
            {
                // recursively delete the directory
                Directory.Delete(artifactDirectoryPath, true);
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException($"Unable to delete the local artifact directory \"{artifactDirectoryPath}\". {exception.Message}", exception);
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

            var lockFileUri = this.GetArtifactLockFileUri(reference);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < ArtifactDirectoryContentionTimeout)
            {
                if (!this.FileResolver.FileExists(lockFileUri))
                {
                    // no lock exists, proceed
                    var artifactDirectoryPath = this.GetArtifactDirectoryPath(reference);

                    // delete the directory and its contents on disk
                    DeleteArtifactDirectory(artifactDirectoryPath);

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
                        File.Delete(lockFileUri.LocalPath);
                    }
                    catch (IOException) { break; }
                }

                // lock is still present - let's give the instance that has the lock some time to finish writing the content to the directory
                // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
                await Task.Delay(ArtifactDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new ExternalArtifactException($"Exceeded the timeout of \"{ArtifactDirectoryContentionTimeout}\" for the lock on file \"{lockFileUri}\" to be released.");
        }

        // base implementation for cache invalidation that should fit all external registries
        protected async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCacheInternal(IEnumerable<TArtifactReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Delete artifact {reference.FullyQualifiedReference} from cache");
                try
                {
                    if (Directory.Exists(GetArtifactDirectoryPath(reference)))
                    {
                        await this.TryDeleteArtifactDirectoryAsync(reference);
                    }
                }
                catch (Exception exception)
                {
                    if (exception.Message is { } message)
                    {
                        statuses.Add(reference, x => x.ArtifactDeleteFailedWithMessage(reference.FullyQualifiedReference, message));
                        timer.OnFail($"Unexpected exception {exception}: {message}");

                        return statuses;
                    }

                    statuses.Add(reference, x => x.ArtifactDeleteFailed(reference.FullyQualifiedReference));
                    timer.OnFail($"Unexpected exception {exception}.");
                }
            }

            return statuses;
        }
    }
}
