// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry.Specialized;
using Azure.Identity;
using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using OciManifest = Bicep.Core.Registry.Oci.OciManifest;

namespace Bicep.Core.Registry
{
    public class AzureContainerRegistryManager
    {
        // media types are case-insensitive (they are lowercase by convention only)
        private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;
        private const StringComparison DigestComparison = StringComparison.Ordinal;

        private readonly IContainerRegistryClientFactory clientFactory;

        public AzureContainerRegistryManager(IContainerRegistryClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<OciArtifactResult> PullArtifactAsync(RootConfiguration configuration, OciArtifactModuleReference moduleReference)
        {
            ContainerRegistryBlobClient client;
            OciManifest manifest;
            Stream manifestStream;
            string manifestDigest;

            async Task<(ContainerRegistryBlobClient, OciManifest, Stream, string)> DownloadManifestInternalAsync(bool anonymousAccess)
            {
                var client = this.CreateBlobClient(configuration, moduleReference, anonymousAccess);
                var (manifest, manifestStream, manifestDigest) = await DownloadManifestAsync(moduleReference, client);
                return (client, manifest, manifestStream, manifestDigest);
            }

            try
            {
                // Try authenticated client first.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {moduleReference.FullyQualifiedReference}.");
                (client, manifest, manifestStream, manifestDigest) = await DownloadManifestInternalAsync(anonymousAccess: false);
            }
            catch (RequestFailedException exception) when (exception.Status == 401 || exception.Status == 403)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {moduleReference.FullyQualifiedReference} failed, received code {exception.Status}. Fallback to anonymous pull.");                
                (client, manifest, manifestStream, manifestDigest) = await DownloadManifestInternalAsync(anonymousAccess: true);
            }
            catch(CredentialUnavailableException)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {moduleReference.FullyQualifiedReference} failed due to missing login step. Fallback to anonymous pull.");
                (client, manifest, manifestStream, manifestDigest) = await DownloadManifestInternalAsync(anonymousAccess: true);
            }

            var moduleStream = await ProcessManifest(client, manifest);

            return new OciArtifactResult(manifestDigest, manifest, manifestStream, moduleStream);
        }

        public async Task PushArtifactAsync(RootConfiguration configuration, OciArtifactModuleReference moduleReference, string? artifactType, StreamDescriptor config, string? documentationUri = null, params StreamDescriptor[] layers)
        {
            // TODO: How do we choose this? Does it ever change?
            var algorithmIdentifier = DescriptorFactory.AlgorithmIdentifierSha256;

            // push is not supported anonymously
            var blobClient = this.CreateBlobClient(configuration, moduleReference, anonymousAccess: false);

            config.ResetStream();
            var configDescriptor = DescriptorFactory.CreateDescriptor(algorithmIdentifier, config);

            config.ResetStream();
            var configUploadResult = await blobClient.UploadBlobAsync(config.Stream);

            var layerDescriptors = new List<OciDescriptor>(layers.Length);
            foreach (var layer in layers)
            {
                layer.ResetStream();
                var layerDescriptor = DescriptorFactory.CreateDescriptor(algorithmIdentifier, layer);
                layerDescriptors.Add(layerDescriptor);

                layer.ResetStream();
                var layerUploadResult = await blobClient.UploadBlobAsync(layer.Stream);
            }

            OciManifest manifest;

            if (string.IsNullOrWhiteSpace(documentationUri))
            {
                manifest = new OciManifest(2, artifactType, configDescriptor, layerDescriptors);
            }
            else
            {
                var annotations = new Dictionary<string, string>
            {
                { LanguageConstants.OciOpenContainerImageDocumentationAnnotation, documentationUri }
            };
                manifest = new OciManifest(2, artifactType, configDescriptor, layerDescriptors, annotations);
            }

            using var manifestStream = new MemoryStream();
            OciSerialization.Serialize(manifestStream, manifest);

            manifestStream.Position = 0;
            // BUG: the client closes the stream :( (is it still the case?)
            var manifestUploadResult = await blobClient.UploadManifestAsync(manifestStream, new UploadManifestOptions(tag: moduleReference.Tag));
        }

        private static Uri GetRegistryUri(OciArtifactModuleReference moduleReference) => new($"https://{moduleReference.Registry}");

        private ContainerRegistryBlobClient CreateBlobClient(RootConfiguration configuration, OciArtifactModuleReference moduleReference, bool anonymousAccess) => anonymousAccess
            ? this.clientFactory.CreateAnonymouosBlobClient(configuration, GetRegistryUri(moduleReference), moduleReference.Repository)
            : this.clientFactory.CreateAuthenticatedBlobClient(configuration, GetRegistryUri(moduleReference), moduleReference.Repository);

