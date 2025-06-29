// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bicep.IO.FileSystem
{
    public static partial class FileSystemPathFacts
    {
        public static readonly FrozenSet<string> WindowsReservedFileNames = new string[]
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM¹", "COM²", "COM³",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT¹", "LPT²", "LPT³"
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        public static bool IsWindowsReservedFileName(string fileName) => WindowsReservedFileNames.Contains(fileName);

        public static bool IsWindowsDosDevicePath(string path) =>
            path.StartsWith(@"\\.\", StringComparison.Ordinal) ||
            path.StartsWith(@"\\?\", StringComparison.Ordinal);

        public static bool IsUncPath(string path) => UncPathPattern().IsMatch(path);

        public static bool ContainsWindowsReservedFileName(string path)
        {
            var pathSpan = path.AsSpan();
            var segment = ReadOnlySpan<char>.Empty;
            int segmentStartIndex = 0;

            while (segmentStartIndex < pathSpan.Length)
            {
                int separatorIndex = pathSpan[segmentStartIndex..].IndexOfAny('/', '\\');

                if (separatorIndex >= 0)
                {
                    segment = pathSpan.Slice(segmentStartIndex, separatorIndex);
                    segmentStartIndex += separatorIndex + 1;
                }
                else
                {
                    segment = pathSpan[segmentStartIndex..];
                    break;
                }

                if (IsWindowsReservedFileName(segment.ToString()))
                {
                    return true;
                }
            }

            // Get file name without extension.
            if (segment.LastIndexOf(".") is var extensionStartIndex && extensionStartIndex >= 0)
            {
                return IsWindowsReservedFileName(segment[..extensionStartIndex].ToString());
            }

            return false;
        }

        [GeneratedRegex(@"^(?:\\)(:?\\[\w\.-_]+){2,}(?:\\?)$", RegexOptions.CultureInvariant)]
        private static partial Regex UncPathPattern();
    }
}
