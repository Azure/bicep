// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OciDescriptor = Bicep.Core.Registry.Oci.OciDescriptor;
using OciManifest = Bicep.Core.Registry.Oci.OciManifest;


namespace Bicep.Core.Registry
{
    public class AzureContainerRegistryManager
    {
        // media types are case-insensitive (they are lowercase by convention only)
        private const StringComparison DigestComparison = StringComparison.Ordinal;

        private readonly IContainerRegistryClientFactory clientFactory;

        // From the spec: "While the algorithm does allow one to implement a wide variety of algorithms, compliant implementations should use sha256."
        // (https://docs.docker.com/registry/spec/api/#content-digests)
        private static readonly string DigestAlgorithmIdentifier = OciDescriptor.AlgorithmIdentifierSha256;

        public AzureContainerRegistryManager(IContainerRegistryClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<OciArtifactResult> PullArtifactAsync(
            RootConfiguration configuration,
            IOciArtifactReference artifactReference)
        {
            async Task<OciArtifactResult> DownloadManifestInternalAsync(bool anonymousAccess)
            {
                var client = CreateBlobClient(configuration, artifactReference, anonymousAccess);
                return await DownloadManifestAndLayersAsync(artifactReference, client);
            }

            try
            {
                // Try authenticated client first.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {artifactReference.FullyQualifiedReference}.");
                return await DownloadManifestInternalAsync(anonymousAccess: false);
            }
            catch (RequestFailedException exception) when (exception.Status == 401 || exception.Status == 403)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {artifactReference.FullyQualifiedReference} failed, received code {exception.Status}. Fallback to anonymous pull.");
                return await DownloadManifestInternalAsync(anonymousAccess: true);
            }
            catch (CredentialUnavailableException)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {artifactReference.FullyQualifiedReference} failed due to missing login step. Fallback to anonymous pull.");
                return await DownloadManifestInternalAsync(anonymousAccess: true);
            }
        }

        public async Task PushArtifactAsync(
            RootConfiguration configuration,
            IOciArtifactReference artifactReference,
            string? mediaType,
            string? artifactType,
            OciDescriptor config,
            IEnumerable<OciDescriptor> layers,
            OciManifestAnnotationsBuilder annotations)
        {

            // push is not supported anonymously
            var blobClient = this.CreateBlobClient(configuration, artifactReference, anonymousAccess: false);

            _ = await blobClient.UploadBlobAsync(config.Data);

            var layerDescriptors = layers.ToImmutableArray();
            foreach (var layer in layerDescriptors)
            {
                layerDescriptors.Add(layer);
                _ = await blobClient.UploadBlobAsync(layer.Data);
            }

            /* Sample artifact manifest:
                {
                    "schemaVersion": 2,
                    "artifactType": "application/vnd.ms.bicep.module.artifact",
                    "config": {
                        "mediaType": "application/vnd.ms.bicep.module.config.v1+json",
                        "digest": "sha256:...",
                        "size": 2
                    },
                    "layers": [
                        {
                        "mediaType": "application/vnd.ms.bicep.module.layer.v1+json",
                        "digest": "sha256:...",
                        "size": 2774
                        }
                    ],
                    "annotations": {
                        "org.opencontainers.image.description": "module description"
                        "org.opencontainers.image.documentation": "https://www.contoso.com/moduledocumentation.html"
                    }
                }
             */

            var manifest = new OciManifest(2, mediaType, artifactType, config, layerDescriptors, annotations.Build());

            var manifestBinaryData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
            _ = await blobClient.SetManifestAsync(manifestBinaryData, artifactReference.Tag, mediaType: ManifestMediaType.OciImageManifest);
        }

        private static Uri GetRegistryUri(IOciArtifactReference artifactReference) => new($"https://{artifactReference.Registry}");

        private ContainerRegistryContentClient CreateBlobClient(
            RootConfiguration configuration,
            IOciArtifactReference artifactReference,
            bool anonymousAccess) => anonymousAccess
            ? this.clientFactory.CreateAnonymousBlobClient(configuration, GetRegistryUri(artifactReference), artifactReference.Repository)
            : this.clientFactory.CreateAuthenticatedBlobClient(configuration, GetRegistryUri(artifactReference), artifactReference.Repository);

