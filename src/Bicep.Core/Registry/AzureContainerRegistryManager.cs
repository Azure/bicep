// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OciDescriptor = Bicep.Core.Registry.Oci.OciDescriptor;
using OciManifest = Bicep.Core.Registry.Oci.OciManifest;

namespace Bicep.Core.Registry
{
    public class AzureContainerRegistryManager
    {
        // media types are case-insensitive (they are lowercase by convention only)
        private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;
        private const StringComparison DigestComparison = StringComparison.Ordinal;

        private readonly IContainerRegistryClientFactory clientFactory;

        // https://docs.docker.com/registry/spec/api/#content-digests:
        //   "While the algorithm does allow one to implement a wide variety of algorithms, compliant implementations should use sha256."
        private readonly string DigestAlgorithmIdentifier = DescriptorFactory.AlgorithmIdentifierSha256;

        public AzureContainerRegistryManager(IContainerRegistryClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<OciArtifactResult> PullModuleAsync(RootConfiguration configuration, OciArtifactModuleReference moduleReference)
        {
            IOciRegistryContentClient client;
            OciManifest manifest;
            Stream manifestStream;
            string moduleManifestDigest;

            async Task<(IOciRegistryContentClient, OciManifest, Stream, string)> DownloadModuleManifestInternalAsync(bool anonymousAccess)
            {
                var client = this.CreateBlobClient(configuration, moduleReference, anonymousAccess);
                var (manifest, manifestStream, manifestDigest) = await DownloadModuleManifestAsync(moduleReference, client);
                return (client, manifest, manifestStream, manifestDigest);
            }

            try
            {
                // Try authenticated client first.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {moduleReference.FullyQualifiedReference}.");
                (client, manifest, manifestStream, moduleManifestDigest) = await DownloadModuleManifestInternalAsync(anonymousAccess: false);
            }
            catch (RequestFailedException exception) when (exception.Status == 401 || exception.Status == 403)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {moduleReference.FullyQualifiedReference} failed, received code {exception.Status}. Fallback to anonymous pull.");
                (client, manifest, manifestStream, moduleManifestDigest) = await DownloadModuleManifestInternalAsync(anonymousAccess: true);
            }
            catch (CredentialUnavailableException)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull artifact for module {moduleReference.FullyQualifiedReference} failed due to missing login step. Fallback to anonymous pull.");
                (client, manifest, manifestStream, moduleManifestDigest) = await DownloadModuleManifestInternalAsync(anonymousAccess: true);
            }

            // Continue using the working client for the rest of our calls

            var moduleStream = await GetArmTemplateFromModuleManifestAsync(client, manifest);

            return new OciArtifactResult(moduleManifestDigest, manifest, manifestStream, moduleStream);
        }

        public async Task PushModuleAsync(RootConfiguration configuration, OciArtifactModuleReference moduleReference, string? artifactType, StreamDescriptor config, string? documentationUri, string? description, StreamDescriptor[] layers)
        {
            // push is not supported anonymously
            var blobClient = this.CreateBlobClient(configuration, moduleReference, anonymousAccess: false);

            await PushModuleManifestAsync(blobClient, moduleReference, artifactType, config, documentationUri, description, layers);
        }

