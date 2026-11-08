// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph.Artifacts
{
    /// <summary>
    /// The cache entry of an OCI artifact: <c>br/&lt;registry&gt;/&lt;repository&gt;/&lt;tag or digest&gt;/</c> under the cache root.
    /// </summary>
    public abstract class BicepRegistryArtifact : ExternalArtifact
    {
        public BicepRegistryArtifact(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(ResolveCacheDirectory(address, rootCacheDirectory))
        {
        }

        public IFileHandle ManifestFile => this.GetFile("manifest");

        public IFileHandle MetadataFile => this.GetFile("metadata");

        // The manifest and the metadata are written first and last, so all three files exist only once the entry is complete.
        public override bool IsComplete => this.EntryPoint.Exists() && this.ManifestFile.Exists() && this.MetadataFile.Exists();

        /// <summary>
        /// Writes a pulled artifact into the cache, unless another compilation already has.
        /// </summary>
        /// <param name="artifactReference">The reference, for error messages.</param>
        public Task WriteAsync(OciArtifactResult result, string artifactReference) => this.WriteAsync(() =>
        {
            // Write the original manifest bytes rather than a serialized manifest object, so that the manifest hash matches.
            this.ManifestFile.Write(result.ManifestData);

            this.WriteLayers(result, artifactReference);

            var metadata = new ModuleMetadata(result.ManifestDigest);
            using var metadataStream = new MemoryStream();
            OciSerialization.Serialize(metadataStream, metadata);
            metadataStream.Position = 0;

            this.MetadataFile.Write(metadataStream);
        });

        // CONSIDER: Write all layers as separate binary files instead of the files we know about (main.json, source.tgz, ...),
        // so that every version can restore every layer, even optional layers added later. The manifest says what each layer is.
        // (https://github.com/Azure/bicep/issues/11900)
        protected abstract void WriteLayers(OciArtifactResult result, string artifactReference);

        private static IDirectoryHandle ResolveCacheDirectory(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
        {
            // rootCacheDirectory is already set to %userprofile%\.bicep\br or ~/.bicep/br by default depending on OS
            // we need to split each component of the reference into a sub directory to fit within the max file name length limit on linux and mac

            // TODO: Need to deal with IDNs (internationalized domain names)
            // domain names can only contain alphanumeric chars, _, -, and numbers (example.azurecr.io or example.azurecr.io:443)
            // IPV4 addresses only contain dots and numbers (127.0.0.1 or 127.0.0.1:5000)
            // IPV6 addresses are hex digits with colons (2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF or [2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF]:5000)
            // the only problematic character is the colon, which we will replace with $ which is not allowed in any of the possible registry values
            // we will also normalize casing for registries since they are case-insensitive
            var registry = address.Registry.Replace(':', '$').ToLowerInvariant();

            // modules can have mixed hierarchy depths so we will flatten a repository into a single directory name
            // however to do this we must get rid of slashes (not a valid file system char on windows and a directory separator on linux/mac)
            // the replacement char must one that is not valid in a repository string but is valid in common type systems
            var repository = address.Repository.Replace('/', '$');

            var tagOrDigest = (address.Tag, address.Digest) switch
            {
                // tags are case-sensitive with length up to 128
                (not null, null) => TagEncoder.Encode(address.Tag),
                // digests are strings like "sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
                // and are already guaranteed to be lowercase
                // the only problematic character is the : which we will replace with a #
                // however the encoding we choose must not be ambiguous with the tag encoding
                (null, not null) => address.Digest.Replace(':', '#'),
                _ => throw new InvalidOperationException("Either tag or digest must be set."),
            };

            return rootCacheDirectory.GetDirectory($"{OciArtifactReferenceFacts.Scheme}/{registry}/{repository}/{tagOrDigest}");
        }
    }
}