        private static async Task<OciArtifactResult> DownloadManifestAndLayersAsync(IOciArtifactReference artifactReference, ContainerRegistryContentClient client)
        {
            Response<GetManifestResult> manifestResponse;
            try
            {
                // either Tag or Digest is null (enforced by reference parser)
                var tagOrDigest = artifactReference.Tag
                    ?? artifactReference.Digest
                    ?? throw new ArgumentNullException(nameof(artifactReference), $"The specified artifact reference has both {nameof(artifactReference.Tag)} and {nameof(artifactReference.Digest)} set to null.");

                manifestResponse = await client.GetManifestAsync(tagOrDigest);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                // manifest does not exist
                Trace.WriteLine($"Manifest for module {artifactReference.FullyQualifiedReference} could not be found in the registry.");
                throw new OciArtifactRegistryException("The artifact does not exist in the registry.", exception);
            }
            Debug.Assert(manifestResponse.Value.Manifest.ToArray().Length > 0);

            // the Value is disposable, but we are not calling it because we need to pass the stream outside of this scope (and it will GC correctly)
            using var stream = manifestResponse.Value.Manifest.ToStream();

            // BUG: The SDK internally consumed the stream for validation purposes and left position at the end
            stream.Position = 0;
            ValidateManifestResponse(manifestResponse);

            var deserializedManifest = OciManifest.FromBinaryData(manifestResponse.Value.Manifest) ?? throw new InvalidOperationException("the manifest is not a valid OCI manifest");
            var layerTasks = deserializedManifest.Layers.AsParallel().WithDegreeOfParallelism(5)
                .Select(async layer => new OciArtifactLayer(layer.Digest, layer.MediaType, await PullLayerAsync(client, layer)));
            var layers = await Task.WhenAll(layerTasks);

            var config = !deserializedManifest.Config.IsEmpty() ?
                new OciArtifactLayer(deserializedManifest.Config.Digest, deserializedManifest.Config.MediaType, await PullLayerAsync(client, deserializedManifest.Config)) :
                null;

            return deserializedManifest.ArtifactType switch
            {
                BicepMediaTypes.BicepModuleArtifactType or null => new OciModuleArtifactResult(manifestResponse.Value.Manifest, manifestResponse.Value.Digest, layers),
                BicepMediaTypes.BicepProviderArtifactType => new OciProviderArtifactResult(manifestResponse.Value.Manifest, manifestResponse.Value.Digest, layers, config),
                _ => throw new InvalidArtifactException($"artifacts of type: \'{deserializedManifest.ArtifactType}\' are not supported by this Bicep version. {OciModuleArtifactResult.NewerVersionMightBeRequired}")
            };
        }

        private static async Task<BinaryData> PullLayerAsync(ContainerRegistryContentClient client, OciDescriptor layer, CancellationToken cancellationToken = default)
        {
            Response<DownloadRegistryBlobResult> blobResult;
            try
            {
                blobResult = await client.DownloadBlobContentAsync(layer.Digest, cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                throw new InvalidArtifactException($"Module manifest refers to a non-existent blob with digest \"{layer.Digest}\".", exception);
            }

            ValidateBlobResponse(blobResult, layer);

            return blobResult.Value.Content;
        }

        private static void ValidateBlobResponse(Response<DownloadRegistryBlobResult> blobResponse, OciDescriptor descriptor)
        {
            var data = blobResponse.Value.Content;
            var dataSize = data.ToArray().Length;

            if (descriptor.Size != dataSize)
            {
                throw new InvalidArtifactException($"Expected blob size of {descriptor.Size} bytes but received {dataSize} bytes from the registry.");
            }
            string digestFromContents = OciDescriptor.ComputeDigest(DigestAlgorithmIdentifier, data);

            if (!string.Equals(descriptor.Digest, digestFromContents, StringComparison.Ordinal))
            {
                throw new InvalidArtifactException($"There is a mismatch in the layer digests. Received content digest = {digestFromContents}, Requested digest = {descriptor.Digest}");
            }
        }

        private static void ValidateManifestResponse(Response<GetManifestResult> manifestResponse)
        {
            var digestFromRegistry = manifestResponse.Value.Digest;
            string digestFromContent = OciDescriptor.ComputeDigest(DigestAlgorithmIdentifier, manifestResponse.Value.Manifest);

            if (!string.Equals(digestFromRegistry, digestFromContent, DigestComparison))
            {
                throw new OciArtifactRegistryException($"There is a mismatch in the manifest digests. Received content digest = {digestFromContent}, Digest in registry response = {digestFromRegistry}");
            }
        }
    }
}
