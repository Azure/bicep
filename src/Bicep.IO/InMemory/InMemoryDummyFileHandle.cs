// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public class InMemoryDummyFileHandle : IFileHandle
    {
        public static readonly InMemoryDummyFileHandle Instance = new();

        private InMemoryDummyFileHandle()
        {
        }

        public IOUri Uri { get; } = new IOUri("inmemory", "", "/DUMMY");

        public void Delete() => throw new UnreachableException();

        public IFileHandle EnsureExists() => throw new UnreachableException();

        public bool Equals(IIOHandle? other) => throw new UnreachableException();

        public bool Exists() => throw new UnreachableException();

        public IDirectoryHandle GetParent() => throw new UnreachableException();

        public void MakeExecutable() => throw new UnreachableException();

        public Stream OpenRead() => throw new UnreachableException();

        public Stream OpenWrite() => throw new UnreachableException();

        public IFileLock? TryLock() => throw new UnreachableException();
    }
}
