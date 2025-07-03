// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;

namespace Bicep.TextFixtures.Fakes.ContainerRegistry
{
    public class FakeContainerRepository : ContainerRepository
    {
        public record BlobEntry(string Digest, BinaryData Data);
        public record ManifestEntry(string Digest, BinaryData Data);

        private readonly ConcurrentDictionary<string, BlobEntry> blobsByDigest = [];
        private readonly ConcurrentDictionary<string, ManifestEntry> manifestsByDigest = [];
        private readonly ConcurrentDictionary<string, string> manifestDigestsByTag = [];

        public FakeContainerRepository(FakeContainerRegistry registry, string name)
            : base()
        {
            this.Registry = registry;
            this.Name = name;
        }

        public FakeContainerRegistry Registry { get; }

        public override string Name { get; }

        public override AsyncPageable<ArtifactManifestProperties> GetAllManifestPropertiesAsync(ArtifactManifestOrder manifestOrder = ArtifactManifestOrder.None, CancellationToken cancellationToken = default)
        {
            // We only care about the tags, so no need to add support for other properties like sizeInBytes, createdOn, etc.
            var manifestProperties = manifestDigestsByTag
                .GroupBy(x => x.Value, x => x.Key)
                .Select(x => ContainerRegistryModelFactory.ArtifactManifestProperties(this.Registry.LoginServer, this.Name, digest: x.Key, tags: x));

            var page = Page<ArtifactManifestProperties>.FromValues(manifestProperties.ToArray(), continuationToken: null, DummyResponse.Instance);

            return AsyncPageable<ArtifactManifestProperties>.FromPages([page]);
        }

        public BlobEntry? TryGetBlob(string digest) => this.blobsByDigest.TryGetValue(digest, out var blob) ? blob : null;

        public BlobEntry SetBlob(BinaryData data)
        {
            var digest = ComputeDigest(data);
            var blob = new BlobEntry(digest, data);

            this.blobsByDigest[digest] = blob;

            return blob;
        }

        public ManifestEntry? TryGetManifest(string tagOrDigest)
        {
            if (!this.manifestDigestsByTag.TryGetValue(tagOrDigest, out var digest))
            {
                // No matching tag, the tagOrDigest value may possibly be a digest.
                digest = tagOrDigest;
            }

            return this.manifestsByDigest.TryGetValue(digest, out var manifest) ? manifest : null;
        }

        public ManifestEntry SetManifest(BinaryData data, string? tag = null)
        {
            var digest = ComputeDigest(data);
            var manifestEntry = new ManifestEntry(digest, data);

            this.manifestsByDigest[digest] = manifestEntry;

            if (tag is not null)
            {
                this.manifestDigestsByTag[tag] = digest;
            }

            return manifestEntry;
        }

        private static string ComputeDigest(BinaryData data) => Core.Registry.Oci.OciDescriptor.ComputeDigest(Core.Registry.Oci.OciDescriptor.AlgorithmIdentifierSha256, data);
    }
}
