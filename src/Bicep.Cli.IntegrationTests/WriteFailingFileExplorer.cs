// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Cli.IntegrationTests;

internal sealed class WriteFailingFileExplorer(
    IFileExplorer inner,
    string outputFileName,
    Exception exception) : IFileExplorer
{
    public IDirectoryHandle GetDirectory(IOUri uri) => inner.GetDirectory(uri);

    public IFileHandle GetFile(IOUri uri)
    {
        var file = inner.GetFile(uri);
        var fileName = uri.GetFileName();
        return fileName.Equals(outputFileName, StringComparison.OrdinalIgnoreCase)
            ? new WriteFailingFileHandle(file, exception)
            : file;
    }

    private sealed class WriteFailingFileHandle(IFileHandle inner, Exception exception) : IFileHandle
    {
        public IOUri Uri => inner.Uri;

        public bool Exists() => inner.Exists();

        public string ReadAllText() => inner.ReadAllText();

        public Task<string> ReadAllTextAsync(CancellationToken cancellationToken = default) =>
            inner.ReadAllTextAsync(cancellationToken);

        public bool Equals(IIOHandle? other) => inner.Equals(other);

        public IDirectoryHandle GetParent() => inner.GetParent();

        public IFileHandle EnsureExists() => inner.EnsureExists();

        public Stream OpenRead() => inner.OpenRead();

        public Stream OpenWrite() => throw exception;

        public void WriteAllText(string text) => throw exception;

        public Task WriteAllTextAsync(string text, CancellationToken cancellationToken = default) =>
            Task.FromException(exception);

        public void Delete() => inner.Delete();

        public void MakeExecutable() => inner.MakeExecutable();

        public IFileLock? TryLock() => inner.TryLock();
    }
}
