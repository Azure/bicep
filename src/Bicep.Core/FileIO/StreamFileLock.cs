// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.IO.Abstractions;
using Bicep.Core.FileIO.Abstractions;

namespace Bicep.Core.FileIO
{
    public sealed class StreamFileLock : IFilelock
    {
        private readonly Stream lockStream;

        public StreamFileLock(Stream lockStream) => this.lockStream = lockStream;

        public void Dispose() => lockStream.Dispose();
    }
}
