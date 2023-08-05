// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.Registry
{
    /// <summary>
    /// Mock OCI registry blob client. This client is intended to represent a single repository within a specific registry Uri.
    /// </summary>
    public class MockRegistryBlobClient : IOciRegistryContentClient
    {
        // maps digest to blob bytes
        public ConcurrentDictionary<string, TextBytes> Blobs { get; } = new();

        // maps digest to manifest bytes
        public ConcurrentDictionary<string, TextBytes> Manifests { get; } = new();

        // maps tag to manifest digest
        public ConcurrentDictionary<string, string> ManifestTags { get; } = new();

        public IDictionary<string, OciManifest> ManifestObjects =>
            Manifests.ToDictionary(kvp => kvp.Key, kvp => OciSerialization.Deserialize<OciManifest>(kvp.Value.ToStream()));

        public IDictionary<string, OciManifest> ModuleManifestObjects =>
            ManifestObjects
            .Where(kvp => kvp.Value.ArtifactType == BicepMediaTypes.BicepModuleArtifactType)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public IDictionary<string, OciManifest> SourceManifestObjects =>
            ManifestObjects
            .Where(kvp => kvp.Value.ArtifactType == BicepMediaTypes.BicepSourceArtifactType)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public async Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            if (!this.Blobs.TryGetValue(digest, out var bytes))
            {
                throw new RequestFailedException(404, "Mock blob does not exist.");
            }

            return CreateResult(ContainerRegistryModelFactory.DownloadRegistryBlobResult(digest, BinaryData.FromBytes(bytes.ToArray())));
        }

        public async Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            if (tagOrDigest is null)
            {
                throw new RequestFailedException($"Downloading a manifest requires '{nameof(tagOrDigest)}' to be specified.");
            }

            if (!this.ManifestTags.TryGetValue(tagOrDigest, out var digest))
            {
                // no matching tag, the tagOrDigest value may possibly be a digest
                digest = tagOrDigest;
            }

            if (!this.Manifests.TryGetValue(digest, out var bytes))
            {
                throw new RequestFailedException(404, "Mock manifest does not exist.");
            }

            return CreateResult(ContainerRegistryModelFactory.GetManifestResult(
                digest: digest,
                mediaType: ManifestMediaType.OciImageManifest.ToString(),
                manifest: BinaryData.FromBytes(bytes.ToArray())));
        }

        public async Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var (copy, digest) = ReadStream(stream);
            Blobs.TryAdd(digest, new TextBytes(copy));

            return CreateResult(ContainerRegistryModelFactory.UploadRegistryBlobResult(digest, copy.Length));
        }

        public async Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var (copy, digest) = ReadStream(manifest.ToStream());
            Manifests.TryAdd(digest, new TextBytes(copy));

            if (tag is not null)
            {
                // map tag to the digest
                this.ManifestTags[tag] = digest;
            }

            return CreateResult(ContainerRegistryModelFactory.SetManifestResult(digest));
        }

        public static (ImmutableArray<byte> bytes, string digest) ReadStream(Stream stream)
        {
            stream.Position = 0;
            string digest = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);

            stream.Position = 0;
            using var reader = new BinaryReader(stream, new UTF8Encoding(false), true);

            var builder = ImmutableArray.CreateBuilder<byte>();

            stream.Position = 0;
            var bytes = reader.ReadBytes((int)stream.Length).ToImmutableArray();

            return (bytes, digest);
        }

        private static Response<T> CreateResult<T>(T value)
        {
            var response = StrictMock.Of<Response>();

            var result = StrictMock.Of<Response<T>>();
            result.SetupGet(m => m.Value).Returns(value);
            result.Setup(m => m.GetRawResponse()).Returns(response.Object);

            return result.Object;
        }

        public async Task<IEnumerable<(string digest, string? artifactType)>> GetReferrersAsync(string mainManifestDigest)
        {
            Debug.Assert(mainManifestDigest != null);

            await Task.Delay(1);

            var mainManifest = await GetManifestAsync(mainManifestDigest);
            if (mainManifest.Value is null)
            {
                return Enumerable.Empty<(string digest, string? artifactType)>();
            }

            var referringManifests = ManifestObjects.Where(m => m.Value.Subject?.Digest == mainManifestDigest);
            return referringManifests.Select(m => (m.Key, m.Value.ArtifactType));
        }
    }
}
