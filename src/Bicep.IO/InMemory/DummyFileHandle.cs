// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public class DummyFileHandle : IFileHandle
    {
        public static readonly DummyFileHandle Default = new(new IOUri("dummy", "", "/DUMMY"));

        public DummyFileHandle(IOUri uri)
        {
            this.Uri = uri;
        }

        public IOUri Uri { get; }

        public void Delete() => throw new UnreachableException();

        public IFileHandle EnsureExists() => throw new UnreachableException();

        public bool Equals(IIOHandle? other) => throw new UnreachableException();

        public bool Exists() => throw new UnreachableException();

        public IDirectoryHandle GetParent() => throw new FileNotFoundException();

        public void MakeExecutable() => throw new UnreachableException();

        public Stream OpenRead() => throw new UnreachableException();

        public Stream OpenWrite() => throw new UnreachableException();

        public string ReadAllText() => throw new UnreachableException();

        public Task<string> ReadAllTextAsync() => throw new UnreachableException();

        public void WriteAllText(string text) => throw new UnreachableException();

        public Task WriteAllTextAsync(string text) => throw new UnreachableException();

        public IFileLock? TryLock() => throw new UnreachableException();
    }
}
