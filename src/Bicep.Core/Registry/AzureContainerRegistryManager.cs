// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using OciDescriptor = Bicep.Core.Registry.Oci.OciDescriptor;
using OciManifest = Bicep.Core.Registry.Oci.OciManifest;

namespace Bicep.Core.Registry
{
    public class AzureContainerRegistryManager
    {
        // media types are case-insensitive (they are lowercase by convention only)
        // private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;
        public static readonly IEqualityComparer<string> MediaTypeComparer = StringComparer.OrdinalIgnoreCase;
        private const StringComparison DigestComparison = StringComparison.Ordinal;

        private readonly IContainerRegistryClientFactory clientFactory;

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
                return await DownloadManifestAsync(artifactReference, client);
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
            string? artifactType,
            StreamDescriptor config,
            string? documentationUri = null,
            string? description = null,
            params StreamDescriptor[] layers)
        {
            // TODO: How do we choose this? Does it ever change?
            var algorithmIdentifier = DescriptorFactory.AlgorithmIdentifierSha256;

            // push is not supported anonymously
            var blobClient = this.CreateBlobClient(configuration, artifactReference, anonymousAccess: false);

            config.ResetStream();
            var configDescriptor = DescriptorFactory.CreateDescriptor(algorithmIdentifier, config);

            config.ResetStream();
            _ = await blobClient.UploadBlobAsync(config.Stream);

            var layerDescriptors = new List<OciDescriptor>(layers.Length);
            foreach (var layer in layers)
            {
                layer.ResetStream();
                var layerDescriptor = DescriptorFactory.CreateDescriptor(algorithmIdentifier, layer);
                layerDescriptors.Add(layerDescriptor);

                layer.ResetStream();
                _ = await blobClient.UploadBlobAsync(layer.Stream);
            }

            var annotations = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(documentationUri))
            {
                annotations[LanguageConstants.OciOpenContainerImageDocumentationAnnotation] = documentationUri;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                annotations[LanguageConstants.OciOpenContainerImageDescriptionAnnotation] = description;
            }

            var manifest = new OciManifest(2, artifactType, configDescriptor, layerDescriptors, annotations);

            using var manifestStream = new MemoryStream();
            OciSerialization.Serialize(manifestStream, manifest);

