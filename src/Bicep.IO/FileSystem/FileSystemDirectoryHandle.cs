// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.IO.FileSystem
{
    public class FileSystemDirectoryHandle : FileSystemIOHandle, IDirectoryHandle
    {
        public FileSystemDirectoryHandle(IFileSystem fileSystem, IOUri uri)
            : base(fileSystem, EnsureTrailingSlash(uri))
        {
        }

        public override bool Exists() => this.FileSystem.Directory.Exists(this.FilePath);

        public IDirectoryHandle EnsureExists()
        {
            this.FileSystem.Directory.CreateDirectory(this.FilePath);
            return this;
        }

        public void Delete() => this.FileSystem.Directory.Delete(this.FilePath, recursive: true);

        public IDirectoryHandle? GetParent()
        {
            var parentUri = this.Uri.Resolve("..");
            return this.Uri == parentUri ? null : new FileSystemDirectoryHandle(this.FileSystem, parentUri);
        }

        public IDirectoryHandle GetDirectory(string relativePath)
        {
            var directoryUri = this.Uri.Resolve(relativePath);
            return new FileSystemDirectoryHandle(this.FileSystem, directoryUri);
        }

        public IFileHandle GetFile(string relativePath)
        {
            var fileUri = this.Uri.Resolve(relativePath);
            return new FileSystemFileHandle(this.FileSystem, fileUri);
        }

        private static IOUri EnsureTrailingSlash(IOUri uri) => uri.Path.EndsWith('/') ? uri : new IOUri(uri.Scheme, uri.Authority, uri.Path + "/");

        public IEnumerable<IDirectoryHandle> EnumerateDirectories(string searchPattern = "")
        {
            var directories = this.FileSystem.Directory.EnumerateDirectories(this.FilePath, searchPattern);

            foreach (var directory in directories)
            {
                var directoryUri = IOUri.FromFilePath(directory);
                yield return new FileSystemDirectoryHandle(this.FileSystem, directoryUri);
            }
        }

        public IEnumerable<IFileHandle> EnumerateFiles(string searchPattern = "")
        {
            var files = this.FileSystem.Directory.EnumerateFiles(this.FilePath, searchPattern);

            foreach (var file in files)
            {
                var fileUri = IOUri.FromFilePath(file);
                yield return new FileSystemFileHandle(this.FileSystem, fileUri);
            }
        }
    }
}