        private static async Task<(OciManifest, Stream, string)> DownloadManifestAsync(OciArtifactModuleReference moduleReference, ContainerRegistryBlobClient client)
        {
            Response<DownloadManifestResult> manifestResponse;
            try
            {
                // either Tag or Digest is null (enforced by reference parser) and DownloadManifestOptions throws if both or neither are null
                manifestResponse = await client.DownloadManifestAsync(new DownloadManifestOptions(tag: moduleReference.Tag, digest: moduleReference.Digest));
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                // manifest does not exist
                throw new OciModuleRegistryException("The module does not exist in the registry.", exception);
            }

            // the Value is disposable, but we are not calling it because we need to pass the stream outside of this scope
            var stream = manifestResponse.Value.ManifestStream;

            // BUG: The SDK internally consumed the stream for validation purposes and left position at the end
            stream.Position = 0;
            ValidateManifestResponse(manifestResponse);

            // the SDK doesn't expose all the manifest properties we need
            // so we need to deserialize the manifest ourselves to get everything
            stream.Position = 0;
            var deserialized = DeserializeManifest(stream);
            stream.Position = 0;

            return (deserialized, stream, manifestResponse.Value.Digest);
        }

        private static void ValidateManifestResponse(Response<DownloadManifestResult> manifestResponse)
        {
            var digestFromRegistry = manifestResponse.Value.Digest;
            var stream = manifestResponse.Value.ManifestStream;

            // TODO: The registry may use a different digest algorithm - we need to handle that
            string digestFromContent = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);

            if (!string.Equals(digestFromRegistry, digestFromContent, DigestComparison))
            {
                throw new OciModuleRegistryException($"There is a mismatch in the manifest digests. Received content digest = {digestFromContent}, Digest in registry response = {digestFromRegistry}");
            }
        }

        private static async Task<Stream> ProcessManifest(ContainerRegistryBlobClient client, OciManifest manifest)
        {
            // Bicep versions before 0.14 used to publish modules without the artifactType field set in the OCI manifest,
            // so we must allow null here
            if(manifest.ArtifactType is not null && !string.Equals(manifest.ArtifactType, BicepMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Expected OCI artifact to have the artifactType field set to either null or '{BicepMediaTypes.BicepModuleArtifactType}' but found '{manifest.ArtifactType}'.");
            }

            ProcessConfig(manifest.Config);
            if (manifest.Layers.Length != 1)
            {
                throw new InvalidModuleException("Expected a single layer in the OCI artifact.");
            }

            var layer = manifest.Layers.Single();

            return await ProcessLayer(client, layer);
        }

        private static void ValidateBlobResponse(Response<DownloadBlobResult> blobResponse, OciDescriptor descriptor)
        {
            var stream = blobResponse.Value.Content;

            if (descriptor.Size != stream.Length)
            {
                throw new InvalidModuleException($"Expected blob size of {descriptor.Size} bytes but received {stream.Length} bytes from the registry.");
            }

            stream.Position = 0;
            string digestFromContents = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);
            stream.Position = 0;

            if (!string.Equals(descriptor.Digest, digestFromContents, DigestComparison))
            {
                throw new InvalidModuleException($"There is a mismatch in the layer digests. Received content digest = {digestFromContents}, Requested digest = {descriptor.Digest}");
            }
        }

        private static async Task<Stream> ProcessLayer(ContainerRegistryBlobClient client, OciDescriptor layer)
        {
            if (!string.Equals(layer.MediaType, BicepMediaTypes.BicepModuleLayerV1Json, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Did not expect layer media type \"{layer.MediaType}\".");
            }

            Response<DownloadBlobResult> blobResult;
            try
            {
                blobResult = await client.DownloadBlobAsync(layer.Digest);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                throw new InvalidModuleException($"Module manifest refers to a non-existent blob with digest \"{layer.Digest}\".", exception);
            }

            ValidateBlobResponse(blobResult, layer);

            return blobResult.Value.Content;
        }

        private static void ProcessConfig(OciDescriptor config)
        {
            // media types are case insensitive
            if (!string.Equals(config.MediaType, BicepMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Did not expect config media type \"{config.MediaType}\".");
            }

            if (config.Size != 0)
            {
                throw new InvalidModuleException("Expected an empty config blob.");
            }
        }

        private static OciManifest DeserializeManifest(Stream stream)
        {
            try
            {
                return OciSerialization.Deserialize<OciManifest>(stream);
            }
            catch (Exception exception)
            {
                throw new InvalidModuleException("Unable to deserialize the module manifest.", exception);
            }
        }

        private class InvalidModuleException : OciModuleRegistryException
        {
            public InvalidModuleException(string innerMessage) : base($"The OCI artifact is not a valid Bicep module. {innerMessage}")
            {
            }

            public InvalidModuleException(string innerMessage, Exception innerException)
                : base($"The OCI artifact is not a valid Bicep module. {innerMessage}", innerException)
            {
            }
        }
    }
}
