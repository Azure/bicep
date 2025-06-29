// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.FileSystem
{
    public sealed class FileSystemFileExplorer : IFileExplorer
    {
        private readonly IFileSystem fileSystem;

        public FileSystemFileExplorer(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public IDirectoryHandle GetDirectory(IOUri uri) => new FileSystemDirectoryHandle(fileSystem, uri);

        public IFileHandle GetFile(IOUri uri) => new FileSystemFileHandle(this.fileSystem, uri);
    }
}
