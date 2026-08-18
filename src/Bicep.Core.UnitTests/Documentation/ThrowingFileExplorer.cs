// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.UnitTests.Documentation;

internal sealed class ThrowingFileExplorer(Exception exceptionToThrow) : IFileExplorer
{
    public IDirectoryHandle GetDirectory(IOUri uri) => throw new NotSupportedException();

    public IFileHandle GetFile(IOUri uri) => new ThrowingFileHandle(uri, exceptionToThrow);

    private sealed class ThrowingFileHandle(IOUri uri, Exception exceptionToThrow) : IFileHandle
    {
        public IOUri Uri { get; } = uri;

        public bool Exists() => true;

        public string ReadAllText() => throw exceptionToThrow;

        public Task<string> ReadAllTextAsync(CancellationToken cancellationToken = default) => throw exceptionToThrow;

        public bool Equals(IIOHandle? other) => other is ThrowingFileHandle otherHandle && Uri.Equals(otherHandle.Uri);

        public IDirectoryHandle GetParent() => throw new NotSupportedException();

        public IFileHandle EnsureExists() => throw new NotSupportedException();

        public Stream OpenRead() => throw new NotSupportedException();

        public Stream OpenWrite() => throw new NotSupportedException();

        public void WriteAllText(string text) => throw new NotSupportedException();

        public Task WriteAllTextAsync(string text, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public void Delete() => throw new NotSupportedException();

        public void MakeExecutable() => throw new NotSupportedException();

        public IFileLock? TryLock() => throw new NotSupportedException();
    }
}

internal sealed class SelectivelyThrowingFileExplorer(IFileExplorer inner, IOUri throwingUri, Exception exceptionToThrow) : IFileExplorer
{
    public IDirectoryHandle GetDirectory(IOUri uri) => inner.GetDirectory(uri);

    public IFileHandle GetFile(IOUri uri) => uri.Equals(throwingUri)
        ? new ThrowingFileExplorer(exceptionToThrow).GetFile(uri)
        : inner.GetFile(uri);
}
