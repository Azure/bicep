// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;

namespace Bicep.Core.FileIO
{
    public sealed class FileSystemFileLock : IFilelock
    {
        private readonly Stream lockStream;

        public FileSystemFileLock(Stream lockStream) => this.lockStream = lockStream;

        public void Dispose() => lockStream.Dispose();
    }
}
