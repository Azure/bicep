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
            var text = this.FileStore.ReadFile(this).ToString();

            return new MemoryStream(Encoding.UTF8.GetBytes(text));
        }

        public Stream OpenAsyncRead() => this.OpenRead();

        public Stream OpenWrite() => new InMemoryFileStream(data => this.FileStore.WriteFile(this, data));

        public Stream OpenAsyncWrite() => this.OpenWrite();

        public string ReadAllText() => this.FileStore.ReadFile(this).ToString();

        public Task<string> ReadAllTextAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.ReadAllText());
        }

        public void WriteAllText(string text) => this.FileStore.WriteFile(this, text);

        public Task WriteAllTextAsync(string text, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.WriteAllText(text);

            return Task.CompletedTask;
        }

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
            private readonly Action<BinaryData> onDisposing;

            public InMemoryFileStream(Action<BinaryData> onDisposing)
            {
                this.onDisposing = onDisposing;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.onDisposing(BinaryData.FromBytes(this.ToArray()));
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
