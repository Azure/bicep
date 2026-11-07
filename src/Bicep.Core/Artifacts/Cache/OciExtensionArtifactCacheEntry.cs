// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Extensions;
using Bicep.Core.RegistryClients.Oci;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.Artifacts.Cache
{
    public sealed class OciExtensionArtifactCacheEntry : OciArtifactCacheEntry, IExtensionArtifact
    {
        public OciExtensionArtifactCacheEntry(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(address, rootCacheDirectory)
        {
        }

        public IFileHandle TypesTgzFile => this.GetFile("types.tgz");

        public IFileHandle BinaryFile => this.GetFile("extension.bin");

        public override IFileHandle EntryPoint => this.TypesTgzFile;

        protected override void WriteLayers(OciArtifactResult result, string artifactReference)
        {
            this.TypesTgzFile.Write(result.GetMainLayer().Data);

            if (result is not OciExtensionArtifactResult extension)
            {
                return;
            }

            var config = extension.Config is { } ?
                JsonSerializer.Deserialize(extension.Config.Data, OciExtensionV1ConfigSerializationContext.Default.OciExtensionV1Config) :
                null;

            // if the artifact supports local deployment, fetch the extension binary
            if (config?.LocalDeployEnabled == true &&
                config?.SupportedArchitectures is { } binaryArchitectures)
            {
                if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
                {
                    throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute extension \"{artifactReference}\".");
                }

                if (!binaryArchitectures.Contains(architecture.Name) ||
                    result.TryGetSingleLayerByMediaType(BicepMediaTypes.GetExtensionArtifactLayerV1Binary(architecture)) is not { } binaryData)
                {
                    throw new InvalidOperationException($"The extension \"{artifactReference}\" does not support architecture {architecture.Name}.");
                }

                this.BinaryFile.Write(binaryData);
                this.BinaryFile.MakeExecutable();
            }
        }
    }
}