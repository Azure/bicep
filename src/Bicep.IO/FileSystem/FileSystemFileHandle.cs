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
    public sealed class FileSystemFileHandle : FileSystemResourceHandle, IFileHandle
    {
        public FileSystemFileHandle(IFileSystem fileSystem, string path)
            : base(fileSystem, path)
        {
        }

        public override bool Exists() => this.FileSystem.File.Exists(Identifier.GetFileSystemPath());

        public IDirectoryHandle GetParent()
        {
            var parentDirectoryPath = this.FileSystem.Path.GetDirectoryName(Identifier.GetFileSystemPath());

            if (string.IsNullOrEmpty(parentDirectoryPath))
            {
                throw new UnreachableException("The file must have a parent directory.");
            }

            return new FileSystemDirectoryHandle(this.FileSystem, parentDirectoryPath);
        }

        public Stream OpenRead() => this.FileSystem.File.OpenRead(Identifier.GetFileSystemPath());

        public Stream OpenWrite()
        {
            this.GetParent().EnsureExists();

            return this.FileSystem.File.OpenWrite(Identifier.GetFileSystemPath());
        }

        public IFileLock? TryLock() => FileSystemStreamLock.TryCreate(this.FileSystem, Identifier.GetFileSystemPath());
    }
}
