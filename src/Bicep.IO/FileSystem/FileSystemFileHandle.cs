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
        public FileSystemFileHandle(IFileSystem fileSystem, string path)
            : base(fileSystem, path)
        {
        }

        public override bool Exists() => this.FileSystem.File.Exists(this.Uri.GetFileSystemPath());

        public IFileHandle EnsureExists()
        {
            using (this.FileSystem.File.Open(this.Uri.GetFileSystemPath(), FileMode.Append, FileAccess.Write))
            {
            }

            return this;
        }

        public IDirectoryHandle GetParent()
        {
            var parentDirectoryPath = this.FileSystem.Path.GetDirectoryName(this.Uri.GetFileSystemPath());

            if (string.IsNullOrEmpty(parentDirectoryPath))
            {
                throw new UnreachableException("The file must have a parent directory.");
            }

            return new FileSystemDirectoryHandle(this.FileSystem, parentDirectoryPath);
        }

        public Stream OpenRead() => this.FileSystem.File.OpenRead(this.Uri.GetFileSystemPath());

        public Stream OpenWrite()
        {
            this.GetParent().EnsureExists();

            return this.FileSystem.File.OpenWrite(this.Uri.GetFileSystemPath());
        }

        public void Delete() => this.FileSystem.File.Delete(this.Uri.GetFileSystemPath());

        public void MakeExecutable()
        {
            if (!OperatingSystem.IsWindows())
            {
                this.FileSystem.File.SetUnixFileMode(this.Uri.GetFileSystemPath(), UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }
        }

        public IFileLock? TryLock() => FileSystemStreamLock.TryCreate(this.FileSystem, this.Uri.GetFileSystemPath());
    }
}
