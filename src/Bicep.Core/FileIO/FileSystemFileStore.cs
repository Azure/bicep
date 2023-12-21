// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Constants;
using Bicep.Core.FileIO.Abstractions;
using Bicep.Core.Platform;
using Bicep.Core.Utils;

namespace Bicep.Core.FileIO
{
    public class FileSystemFileStore : IFileStore
    {
        private const char DirectorySeparator = '/';

        private static readonly bool PathCaseSensitive = OperationSystemInformation.IsLinux;

        private static readonly StringComparison PathComparison = PathCaseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        private readonly IFileSystem fileSystem;

        public FileSystemFileStore(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public FilePointer Parse(string path)
        {
            // Normalize Windows directory separator.
            path = path.Replace('\\', DirectorySeparator);

            if (path.IndexOfAny(this.fileSystem.Path.GetInvalidPathChars()) >= 0)
            {
                throw new IOException($"The path '{path}' contains invalid path character(s).");
            }

            var kind = path.StartsWith(DirectorySeparator) ? FilePointerKind.Absolute : FilePointerKind.Relative;

            if (OperationSystemInformation.IsWindows)
            {
                if (path.Split(DirectorySeparator).Any(OperationSystemInformation.IsWindowsReservedFileName))
                {
                    throw new IOException($"The path '{path}' contains one or more Windows reserved file names.");
                }

                if (path.Length >= 2 && char.IsAsciiLetter(path[0]) && path[1] == ':')
                {
                    // Normalize Windows drive letter
                    path = char.ToLowerInvariant(path[0]) + path[1..];

                    if (path.Length >= 3 && path[2] == DirectorySeparator)
                    {
                        kind = FilePointerKind.Absolute;
                    }
                }
            }

            return new(path, kind);
        }

        public FilePointer Combine(FilePointer basePointer, FilePointer relativePointer)
        {
            if (basePointer.IsRelative)
            {
                throw new ArgumentException($"Expected {nameof(basePointer)} to be an absolute path.");
            }

            if (relativePointer.IsAbsolute)
            {
                throw new ArgumentException($"Expected {nameof(relativePointer)} to be a relative path.");
            }

            return new(this.fileSystem.Path.Combine(basePointer, relativePointer), FilePointerKind.Absolute);
        }

        public FileKind GetFileKind(FilePointer path)
        {
            var extension = this.fileSystem.Path.GetExtension(path);

            return FileKind.FromExtension(extension);
        }

        public int Compare(FilePointer x, FilePointer y) => string.Compare(x, y, PathComparison);

        public bool Equals(FilePointer x, FilePointer y) => string.Equals(x, y, PathComparison);

        public int GetHashCode([DisallowNull] FilePointer path) => PathCaseSensitive
            ? path.Value.GetHashCode()
            : path.Value.ToLowerInvariant().GetHashCode();

        public IFilelock? TryAquireFileLock(FilePointer path)
        {
            try
            {
                // FileMode.OpenOrCreate - we don't want Create because it will also execute a truncate operation in some cases, which is unnecessary
                // FileShare.None - we want locking on the file (even if advisory on some platforms)
                // FileOptions.None - DeleteOnClose is NOT ATOMIC on Linux/Mac and causes race conditions
                var lockStream = fileSystem.FileStream.New(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.None);

                return new StreamFileLock(lockStream);
            }
            catch (IOException exception) when (exception.GetType() == typeof(IOException))
            {
                // when file is locked, an IOException is thrown
                // there are other cases where an exception derived from IOException is thrown, but we want to filter them out
                // TODO: What are the other cases where IOException will be thrown?
                return null;
            }
        }

        public string ReadAllText(FilePointer path) => this.fileSystem.File.ReadAllText(path);

        public IEnumerable<string> ReadLines(FilePointer path) => this.fileSystem.File.ReadLines(path);

        public void WriteStream(FilePointer path, Stream stream)
        {
            using var fileStream = fileSystem.File.Open(path, FileMode.Create);
            stream.CopyTo(fileStream);
        }
    }
}
