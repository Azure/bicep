// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.ArtifactCache
{
    public class LocalExtensionCacheAccessor
    {
        private readonly IDirectoryHandle cacheDirectory;

        public LocalExtensionCacheAccessor(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
        {
            this.cacheDirectory = ResolveCacheDirectory(extensionBinaryData, rootCacheDirectory);
        }

        public IFileHandle TypesTgzFile => this.cacheDirectory.GetFile("types.tgz");

        public static IDirectoryHandle ResolveCacheDirectory(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
        {
            // The cache directory is structured as "local/{extensionDigest}"
            // where extensionDigest is a unique identifier for the extension.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, extensionBinaryData, separator: '_');

            return rootCacheDirectory.GetDirectory($"local/{digest}");
        }
    }
}
