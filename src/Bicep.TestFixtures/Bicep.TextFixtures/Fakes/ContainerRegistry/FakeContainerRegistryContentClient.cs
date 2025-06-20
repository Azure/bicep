// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Azure;
using Azure.Containers.ContainerRegistry;
using static Azure.Containers.ContainerRegistry.ContainerRegistryModelFactory;

namespace Bicep.TextFixtures.Fakes.ContainerRegistry
{
    public class FakeContainerRegistryContentClient : ContainerRegistryContentClient
    {
        private readonly FakeContainerRepository repository;

        public FakeContainerRegistryContentClient(FakeContainerRepository repository)
            : base()
        {
            // ensure we call the base parameterless constructor to prevent outgoing calls
            this.repository = repository;
        }

        public override Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default)
        {
            if (this.repository.TryGetBlob(digest) is not { } blob)
            {
                throw new RequestFailedException(404, $"Mock blob with digest {digest} does not exist.");
            }

            return CreateResponseTask(DownloadRegistryBlobResult(digest, blob.Data));
        }

        public override Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default)
        {
            if (this.repository.TryGetManifest(tagOrDigest) is not { } manifest)
            {
                throw new RequestFailedException(404, $"Mock manifest with tag or digest {tagOrDigest} does not exist.");
            }

            return CreateResponseTask(GetManifestResult(manifest.Digest, ManifestMediaType.OciImageManifest.ToString(), manifest.Data));
        }

        public override Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream blobStream, CancellationToken cancellationToken = default) => Task.FromResult(UploadBlob(blobStream, cancellationToken));

        public override Response<UploadRegistryBlobResult> UploadBlob(Stream blobStream, CancellationToken cancellationToken = default)
        {
            var data = BinaryData.FromStream(blobStream);
            var blob = this.repository.SetBlob(data);

            return CreateResponse(UploadRegistryBlobResult(blob.Digest, data.ToArray().Length));
        }

        public override Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifestData, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default) =>
            Task.FromResult(SetManifest(manifestData, tag, mediaType, cancellationToken));

        public override Response<SetManifestResult> SetManifest(BinaryData manifestData, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        {
            var manifest = this.repository.SetManifest(manifestData, tag);

            return CreateResponse(SetManifestResult(manifest.Digest));
        }

        private static Response<T> CreateResponse<T>(T value) => Response.FromValue(value, DummyResponse.Instance);

        private static Task<Response<T>> CreateResponseTask<T>(T value) => Task.FromResult(CreateResponse(value));
    }
}
