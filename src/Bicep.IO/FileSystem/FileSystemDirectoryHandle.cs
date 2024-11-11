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
    public class FileSystemDirectoryHandle : FileSystemResourceHandle, IDirectoryHandle
    {
        public FileSystemDirectoryHandle(IFileSystem fileSystem, string fileSystemPath)
            : base(fileSystem, EnsureTrailingSlash(fileSystem, fileSystemPath))
        {
        }

        public override bool Exists() => FileSystem.Directory.Exists(Identifier.GetFileSystemPath());

        public IDirectoryHandle GetDirectory(string relativePath)
        {
            var directoryPath = GetFullPath(relativePath);

            return new FileSystemDirectoryHandle(FileSystem, directoryPath);
        }

        public IFileHandle GetFile(string relativePath)
        {
            var filePath = GetFullPath(relativePath);

            return new FileSystemFileHandle(FileSystem, filePath);
        }

        public IDirectoryHandle? GetParent()
        {
            var currentPath = Identifier.GetFileSystemPath().TrimEnd(this.FileSystem.Path.DirectorySeparatorChar);
            var parentDirectoryPath = this.FileSystem.Path.GetDirectoryName(currentPath);

            if (string.IsNullOrEmpty(parentDirectoryPath))
            {
                return null;
            }

            return new FileSystemDirectoryHandle(FileSystem, parentDirectoryPath);
        }

        private string GetFullPath(string relativePath)
        {
            if (FileSystem.Path.IsPathRooted(relativePath))
            {
                throw new FileSystemPathException("Path must be relative.");
            }

            try
            {
                return FileSystem.Path.GetFullPath(relativePath, basePath: Identifier.GetFileSystemPath());
            }
            catch (Exception exception) when (exception is ArgumentException)
            {
                throw new FileSystemPathException(exception.Message);
            }
        }

        private static string EnsureTrailingSlash(IFileSystem fileSystem, string path) =>
            path.EndsWith(fileSystem.Path.DirectorySeparatorChar) ? path : path + fileSystem.Path.DirectorySeparatorChar;
    }
}
