// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
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
    public abstract class ExternalModuleRegistry<TModuleReference, TModuleEntity> : ModuleRegistry<TModuleReference>
        where TModuleReference : ModuleReference
    {
        // if we're unable to acquire a lock on the module directory in the cache, we will retry until this timeout is reached
        private static readonly TimeSpan ModuleDirectoryContentionTimeout = TimeSpan.FromSeconds(5);

        // interval at which we will retry acquiring the lock on the module directory in the cache
        private static readonly TimeSpan ModuleDirectoryContentionRetryInterval = TimeSpan.FromMilliseconds(300);

        protected ExternalModuleRegistry(IFileResolver fileResolver)
        {
            this.FileResolver = fileResolver;
        }

        protected IFileResolver FileResolver { get; }

        protected abstract void WriteModuleContent(TModuleReference reference, TModuleEntity entity);

        protected abstract string GetModuleDirectoryPath(TModuleReference reference);

        protected abstract Uri GetModuleLockFileUri(TModuleReference reference);

        protected async Task TryWriteModuleContentAsync(TModuleReference reference, TModuleEntity entity)
        {
            // this has to be after downloading the module content so we don't create directories for non-existent modules
            var moduleDirectoryPath = this.GetModuleDirectoryPath(reference);

            // creating the directory doesn't require locking
            CreateModuleDirectory(moduleDirectoryPath);

            /*
             * We have already downloaded the module content from the registry.
             * The following sections will attempt to synchronize the module write with other
             * instances of the language server running on the same machine.
             *
             * We are not trying to prevent tampering with the module cache by the user.
             */

            var lockFileUri = this.GetModuleLockFileUri(reference);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < ModuleDirectoryContentionTimeout)
            {
                using (var @lock = this.FileResolver.TryAcquireFileLock(lockFileUri))
                {
                    // the placement of "if" inside "using" guarantees that even an exception thrown by the condition results in the lock being released
                    // (current condition can't throw, but this potentially avoids future regression)
                    if (@lock is not null)
                    {
                        // we have acquired the lock
                        if (!this.IsModuleRestoreRequired(reference))
                        {
                            // the other instance has already written out the content to disk - we can discard the content we downloaded
                            return;
                        }

                        // write the contents to disk
                        this.WriteModuleContent(reference, entity);
                        return;
                    }
                }

                // we have not acquired the lock - let's give the instance that has the lock some time to finish writing the content to the directory
                // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
                await Task.Delay(ModuleDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new ExternalModuleException($"Exceeded the timeout of \"{ModuleDirectoryContentionTimeout}\" to acquire the lock on file \"{lockFileUri}\".");
        }

        private static void CreateModuleDirectory(string moduleDirectoryPath)
        {
            try
            {
                // ensure that the directory exists
                Directory.CreateDirectory(moduleDirectoryPath);
            }
            catch (Exception exception)
            {
                throw new ExternalModuleException($"Unable to create the local module directory \"{moduleDirectoryPath}\". {exception.Message}", exception);
            }
        }

        private static void DeleteModuleDirectory(string moduleDirectoryPath)
        {
            try
            {
                // recursively delete the directory
                Directory.Delete(moduleDirectoryPath, true);
            }
            catch (Exception exception)
            {
                throw new ExternalModuleException($"Unable to delete the local module directory \"{moduleDirectoryPath}\". {exception.Message}", exception);
            }
        }

        private async Task TryDeleteModuleDirectoryAsync(TModuleReference reference)
        {
            /*
             * The following sections will attempt to synchronize the module directory delete with other
             * instances of the language server running on the same machine.
             *
             * We are not trying to prevent tampering with the module cache by the user.
             */

            var lockFileUri = this.GetModuleLockFileUri(reference);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < ModuleDirectoryContentionTimeout)
            {
                if(!this.FileResolver.FileExists(lockFileUri))
                {
                    // no lock exists, proceed
                    var moduleDirectoryPath = this.GetModuleDirectoryPath(reference);

                    // delete the directory and its contents on disk
                    DeleteModuleDirectory(moduleDirectoryPath);

                    return;
                } else {
                    try {
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
                await Task.Delay(ModuleDirectoryContentionRetryInterval);
            }

            // we have exceeded the timeout
            throw new ExternalModuleException($"Exceeded the timeout of \"{ModuleDirectoryContentionTimeout}\" for the lock on file \"{lockFileUri}\" to be released.");
        }

        // base implementation for cache invalidation that should fit all external registries
        protected async Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateModulesCacheInternal(IEnumerable<TModuleReference> references)
        {
            var statuses = new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Delete module {reference.FullyQualifiedReference} from cache");
                try
                {
                    if(Directory.Exists(GetModuleDirectoryPath(reference))) {
                        await this.TryDeleteModuleDirectoryAsync(reference);
                    }
                }
                catch (Exception exception)
                {
                    if (exception.Message is { } message)
                    {
                        statuses.Add(reference, x => x.ModuleDeleteFailedWithMessage(reference.FullyQualifiedReference, message));
                        timer.OnFail($"Unexpected exception {exception}: {message}");

                        return statuses;
                    }

                    statuses.Add(reference, x => x.ModuleDeleteFailed(reference.FullyQualifiedReference));
                    timer.OnFail($"Unexpected exception {exception}.");
                }
            }

            return statuses;
        }
    }
}
