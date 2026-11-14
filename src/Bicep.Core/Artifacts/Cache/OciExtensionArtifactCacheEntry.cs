// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.RegistryClients.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Cache
{
    public sealed class OciExtensionArtifactCacheEntry : OciArtifactCacheEntry
    {
        private readonly ExtensionArtifactCacheLayout layout;

        public OciExtensionArtifactCacheEntry(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(address, rootCacheDirectory)
        {
            this.layout = new(this.CacheDirectory);
        }

        public IFileHandle TypesTgzFile => this.layout.TypesTgzFile;

        public IFileHandle BinaryFile => this.layout.BinaryFile;

        public override IFileHandle EntryPoint => this.TypesTgzFile;

        /// <summary>
        /// Writes a pulled extension into the cache, with this machine's binary if it has binaries, unless another
        /// compilation already has.
        /// </summary>
        /// <param name="reference">The extension's reference, for error messages.</param>
        public Task WriteAsync(ExtensionArtifactPackage package, OciArtifactPayload payload, ArtifactReference reference) =>
            this.WriteAsync(payload, () => this.layout.Write(package, reference));
    }
}