// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
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

        /// <summary>
        /// Writes a pulled extension into the cache, with this machine's binary if it has binaries, unless another
        /// compilation already has.
        /// </summary>
        /// <param name="artifactReference">The reference, for error messages.</param>
        public Task WriteAsync(ExtensionArtifactPackage package, OciArtifactPayload payload, string artifactReference) => this.WriteAsync(payload, () =>
        {
            this.TypesTgzFile.Write(package.Types);

            if (package.Binaries.Any())
            {
                if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
                {
                    throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute extension \"{artifactReference}\".");
                }

                if (package.Binaries.FirstOrDefault(x => x.Architecture.Name == architecture.Name) is not { } binary)
                {
                    throw new InvalidOperationException($"The extension \"{artifactReference}\" does not support architecture {architecture.Name}.");
                }

                this.BinaryFile.Write(binary.Data);
                this.BinaryFile.MakeExecutable();
            }
        });
    }
}