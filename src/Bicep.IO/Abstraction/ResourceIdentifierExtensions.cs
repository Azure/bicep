// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public static class ResourceIdentifierExtensions
    {
        public static ReadOnlySpan<char> GetExtension(this ResourceIdentifier identifier)
        {
            int lastDotIndex = GetExtensionStartIndex(identifier.Path);

            if (lastDotIndex == -1 || lastDotIndex == identifier.Path.Length - 1)
            {
                return "";
            }

            return identifier.Path.AsSpan()[lastDotIndex..];
        }

        public static ResourceIdentifier WithExtension(this ResourceIdentifier identifier, string extension)
        {
            if (!extension.StartsWith('.'))
            {
                extension = "." + extension;
            }

            var extensionStartIndex = GetExtensionStartIndex(identifier.Path);
            var newPath = extensionStartIndex == -1
                ? identifier.Path + extension
                : string.Concat(identifier.Path.AsSpan(0, extensionStartIndex), extension);

            return new ResourceIdentifier(
                identifier.Scheme,
                identifier.Authority,
                newPath,
                identifier.Query,
                identifier.Fragment);
        }

        private static int GetExtensionStartIndex(string path)
        {
            int extensionStartIndex = -1;

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '.')
                {
                    extensionStartIndex = i;
                }
                else if (path[i] == '/')
                {
                    extensionStartIndex = -1;
                }
            }

            return extensionStartIndex;
        }
    }
}