        private async Task<OciDescriptor> PushModuleManifestAsync(IOciRegistryContentClient blobClient, OciArtifactModuleReference moduleReference, string? artifactType, StreamDescriptor config, string? documentationUri, string? description, StreamDescriptor[] layers)
        {
            /* Sample module manifest:
                {
                    "schemaVersion": 2,
                    "artifactType": "application/vnd.ms.bicep.module.artifact",
                    "config": {
                      "mediaType": "application/vnd.ms.bicep.module.config.v1+json",
                      "digest": "sha256:...",
                      "size": 0
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

            var annotations = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(documentationUri))
            {
                annotations[LanguageConstants.OciOpenContainerImageDocumentationAnnotation] = documentationUri;
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                annotations[LanguageConstants.OciOpenContainerImageDescriptionAnnotation] = description;
            }

            var moduleManifestDescriptor = await PushArtifactAsync(
                blobClient,
                moduleReference.Tag,
                subject: null,
                artifactType,
                config,
                annotations,
                layers);
            Trace.WriteLine($"Uploaded module {moduleReference.FullyQualifiedReference} with digest {moduleManifestDescriptor.Digest}");

            return moduleManifestDescriptor;
        }

        private async Task<OciDescriptor> PushArtifactAsync(
            IOciRegistryContentClient blobClient,
            string? tag,
             // Subject specifies a manifest that this artifact "refers" to, thus "attaching" this artifact to the subject manifest. This manifest will remain alive as long as the referred manifest exists.
            OciDescriptor? subject,
            string? artifactType,
            StreamDescriptor config,
            IDictionary<string, string> annotations,
            StreamDescriptor[] layers
        )
        {
            // Upload the configuration as a blob
            config.ResetStream();
            var configDescriptor = DescriptorFactory.CreateDescriptor(DigestAlgorithmIdentifier, config);

            config.ResetStream();
            var configBlobUploadResult = await blobClient.UploadBlobAsync(config.Stream);
            if (configBlobUploadResult.Value.Digest != configDescriptor.Digest)
            {
                throw new OciModuleRegistryException($"Uploaded blob received an unexpected digest value.  Expected digest = {configDescriptor.Digest}, digest in registry response = {configBlobUploadResult.Value.Digest}");
            }

            // Upload each layer's contents as blobs
            var layerDescriptors = new List<OciDescriptor>(layers.Length);
            foreach (var layer in layers)
            {
                var layerDescriptor = DescriptorFactory.CreateDescriptor(DigestAlgorithmIdentifier, layer);
                layerDescriptors.Add(layerDescriptor);

                layer.ResetStream();
                var layerBlobUploadResult = await blobClient.UploadBlobAsync(layer.Stream);
                if (layerBlobUploadResult.Value.Digest != layerDescriptor.Digest)
                {
                    throw new OciModuleRegistryException($"Uploaded blob received an unexpected digest value.  Expected digest = {layerDescriptor.Digest}, digest in registry response = {layerBlobUploadResult.Value.Digest}");
                }
            }

            // Create and upload the manifest
            var manifest = new OciManifest(
                schemaVersion: 2,
                mediaType: null,
                artifactType,
                config: configDescriptor,
                layers: layerDescriptors,
                subject: subject,
                annotations);

            using var manifestStream = new MemoryStream();
            OciSerialization.Serialize(manifestStream, manifest);

            manifestStream.Position = 0;
            var manifestBinaryData = await BinaryData.FromStreamAsync(manifestStream);
            var manifestUploadResult = await blobClient.SetManifestAsync(manifestBinaryData, tag, mediaType: ManifestMediaType.OciImageManifest);

            // Create a descriptor to refer to this new manifest
            manifestStream.Position = 0;
            var manifestStreamDescriptor = new StreamDescriptor(manifestStream, ManifestMediaType.OciImageManifest.ToString());
            var manifestDescriptor = DescriptorFactory.CreateDescriptor(DigestAlgorithmIdentifier, manifestStreamDescriptor);
            Debug.Assert(manifestDescriptor.Digest == manifestUploadResult.Value.Digest, "The calculated digest of the manifest descriptor should match the uploaded digest.");

            return manifestDescriptor;
        }

        private static Uri GetRegistryUri(OciArtifactModuleReference moduleReference) => new($"https://{moduleReference.Registry}");

        private IOciRegistryContentClient CreateBlobClient(RootConfiguration configuration, OciArtifactModuleReference moduleReference, bool anonymousAccess) => anonymousAccess
            ? this.clientFactory.CreateAnonymousBlobClient(configuration, GetRegistryUri(moduleReference), moduleReference.Repository)
            : this.clientFactory.CreateAuthenticatedBlobClient(configuration, GetRegistryUri(moduleReference), moduleReference.Repository);

        private static async Task<(OciManifest, Stream, string)> DownloadModuleManifestAsync(OciArtifactModuleReference moduleReference, IOciRegistryContentClient client)
        {
            Response<GetManifestResult> manifestResponse;
            try
            {
                // either Tag or Digest is null (enforced by reference parser)
                var tagOrDigest = moduleReference.Tag
                    ?? moduleReference.Digest
                    ?? throw new ArgumentNullException(nameof(moduleReference), $"The specified module reference has both {nameof(moduleReference.Tag)} and {nameof(moduleReference.Digest)} set to null.");

                manifestResponse = await client.GetManifestAsync(tagOrDigest);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                // manifest does not exist
                throw new OciModuleRegistryException("The module does not exist in the registry.", exception);
            }
            Debug.Assert(manifestResponse.Value.Manifest.ToArray().Length > 0);

            // the Value is disposable, but we are not calling it because we need to pass the stream outside of this scope
            var stream = manifestResponse.Value.Manifest.ToStream();

            // BUG: The SDK internally consumed the stream for validation purposes and left position at the end
            stream.Position = 0;
            ValidateModuleManifestResponse(manifestResponse);

            // the SDK doesn't expose all the manifest properties we need
            // so we need to deserialize the manifest ourselves to get everything
            stream.Position = 0;
            var deserialized = DeserializeManifest(stream);
            stream.Position = 0;

            return (deserialized, stream, manifestResponse.Value.Digest);
        }

        private static void ValidateModuleManifestResponse(Response<GetManifestResult> manifestResponse)
        {
            var digestFromRegistry = manifestResponse.Value.Digest;
            var stream = manifestResponse.Value.Manifest.ToStream();

            string digestFromContent = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);

            if (!string.Equals(digestFromRegistry, digestFromContent, DigestComparison))
            {
                throw new OciModuleRegistryException($"There is a mismatch in the manifest digests. Received content digest = {digestFromContent}, Digest in registry response = {digestFromRegistry}");
            }
        }

        private static async Task<Stream> GetArmTemplateFromModuleManifestAsync(IOciRegistryContentClient client, OciManifest manifest)
        {
            // Bicep versions before 0.14 used to publish modules without the artifactType field set in the OCI manifest,
            // so we must allow null here
            if (manifest.ArtifactType is not null && !string.Equals(manifest.ArtifactType, BicepMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Expected OCI artifact to have the artifactType field set to either null or '{BicepMediaTypes.BicepModuleArtifactType}' but found '{manifest.ArtifactType}'.", InvalidModuleExceptionKind.WrongArtifactType);
            }
            ValidateModuleManifestConfig(manifest.Config);

            if (manifest.Layers.Length != 1)
            {
                throw new InvalidModuleException("Expected a single layer in the OCI artifact.");
            }

            var layer = manifest.Layers.Single();

            return await ProcessModuleManifestLayerAsync(client, layer);
        }

        private static void ValidateModuleManifestConfig(OciDescriptor config)
        {
            // media types are case insensitive
            if (!string.Equals(config.MediaType, BicepMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Expected module manifest to have media type \"{BicepMediaTypes.BicepModuleConfigV1}\", but found \"{config.MediaType}\"", InvalidModuleExceptionKind.WrongModuleLayerMediaType);
            }

            if (config.Size != 0)
            {
                throw new InvalidModuleException("Expected an empty config blob.");
            }
        }

        private static async Task<Stream> ProcessModuleManifestLayerAsync(IOciRegistryContentClient client, OciDescriptor layer)
        {
            if (!string.Equals(layer.MediaType, BicepMediaTypes.BicepModuleLayerV1Json, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Expected main module manifest layer to have media type \"{layer.MediaType}\", but found \"{layer.MediaType}\"", InvalidModuleExceptionKind.WrongModuleLayerMediaType);
            }

            return await DownloadBlobAsync(client, layer.Digest, layer.Size);
        }

        private static async Task<Stream> DownloadBlobAsync(IOciRegistryContentClient client, string digest, long expectedSize)
        {
            Response<DownloadRegistryBlobResult> blobResponse;
            try
            {
                blobResponse = await client.DownloadBlobContentAsync(digest);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                throw new InvalidModuleException($"Could not find container registry blob with digest \"{digest}\".", exception);
            }

            var stream = blobResponse.Value.Content.ToStream();

            if (expectedSize != stream.Length)
            {
                throw new InvalidModuleException($"Expected container registry blob with digest {digest} to have a size of {expectedSize} bytes but it contains {stream.Length} bytes.");
            }

            stream.Position = 0;
            string digestFromContents = DescriptorFactory.ComputeDigest(DescriptorFactory.AlgorithmIdentifierSha256, stream);
            stream.Position = 0;

            if (!string.Equals(digest, digestFromContents, DigestComparison))
            {
                throw new InvalidModuleException($"There is a mismatch in the module's container registry digests. Received content digest = {digestFromContents}, requested digest = {digest}");
            }

            return blobResponse.Value.Content.ToStream();
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
    }
}
