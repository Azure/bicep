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
    public class InMemoryFileHandle : InMemoryIOHandle, IFileHandle
    {
        public InMemoryFileHandle(InMemoryFileExplorer.FileStore store, IOUri uri)
            : base(store, EnsureNoTrailingSlash(uri))
        {
        }

        public void Delete() => this.FileStore.DeleteFile(this);

        public IFileHandle EnsureExists()
        {
            this.FileStore.CreateFileIfNotExists(this);

            return this;
        }

        public override bool Exists() => this.FileStore.FileExists(this);

        public IDirectoryHandle GetParent()
        {
            var parentUri = this.Uri.Resolve(".");

            return new InMemoryDirectoryHandle(this.FileStore, parentUri);
        }

        public void MakeExecutable() => throw new NotSupportedException();

        public Stream OpenRead()
        {
            var text = this.FileStore.ReadFile(this);

            return new MemoryStream(Encoding.UTF8.GetBytes(text));
        }

        public Stream OpenWrite() => new InMemoryFileStream(text => this.FileStore.WriteFile(this, text));

        public IFileLock? TryLock() => DummyFileLock.Instance;

        private static IOUri EnsureNoTrailingSlash(IOUri uri)
        {
            if (uri.Path.EndsWith('/'))
            {
                throw new ArgumentException("File path must not end with a slash.", nameof(uri));
            }

            return uri;
        }

        private class InMemoryFileStream : MemoryStream
        {
            private readonly Action<string> onDisposing;

            public InMemoryFileStream(Action<string> onDisposing)
            {
                this.onDisposing = onDisposing;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.onDisposing(Encoding.UTF8.GetString(this.ToArray()));
                }

                base.Dispose(disposing);
            }
        }

        private class DummyFileLock : IFileLock
        {
            public static readonly DummyFileLock Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
