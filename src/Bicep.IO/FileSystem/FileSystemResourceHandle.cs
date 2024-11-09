// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.IO.FileSystem
{
    public abstract class FileSystemResourceHandle : IResourceHandle
    {
        protected FileSystemResourceHandle(IFileSystem fileSystem, string fileSystemPath)
            : this(fileSystem, CreateIdentifier(fileSystem, fileSystemPath))
        {
        }

        private FileSystemResourceHandle(IFileSystem fileSystem, ResourceIdentifier identifier)
        {
            FileSystem = fileSystem;
            Identifier = identifier;
        }

        protected IFileSystem FileSystem { get; }

        public ResourceIdentifier Identifier { get; }

        public abstract bool Exists();

        public override string ToString() => Identifier.GetFileSystemPath();

        public override int GetHashCode() => HashCode.Combine(this.GetType(), Identifier);

        public bool Equals(IResourceHandle? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.GetType() == other.GetType() && Identifier == other.Identifier;
        }

        private static ResourceIdentifier CreateIdentifier(IFileSystem fileSystem, string fileSystemPath)
        {
            FileSystemPathException.ThrowIfWhiteSpace(fileSystemPath);
            FileSystemPathException.ThrowIfUnsupportedWindowsDosDevicePath(fileSystemPath);
            FileSystemPathException.ThrowIfUnsupportedWindowsUncPath(fileSystemPath);
            FileSystemPathException.ThrowIfContainsUnsupportedWindowsReservedName(fileSystemPath);

            try
            {
                // canolicalizes and validates the path.
                fileSystemPath = fileSystem.Path.GetFullPath(fileSystemPath);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Only normalized path separators on Windows because '\' is a valid file name character on Linux / macOS.
                    fileSystemPath = "/" + fileSystemPath.Replace('\\', '/');
                }

                return new ResourceIdentifier("file", "", fileSystemPath);
            }
            catch (Exception exception) when (exception is ArgumentException or SecurityException or NotSupportedException or PathTooLongException)
            {
                throw new FileSystemPathException(exception.Message);
            }
        }

    }
}
