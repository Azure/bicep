// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;

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

        public override IFileHandle EntryPoint => this.MainTemplateFile;

        protected override void WriteLayers(OciArtifactResult result, string artifactReference)
        {
            this.MainTemplateFile.Write(result.GetMainLayer().Data);

            if (result.TryGetSingleLayerByMediaType(BicepMediaTypes.BicepSourceV1Layer) is { } sourceData)
            {
                this.SourceTgzFile.Write(sourceData);
            }
        }
    }
}
