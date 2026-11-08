// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Registry;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph.Artifacts
{
    /// <summary>
    /// An artifact's entry in the local cache. It owns the entry's directory and file names, which older Bicep versions
    /// on the same machine also read and write, so they must not change.
    /// </summary>
    /// <remarks>
    /// Entries are never updated in place: they are written once under a lock and only deleted as a whole, so a complete
    /// entry can be read without locking.
    /// </remarks>
    public abstract class ExternalArtifact
    {
        // If another process holds the lock on the cache directory, retry until this timeout is reached.
        private static readonly TimeSpan LockContentionTimeout = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan LockContentionRetryInterval = TimeSpan.FromMilliseconds(300);

        protected ExternalArtifact(IDirectoryHandle cacheDirectory)
        {
            this.CacheDirectory = cacheDirectory;
        }

        public IDirectoryHandle CacheDirectory { get; }

        /// <summary>
        /// Gets the file the compiler loads for the artifact. It only exists once the entry is complete.
        /// </summary>
        public abstract IFileHandle EntryPoint { get; }

        /// <summary>
        /// Gets a value indicating whether every file of the entry has been written.
        /// </summary>
        public abstract bool IsComplete { get; }

        private IFileHandle LockFile => this.GetFile("lock");

        /// <summary>
        /// Deletes the entry, waiting for another process that is writing it to finish.
        /// </summary>
        /// <exception cref="ExternalArtifactException">The entry could not be deleted.</exception>
        public async Task DeleteAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < LockContentionTimeout)
            {
                if (!this.LockFile.Exists())
                {
                    try
                    {
                        this.CacheDirectory.Delete();
                    }
                    catch (Exception exception)
                    {
                        throw new ExternalArtifactException($"Unable to delete the local artifact directory \"{this.CacheDirectory.Uri}\". {exception.Message}", exception);
                    }

                    return;
                }

                try
                {
                    // A released lock can leave its file behind (see FileLock), so try to delete it. If that fails,
                    // the lock is still held, and we wait for the other process to finish.
                    this.LockFile.Delete();
                }
                catch (IOException)
                {
                    break;
                }

                await Task.Delay(LockContentionRetryInterval);
            }

            throw new ExternalArtifactException($"Exceeded the timeout of \"{LockContentionTimeout}\" for the lock on file \"{this.LockFile.Uri}\" to be released.");
        }

        protected IFileHandle GetFile(string fileName) => this.CacheDirectory.GetFile(fileName);

        /// <summary>
        /// Writes the entry under the cache lock, unless another compilation, possibly in another process, already completed it.
        /// </summary>
        /// <exception cref="ExternalArtifactException">The cache directory could not be created or locked.</exception>
        protected async Task WriteAsync(Action writeContent)
        {
            this.EnsureCacheDirectoryExists();

            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < LockContentionTimeout)
            {
                if (this.TryWriteUnderLock(writeContent))
                {
                    return;
                }

                await Task.Delay(LockContentionRetryInterval);
            }

            throw this.CreateLockTimeoutException();
        }

        /// <inheritdoc cref="WriteAsync(Action)"/>
        protected void Write(Action writeContent)
        {
            this.EnsureCacheDirectoryExists();

            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < LockContentionTimeout)
            {
                if (this.TryWriteUnderLock(writeContent))
                {
                    return;
                }

                Thread.Sleep(LockContentionRetryInterval);
            }

            throw this.CreateLockTimeoutException();
        }

        private void EnsureCacheDirectoryExists()
        {
            try
            {
                // Creating the directory doesn't require the lock.
                this.CacheDirectory.EnsureExists();
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException($"Unable to create the local artifact directory \"{this.CacheDirectory.Uri}\". {exception.Message}", exception);
            }
        }

        private bool TryWriteUnderLock(Action writeContent)
        {
            // Disposing the lock releases it even if the completeness check or the write throws.
            using var @lock = this.LockFile.TryLock();

            if (@lock is null)
            {
                // Another process is writing the same content; give it time to finish.
                return false;
            }

            if (!this.IsComplete)
            {
                writeContent();
            }

            return true;
        }

        private ExternalArtifactException CreateLockTimeoutException() =>
            new($"Exceeded the timeout of \"{LockContentionTimeout}\" to acquire the lock on file \"{this.LockFile.Uri}\".");
    }
}
