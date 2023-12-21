// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Platform
{
    public static class OperationSystemInformation
    {
        public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        
        // TODO: Convert to FrozenSet after migrating to .NET 8.
        public static readonly ImmutableHashSet<string> WindowsReservedFileNames = ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase,
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM¹", "COM²", "COM³",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT¹", "LPT²", "LPT³");

        public static bool IsWindowsReservedFileName(string fileName) => WindowsReservedFileNames.Contains(fileName);
    }
}
