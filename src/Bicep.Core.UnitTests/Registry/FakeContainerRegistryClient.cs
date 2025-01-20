// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Legacy.Table;
using Moq;

namespace Bicep.Core.UnitTests.Registry
{
    /// <summary>
    /// Mock OCI registry blob client. This client is intended to represent a single repository within a specific registry Uri.
    /// </summary>
    public class FakeContainerRegistryClient : ContainerRegistryClient
    {
        public FakeContainerRegistryClient() : base()
        {
            // ensure we call the base parameterless constructor to prevent outgoing calls
        }

        public int CountCallsGetRepositoryNamesAsync { get; private set; }

        public SortedList<string, string> FakeRepositoryNames { get; set; } = new();

        //asdfg
        //// maps digest to blob bytes
        //public ConcurrentDictionary<string, BinaryData> Blobs { get; } = new();

        //// May be different than number of actual blobs because blobs with the same contents get stored only once
        //public int BlobUploads { get; private set; }

        //// maps digest to manifest bytes
        //public ConcurrentDictionary<string, BinaryData> Manifests { get; } = new();

        //// maps tag to manifest digest
        //public ConcurrentDictionary<string, string> ManifestTags { get; } = new();

        //public IDictionary<string, OciManifest> ManifestObjects =>
        //    Manifests.ToDictionary(kvp => kvp.Key, kvp => OciManifest.FromBinaryData(kvp.Value)!);

        //public IDictionary<string, OciManifest> ModuleManifestObjects =>
        //    ManifestObjects
        //    .Where(kvp => kvp.Value.ArtifactType == BicepMediaTypes.BicepModuleArtifactType)
        //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public override AsyncPageable<string> GetRepositoryNamesAsync(CancellationToken cancellationToken = default) //asdfg test with lots and lots
        {
            CountCallsGetRepositoryNamesAsync++;

            var page = Page<string>.FromValues(FakeRepositoryNames.Values.ToArray(), continuationToken: null, StrictMock.Of<Response>().Object);
            return AsyncPageable<string>.FromPages([page]);
        }

        public override ContainerRepository GetRepository(string repositoryName)
        {
            var repository = StrictMock.Of<ContainerRepository>();
            repository.Setup(x => x.GetAllManifestPropertiesAsync(It.IsAny<ArtifactManifestOrder>(), It.IsAny<CancellationToken>()))
                .Returns((ArtifactManifestOrder order, CancellationToken token) =>
                    {
                        ArtifactManifestProperties is not mockable
                        var properties = StrictMock.Of<ArtifactManifestProperties>();
                        properties.Setup(x => x.Tags).Returns(FakeRepositoryNames.Keys.ToImmutableArray());

                        return AsyncPageable<ArtifactManifestProperties>.FromPages(
                            new[] { Page<ArtifactManifestProperties>.FromValues(
                            new[] { properties.Object }, null, StrictMock.Of<Response>().Object) }
                        );
                    }
                );
            return repository.Object;
        }


        //asdfg
        //public override async Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default)
        //{
        //    await Task.Yield();

        //    if (!this.Blobs.TryGetValue(digest, out var bytes))
        //    {
        //        throw new RequestFailedException(404, "Mock blob does not exist.");
        //    }

        //    return CreateResult(ContainerRegistryModelFactory.DownloadRegistryBlobResult(digest, BinaryData.FromBytes(bytes.ToArray())));
        //}

        //public override async Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default)
        //{
        //    await Task.Yield();

        //    if (tagOrDigest is null)
        //    {
        //        throw new RequestFailedException($"Downloading a manifest requires '{nameof(tagOrDigest)}' to be specified.");
        //    }

        //    if (!this.ManifestTags.TryGetValue(tagOrDigest, out var digest))
        //    {
        //        // no matching tag, the tagOrDigest value may possibly be a digest
        //        digest = tagOrDigest;
        //    }

        //    if (!this.Manifests.TryGetValue(digest, out var data))
        //    {
        //        throw new RequestFailedException(404, "Mock manifest does not exist.");
        //    }

        //    return CreateResult(ContainerRegistryModelFactory.GetManifestResult(
        //        digest: digest,
        //        mediaType: ManifestMediaType.OciImageManifest.ToString(),
        //        manifest: data));
        //}

        //public override async Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream stream, CancellationToken cancellationToken = default)
        //{
        //    await Task.Yield();
        //    return UploadBlob(stream, cancellationToken);
        //}

        //public override Response<UploadRegistryBlobResult> UploadBlob(Stream stream, CancellationToken cancellationToken = default)
        //{
        //    var (data, digest) = ReadStream(stream);
        //    Blobs.TryAdd(digest, data);

        //    var result = CreateResult(ContainerRegistryModelFactory.UploadRegistryBlobResult(digest, data.ToArray().Length));
        //    ++BlobUploads;
        //    return result;
        //}

        //public override async Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        //{
        //    await Task.Yield();
        //    return SetManifest(manifest, tag, mediaType, cancellationToken);
        //}

        //public override Response<SetManifestResult> SetManifest(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        //{
        //    var (copy, digest) = ReadStream(manifest.ToStream());
        //    Manifests.TryAdd(digest, copy);

        //    if (tag is not null)
        //    {
        //        // map tag to the digest
        //        this.ManifestTags[tag] = digest;
        //    }

        //    return CreateResult(ContainerRegistryModelFactory.SetManifestResult(digest));
        //}

        //public static (BinaryData bytes, string digest) ReadStream(Stream stream)
        //{
        //    var data = BinaryData.FromStream(stream);
        //    string digest = Core.Registry.Oci.OciDescriptor.ComputeDigest(Core.Registry.Oci.OciDescriptor.AlgorithmIdentifierSha256, data);
        //    return (data, digest);
        //}

        //private static Response<T> CreateResult<T>(T value)
        //{
        //    var response = StrictMock.Of<Response>();

        //    var result = StrictMock.Of<Response<T>>();
        //    result.SetupGet(m => m.Value).Returns(value);
        //    result.Setup(m => m.GetRawResponse()).Returns(response.Object);

        //    return result.Object;
        //}
    }
}
