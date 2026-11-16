// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.RegistryClients.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Cache
{
    /// <summary>
    /// The cache entry of a local extension package, extracted under <c>local/sha256_&lt;digest of the package&gt;/</c>.
    /// </summary>
    public sealed class LocalExtensionArtifactCacheEntry : ArtifactCacheEntry
    {
        private readonly ExtensionArtifactCacheLayout layout;

        /// <param name="packageData">The encoded package, whose digest names the entry.</param>
        public LocalExtensionArtifactCacheEntry(BinaryData packageData, IDirectoryHandle rootCacheDirectory)
            : base(ResolveCacheDirectory(packageData, rootCacheDirectory))
        {
            this.layout = new(this.CacheDirectory);
        }

        public IFileHandle TypesTgzFile => this.layout.TypesTgzFile;

        public IFileHandle BinaryFile => this.layout.BinaryFile;

        public override IFileHandle EntryPoint => this.TypesTgzFile;

        public override bool IsComplete => this.TypesTgzFile.Exists();

        /// <summary>
        /// Writes a decoded extension package into the cache, unless another compilation (possibly in another process) already has.
        /// </summary>
        /// <param name="reference">The extension's reference, for error messages.</param>
        /// <exception cref="ArtifactException">The cache directory could not be created or locked.</exception>
        /// <exception cref="InvalidOperationException">The extension does not support the current OS or architecture.</exception>
        public void Write(ExtensionArtifactPackage package, ArtifactReference reference) => this.Write(() => this.layout.Write(package, reference));

        private static IDirectoryHandle ResolveCacheDirectory(BinaryData packageData, IDirectoryHandle rootCacheDirectory)
        {
            // The cache is content-addressed, so that a package extracted once is reused by every reference to the same content.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, packageData, separator: '_');

            return rootCacheDirectory.GetDirectory($"local/{digest}");
        }
    }
}