            manifestStream.Position = 0;
            var manifestBinaryData = await BinaryData.FromStreamAsync(manifestStream);
            var manifestUploadResult = await blobClient.SetManifestAsync(manifestBinaryData, artifactReference.Tag, mediaType: ManifestMediaType.OciImageManifest);
        }

        private static Uri GetRegistryUri(IOciArtifactReference artifactReference) => new($"https://{artifactReference.Registry}");

        private ContainerRegistryContentClient CreateBlobClient(
            RootConfiguration configuration,
            IOciArtifactReference artifactReference,
            bool anonymousAccess) => anonymousAccess
            ? this.clientFactory.CreateAnonymousBlobClient(configuration, GetRegistryUri(artifactReference), artifactReference.Repository)
            : this.clientFactory.CreateAuthenticatedBlobClient(configuration, GetRegistryUri(artifactReference), artifactReference.Repository);

        private static async Task<OciArtifactResult> DownloadManifestAsync(IOciArtifactReference artifactReference, ContainerRegistryContentClient client)
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
                throw new OciModuleRegistryException("The artifact does not exist in the registry.", exception);
            }

            // the Value is disposable, but we are not calling it because we need to pass the stream outside of this scope
            using var stream = manifestResponse.Value.Manifest.ToStream();

            // BUG: The SDK internally consumed the stream for validation purposes and left position at the end
            stream.Position = 0;
            ValidateManifestResponse(manifestResponse);
            return new OciArtifactResult(client, manifestResponse.Value.Manifest, manifestResponse.Value.Digest);
        }

        private static void ValidateManifestResponse(Response<GetManifestResult> manifestResponse)
        {
            var digestFromRegistry = manifestResponse.Value.Digest;
            var stream = manifestResponse.Value.Manifest.ToStream();

            // TODO: The registry may use a different digest algorithm - we need to handle that
            string digestFromContent = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);

            if (!string.Equals(digestFromRegistry, digestFromContent, DigestComparison))
            {
                throw new OciModuleRegistryException($"There is a mismatch in the manifest digests. Received content digest = {digestFromContent}, Digest in registry response = {digestFromRegistry}");
            }
        }

        // private static async Task<Stream> ProcessManifest(ContainerRegistryContentClient client, OciManifest manifest)
        // {
        //     // Bicep versions before 0.14 used to publish modules without the artifactType field set in the OCI manifest,
        //     // so we must allow null here
        //     if (manifest.ArtifactType is not null && !string.Equals(manifest.ArtifactType, BicepMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
        //     {
        //         throw new InvalidModuleException($"Expected OCI artifact to have the artifactType field set to either null or '{BicepMediaTypes.BicepModuleArtifactType}' but found '{manifest.ArtifactType}'.", InvalidModuleExceptionKind.WrongArtifactType);
        //     }

        //     ProcessConfig(manifest.Config);
        //     if (manifest.Layers.Length != 1)
        //     {
        //         throw new InvalidModuleException("Expected a single layer in the OCI artifact.");
        //     }

        //     var layer = manifest.Layers.Single();

        //     return await ProcessLayer(client, layer);
        // }

        // private static void ValidateBlobResponse(Response<DownloadRegistryBlobResult> blobResponse, OciDescriptor descriptor)
        // {
        //     var stream = blobResponse.Value.Content.ToStream();

        //     if (descriptor.Size != stream.Length)
        //     {
        //         throw new InvalidModuleException($"Expected blob size of {descriptor.Size} bytes but received {stream.Length} bytes from the registry.");
        //     }

        //     stream.Position = 0;
        //     string digestFromContents = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);
        //     stream.Position = 0;

        //     if (!string.Equals(descriptor.Digest, digestFromContents, DigestComparison))
        //     {
        //         throw new InvalidModuleException($"There is a mismatch in the layer digests. Received content digest = {digestFromContents}, Requested digest = {descriptor.Digest}");
        //     }
        // }

        // private static readonly ImmutableArray<string> allowedMediaTypes = new() { BicepMediaTypes.BicepModuleLayerV1Json, BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip };
        
        // private static async Task<Stream> ProcessLayer(ContainerRegistryContentClient client, OciDescriptor layer)
        // {
        //     // media types are case insensitive
        //     if (!allowedMediaTypes.Contains(layer.MediaType, MediaTypeComparer))
        //     {
        //         new InvalidModuleException($"Did not expect layer media type \"{layer.MediaType}\".", InvalidModuleExceptionKind.WrongModuleLayerMediaType);
        //     }

        //     Response<DownloadRegistryBlobResult> blobResult;
        //     try
        //     {
        //         blobResult = await client.DownloadBlobContentAsync(layer.Digest);
        //     }
        //     catch (RequestFailedException exception) when (exception.Status == 404)
        //     {
        //         throw new InvalidModuleException($"Module manifest refers to a non-existent blob with digest \"{layer.Digest}\".", exception);
        //     }

        //     ValidateBlobResponse(blobResult, layer);

        //     return blobResult.Value.Content.ToStream();
        // }

        // private static void ProcessConfig(OciDescriptor config)
        // {
        //     string[] allowedMediaTypes = { BicepMediaTypes.BicepModuleConfigV1, BicepMediaTypes.BicepProviderConfigV1 };
        //     // media types are case insensitive
        //     if (!allowedMediaTypes.Contains(config.MediaType, StringComparer.OrdinalIgnoreCase))
        //     {
        //         throw new InvalidModuleException($"Did not expect config media type \"{config.MediaType}\".");
        //     }

        //     if (config.Size > 2)
        //     {
        //         throw new InvalidModuleException("Expected an empty config blob.");
        //     }
        // }
    }
}
