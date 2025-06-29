// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public class DummyFileHandle : IFileHandle
    {
        public static readonly DummyFileHandle Instance = new();

        private DummyFileHandle()
        {
        }

        public IOUri Uri { get; } = new IOUri("dummy", "", "/DUMMY");

        public void Delete() => throw new UnreachableException();

        public IFileHandle EnsureExists() => throw new UnreachableException();

        public bool Equals(IIOHandle? other) => throw new UnreachableException();

        public bool Exists() => throw new UnreachableException();

        public IDirectoryHandle GetParent() => throw new FileNotFoundException();

        public void MakeExecutable() => throw new UnreachableException();

        public Stream OpenRead() => throw new UnreachableException();

        public Stream OpenWrite() => throw new UnreachableException();

        public IFileLock? TryLock() => throw new UnreachableException();
    }
}
