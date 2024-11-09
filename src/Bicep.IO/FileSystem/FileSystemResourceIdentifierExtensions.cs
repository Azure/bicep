// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.FileSystem
{
    public static class FileSystemResourceIdentifierExtensions
    {
        public static string GetFileSystemPath(this ResourceIdentifier identifier)
        {
            if (!identifier.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException($"Unsupported file identifier scheme '{identifier.Scheme}'.");
            }

            if (identifier.Authority.Length != 0)
            {
                throw new NotSupportedException($"Unsupported file identifier authority '{identifier.Authority}'.");
            }

            var uriBuilder = new UriBuilder
            {
                Scheme = identifier.Scheme,
                Host = identifier.Authority,
                Path = identifier.Path,
            };

            return uriBuilder.Uri.LocalPath;
        }
    }
}
