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
        public static ReadOnlySpan<char> GetExtension(this IOUri identifier)
        {
            int lastDotIndex = GetExtensionStartIndex(identifier.Path);

            if (lastDotIndex == -1 || lastDotIndex == identifier.Path.Length - 1)
            {
                return "";
            }

            return identifier.Path.AsSpan()[lastDotIndex..];
        }

        public static IOUri WithExtension(this IOUri identifier, string extension)
        {
            if (!extension.StartsWith('.'))
            {
                extension = "." + extension;
            }

            var extensionStartIndex = GetExtensionStartIndex(identifier.Path);
            var newPath = extensionStartIndex == -1
                ? identifier.Path + extension
                : string.Concat(identifier.Path.AsSpan(0, extensionStartIndex), extension);

            return new IOUri(
                identifier.Scheme,
                identifier.Authority,
                newPath,
                identifier.Query,
                identifier.Fragment);
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
