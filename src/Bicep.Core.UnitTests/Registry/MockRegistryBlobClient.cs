// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Containers.ContainerRegistry.Specialized;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.Registry
{
    /// <summary>
    /// Mock OCI registry blob client. This client is intended to represent a single repository within a specific registry Uri.
    /// </summary>
    public class MockRegistryBlobClient: ContainerRegistryBlobClient
    {
        public MockRegistryBlobClient() : base()
        {
            // ensure we call the base parameterless constructor to prevent outgoing calls
        }

        // maps digest to blob bytes
        public ConcurrentDictionary<string, ImmutableArray<byte>> Blobs { get; } = new();

        // maps digest to manifest bytes
        public ConcurrentDictionary<string, ImmutableArray<byte>> Manifests { get; } = new();

        // maps tag to manifest digest
        public ConcurrentDictionary<string, string> ManifestTags { get; } = new();

        public override async Task<Response<DownloadBlobResult>> DownloadBlobAsync(string digest, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            if(!this.Blobs.TryGetValue(digest, out var bytes))
            {
                throw new RequestFailedException(404, "Mock blob does not exist.");
            }

            return CreateResult(ContainerRegistryModelFactory.DownloadBlobResult(digest, WriteStream(bytes)));
        }

        public override async Task<Response<DownloadManifestResult>> DownloadManifestAsync(DownloadManifestOptions? options = default, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            if(options is null)
            {
                throw new RequestFailedException("Downloading a manifest requires 'options' to be specified.");
            }

            string? digest;
            switch(options.Digest, options.Tag)
            {
                case (not null, not null):
                    throw new RequestFailedException("Both digest and tag cannot be specified when downloading a manifest.");

                case (not null, null):
                    // digest ref
                    digest = options.Digest;
                    break;

                case (null, not null):
                    // tag ref
                    if (!this.ManifestTags.TryGetValue(options.Tag, out digest))
                    {
                        throw new RequestFailedException(404, "Mock manifest tag does not exist.");
                    }

                    break;

                default:
                    throw new RequestFailedException("Either a digest or tag must be specified when downloading a manifest.");
            }

            if(!this.Manifests.TryGetValue(digest, out var bytes))
            {
                throw new RequestFailedException(404, "Mock manifest does not exist.");
            }

            return CreateResult(ContainerRegistryModelFactory.DownloadManifestResult(digest: digest, manifestStream: WriteStream(bytes)));
        }

        public override async Task<Response<UploadBlobResult>> UploadBlobAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var (copy, digest) = ReadStream(stream);
            Blobs.TryAdd(digest, copy);

            return CreateResult(ContainerRegistryModelFactory.UploadBlobResult());
        }

        public override async Task<Response<UploadManifestResult>> UploadManifestAsync(Stream stream, UploadManifestOptions? options = default, CancellationToken cancellationToken = default)
        {
            options ??= new UploadManifestOptions();

            await Task.Yield();

            var (copy, digest) = ReadStream(stream);
            Manifests.TryAdd(digest, copy);

            if(options.Tag is not null)
            {
                // map tag to the digest
                this.ManifestTags[options.Tag] = digest;
            }

            return CreateResult(ContainerRegistryModelFactory.UploadManifestResult());
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

        public static Stream WriteStream(ImmutableArray<byte> bytes)
        {
            var stream = new MemoryStream(bytes.Length);
            var writer = new BinaryWriter(stream, new UTF8Encoding(false), true);

            writer.Write(bytes.AsSpan());
            stream.Position = 0;

            return stream;
        }

        private static Response<T> CreateResult<T>(T value)
        {
            var response = StrictMock.Of<Response>();
            
            var result = StrictMock.Of<Response<T>>();
            result.SetupGet(m => m.Value).Returns(value);
            result.Setup(m => m.GetRawResponse()).Returns(response.Object);

            return result.Object;
        }
    }
}
