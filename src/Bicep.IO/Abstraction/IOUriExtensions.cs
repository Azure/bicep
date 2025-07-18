// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public static class IOUriExtensions
    {
        public static string GetFileName(this IOUri uri) => uri.PathSegments.LastOrDefault() ?? "";

        public static ReadOnlySpan<char> GetFileNameWithoutExtension(this IOUri uri)
        {
            var fileName = uri.PathSegments.LastOrDefault() ?? "";
            int lastDotIndex = GetExtensionStartIndex(fileName);

            if (lastDotIndex == -1)
            {
                return fileName.AsSpan();
            }

            return fileName.AsSpan(0, lastDotIndex);
        }

        public static ReadOnlySpan<char> GetExtension(this IOUri uri)
        {
            int lastDotIndex = GetExtensionStartIndex(uri.Path);

            if (lastDotIndex == -1 || lastDotIndex == uri.Path.Length - 1)
            {
                return "";
            }

            return uri.Path.AsSpan()[lastDotIndex..];
        }

        public static IOUri WithExtension(this IOUri uri, string extension)
        {
            if (!extension.StartsWith('.'))
            {
                extension = "." + extension;
            }

            var extensionStartIndex = GetExtensionStartIndex(uri.Path);
            var newPath = extensionStartIndex == -1
                ? uri.Path + extension
                : string.Concat(uri.Path.AsSpan(0, extensionStartIndex), extension);

            return new IOUri(
                uri.Scheme,
                uri.Authority,
                newPath,
                uri.Query,
                uri.Fragment);
        }

        public static bool HasExtension(this IOUri uri, string extension)
        {
            if (!extension.StartsWith('.'))
            {
                extension = "." + extension;
            }

            var actualExtension = GetExtension(uri);

            return actualExtension.Equals(extension, StringComparison.OrdinalIgnoreCase);
        }

        private static int GetExtensionStartIndex(string path)
        {
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '.')
                {
                    return i;
                }
                else if (path[i] == '/')
                {
                    return -1;
                }
            }

            return -1;
        }
    }
}
