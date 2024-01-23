// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;

namespace Bicep.Core.FileSystem
{
    public sealed class FileLock : IDisposable
    {
        private readonly FileSystemStream lockStream;

        private FileLock(FileSystemStream lockStream)
        {
            this.lockStream = lockStream;
        }

        public void Dispose()
        {
            this.lockStream.Dispose();
        }

        public static FileLock? TryAcquire(IFileSystem fileSystem, string name)
        {
            try
            {
                // FileMode.OpenOrCreate - we don't want Create because it will also execute a truncate operation in some cases, which is unnecessary
                // FileShare.None - we want locking on the file (even if advisory on some platforms)
                // FileOptions.None - DeleteOnClose is NOT ATOMIC on Linux/Mac and causes race conditions
                var lockStream = fileSystem.FileStream.New(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.None);

                return new FileLock(lockStream);
            }
            catch (IOException exception) when (exception.GetType() == typeof(IOException))
            {
                // when file is locked, an IOException is thrown
                // there are other cases where an exception derived from IOException is thrown, but we want to filter them out
                // TODO: What are the other cases where IOException will be thrown?
                return null;
            }
        }
    }
}
