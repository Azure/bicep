// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public class DummyFileHandle : IFileHandle
    {
        public static readonly DummyFileHandle Instance = new();

        private DummyFileHandle()
        {
        }

        public IOUri Uri { get; } = new IOUri("inmemory", "", "dummy");

        public void Delete() => throw new NotSupportedException();

        public IFileHandle EnsureExists() => this;

        public bool Equals(IIOHandle? other) => this == other;

        public bool Exists() => true;

        public IDirectoryHandle GetParent() => throw new NotSupportedException();

        public void MakeExecutable() => throw new NotSupportedException();

        public Stream OpenRead() => throw new NotSupportedException();

        public Stream OpenWrite() => throw new NotSupportedException();

        public IFileLock? TryLock() => throw new NotSupportedException();
    }
}
