// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.IO.FileSystem
{
    public class FileSystemPathException : IOException
    {
        public FileSystemPathException(string message)
            : base(message)
        {
        }

        public static void ThrowIfWhiteSpace(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Throw("The specified path cannot be empty or consists only white-space characters.");
            }
        }

        public static void ThrowIfUnsupportedWindowsDosDevicePath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && FileSystemPathFacts.IsWindowsDosDevicePath(path))
            {
                Throw("Unsupported Windows DOS device path.");
            }
        }

        public static void ThrowIfUnsupportedWindowsUncPath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && FileSystemPathFacts.IsUncPath(path))
            {
                Throw("Unsupported UNC path.");
            }
        }

        public static void ThrowIfContainsUnsupportedWindowsReservedName(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && FileSystemPathFacts.ContainsWindowsReservedFileName(path))
            {
                Throw($"The path contains unsupported Windows reserved file name.");
            }
        }

        [DoesNotReturn]
        private static void Throw(string message) => throw new FileSystemPathException(message);
    }
}
