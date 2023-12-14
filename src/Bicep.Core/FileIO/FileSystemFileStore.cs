// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.FileIO
{
    public class FileSystemFileStore : IFileStore
    {
        private static readonly ImmutableHashSet<string> WindowsReservedFileNames = ImmutableHashSet.Create(
            StringComparer.OrdinalIgnoreCase,
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9");

        private readonly IFileSystem fileSystem;

        public FileSystemFileStore(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public IFilelock? TryAquireFileLock(FilePath path)
        {
            CheckWindowsReservedFileNames(path);

            try
            {
                // FileMode.OpenOrCreate - we don't want Create because it will also execute a truncate operation in some cases, which is unnecessary
                // FileShare.None - we want locking on the file (even if advisory on some platforms)
                // FileOptions.None - DeleteOnClose is NOT ATOMIC on Linux/Mac and causes race conditions
                var lockStream = fileSystem.FileStream.New(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.None);

                return new FileSystemFileLock(lockStream);
            }
            catch (IOException exception) when (exception.GetType() == typeof(IOException))
            {
                // when file is locked, an IOException is thrown
                // there are other cases where an exception derived from IOException is thrown, but we want to filter them out
                // TODO: What are the other cases where IOException will be thrown?
                return null;
            }
        }

        public string ReadAllText(FilePath path)
        {
            CheckWindowsReservedFileNames(path);
        }

        private void CheckWindowsReservedFileNames(FilePath path)
        {
            /*
             * Win10 (and possibly older versions) will block without returning when
             * reading a file whose name is reserved with or without extension, which breaks the language server.
             *
             * As a workaround, we will simulate Win11+ behavior that throws a FileNotFoundException.
             *
             * https://github.com/Azure/bicep/issues/6224
             */

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var fileName = this.fileSystem.Path.GetFileNameWithoutExtension(path);

            if (!WindowsReservedFileNames.Contains(fileName))
            {
                return;
            }

            // On Win11+ FileNotFoundException is thrown. Simulate similar behavior.
            throw new FileNotFoundException($"Could not find file '{path}'.");
        }
    }
}
