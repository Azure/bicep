// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.SourceGraph.Artifacts
{
    public sealed class BicepRegistryModuleArtifact : BicepRegistryArtifact
    {
        public BicepRegistryModuleArtifact(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(address, rootCacheDirectory)
        {
        }

        public IFileHandle MainTemplateFile => this.GetFile("main.json");

        public IFileHandle SourceTgzFile => this.GetFile("source.tgz");
    }
}
