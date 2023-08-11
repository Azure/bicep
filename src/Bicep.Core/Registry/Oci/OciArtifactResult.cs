// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Containers.ContainerRegistry;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Threading;
using Azure;
using System.IO;

namespace Bicep.Core.Registry.Oci
{
    public class OciArtifactResult
    {
        public OciArtifactResult(ContainerRegistryContentClient client, BinaryData manifest, string manifestDigest)
        {
            this.client = client;
            this.manifest = manifest;
            this.serializedManifest = OciManifest.FromBinaryData(manifest) ?? throw new InvalidOperationException("the manifest is not a valid OCI manifest");
            this.manifestDigest = manifestDigest;
        }

        private readonly ContainerRegistryContentClient client;
        private readonly BinaryData manifest;
        private readonly string manifestDigest;
        private readonly OciManifest serializedManifest;

        public Stream ToStream() => manifest.ToStream();

        public OciManifest Manifest => serializedManifest;

        public string ManifestDigest => manifestDigest;

        public async Task<BinaryData> PullLayerAsync(OciDescriptor layer, CancellationToken cancellationToken = default)
        {
            Response<DownloadRegistryBlobResult> blobResult;
            try
            {
                blobResult = await client.DownloadBlobContentAsync(layer.Digest, cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                throw new InvalidModuleException($"Module manifest refers to a non-existent blob with digest \"{layer.Digest}\".", exception);
            }

            ValidateBlobResponse(blobResult, layer);

            return blobResult.Value.Content;
        }

        private static void ValidateBlobResponse(Response<DownloadRegistryBlobResult> blobResponse, OciDescriptor descriptor)
        {
            using var stream = blobResponse.Value.Content.ToStream();

            if (descriptor.Size != stream.Length)
            {
                throw new InvalidModuleException($"Expected blob size of {descriptor.Size} bytes but received {stream.Length} bytes from the registry.");
            }

            stream.Position = 0;
            string digestFromContents = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);
            stream.Position = 0;

            if (!string.Equals(descriptor.Digest, digestFromContents, StringComparison.Ordinal))
            {
                throw new InvalidModuleException($"There is a mismatch in the layer digests. Received content digest = {digestFromContents}, Requested digest = {descriptor.Digest}");
            }
        }

    }
}