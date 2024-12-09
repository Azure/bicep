// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.FileSystem
{
    public sealed class FileSystemFileHandle : FileSystemIOHandle, IFileHandle
    {
        public FileSystemFileHandle(IFileSystem fileSystem, string path)
            : base(fileSystem, path)
        {
        }

        public override bool Exists() => this.FileSystem.File.Exists(Uri.GetFileSystemPath());

        public IDirectoryHandle GetParent()
        {
            var parentDirectoryPath = this.FileSystem.Path.GetDirectoryName(Uri.GetFileSystemPath());

            if (string.IsNullOrEmpty(parentDirectoryPath))
            {
                throw new UnreachableException("The file must have a parent directory.");
            }

            return new FileSystemDirectoryHandle(this.FileSystem, parentDirectoryPath);
        }

        public Stream OpenRead() => this.FileSystem.File.OpenRead(Uri.GetFileSystemPath());

        public Stream OpenWrite()
        {
            this.GetParent().EnsureExists();

            return this.FileSystem.File.OpenWrite(Uri.GetFileSystemPath());
        }

        public IFileLock? TryLock() => FileSystemStreamLock.TryCreate(this.FileSystem, Uri.GetFileSystemPath());
    }
}
