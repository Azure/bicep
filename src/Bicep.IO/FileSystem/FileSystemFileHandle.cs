// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.FileSystem
{
    public sealed class FileSystemFileHandle : FileSystemIOHandle, IFileHandle
    {
        public FileSystemFileHandle(IFileSystem fileSystem, IOUri uri)
            : base(fileSystem, EnsureNoTrailingSlash(uri))
        {
        }

        public override bool Exists() => this.FileSystem.File.Exists(this.FilePath);

        public IFileHandle EnsureExists()
        {
            this.GetParent().EnsureExists();

            using (this.FileSystem.File.Open(this.FilePath, FileMode.Append, FileAccess.Write))
            {
            }

            return this;
        }

        public Stream OpenRead() => this.FileSystem.File.OpenRead(this.FilePath);

        public Stream OpenWrite()
        {
            this.GetParent().EnsureExists();

            return this.FileSystem.FileStream.New(this.FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        public string ReadAllText() => this.FileSystem.File.ReadAllText(this.FilePath);

        public Task<string> ReadAllTextAsync() => this.FileSystem.File.ReadAllTextAsync(this.FilePath);

        public void WriteAllText(string text)
        {
            this.GetParent().EnsureExists();
            this.FileSystem.File.WriteAllText(this.FilePath, text);
        }

        public async Task WriteAllTextAsync(string text)
        {
            this.GetParent().EnsureExists();

            await this.FileSystem.File.WriteAllTextAsync(this.FilePath, text);
        }

        public void Delete() => this.FileSystem.File.Delete(this.FilePath);

        public void MakeExecutable()
        {
            if (!OperatingSystem.IsWindows())
            {
                this.FileSystem.File.SetUnixFileMode(this.FilePath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }
        }

        public IDirectoryHandle GetParent()
        {
            var parentUri = this.Uri.Resolve(".");

            return new FileSystemDirectoryHandle(this.FileSystem, parentUri);
        }

        public IFileLock? TryLock() => FileSystemStreamLock.TryCreate(this.FileSystem, this.FilePath);

        private static IOUri EnsureNoTrailingSlash(IOUri uri)
        {
            if (uri.Path.EndsWith('/'))
            {
                throw new IOException($"File path '{uri}' must not end with a slash.");
            }

            return uri;
        }
    }
}
