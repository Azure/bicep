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
    public sealed class BicepRegistryExtensionArtifact : BicepRegistryArtifact, IExtensionArtifact
    {
        public BicepRegistryExtensionArtifact(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(address, rootCacheDirectory)
        {
        }

        public IFileHandle TypesTgzFile => this.GetFile("types.tgz");

        public IFileHandle BinaryFile => this.GetFile("extension.bin");
    }
}
