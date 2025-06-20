// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.SourceGraph.Artifacts
{
    internal class LocalExtensionArtifact : ExternalArtifact, IExtensionArtifact
    {
        public LocalExtensionArtifact(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
            : base(ResolveCacheDirectory(extensionBinaryData, rootCacheDirectory))
        {
        }

        public IFileHandle TypesTgzFile => this.GetFile("types.tgz");

        public IFileHandle BinaryFile => this.GetFile("extension.bin");

        private static IDirectoryHandle ResolveCacheDirectory(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
        {
            // The cache directory is structured as "local/{extensionDigest}"
            // where extensionDigest is a unique identifier for the extension.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, extensionBinaryData, separator: '_');

            return rootCacheDirectory.GetDirectory($"local/{digest}");
        }
    }
}
