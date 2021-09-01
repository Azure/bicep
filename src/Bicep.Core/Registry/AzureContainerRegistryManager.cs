// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Core;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.RegistryClient;
using Bicep.Core.RegistryClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadManifestOptions = Bicep.Core.RegistryClient.UploadManifestOptions;

namespace Bicep.Core.Registry
{
    public class AzureContainerRegistryManager
    {
        // media types are case-insensitive (they are lowercase by convention only)
        private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;
        private const StringComparison DigestComparison = StringComparison.Ordinal;

        private readonly string artifactCachePath;
        private readonly TokenCredential tokenCredential;
        private readonly IContainerRegistryClientFactory clientFactory;

        public AzureContainerRegistryManager(string artifactCachePath, TokenCredential tokenCredential, IContainerRegistryClientFactory clientFactory)
        {
            this.artifactCachePath = artifactCachePath;
            this.tokenCredential = tokenCredential;
            this.clientFactory = clientFactory;
        }

        public async Task<OciClientResult> PullArtifactsync(OciArtifactModuleReference moduleReference)
        {
            try
            {
                await PullArtifactInternalAsync(moduleReference);

                return new(true, null);
            }
            catch(AcrManagerException exception)
            {
                // we can trust the message in our own exception
                return new(false, exception.Message);
            }
            catch(Exception exception)
            {
                return new(false, $"Unhandled exception: {exception}");
            }
        }

        public async Task PushArtifactAsync(OciArtifactModuleReference moduleReference, StreamDescriptor config, params StreamDescriptor[] layers)
        {
            // TODO: Add similar exception handling as in the pull* method

            // TODO: How do we choose this? Does it ever change?
            var algorithmIdentifier = DescriptorFactory.AlgorithmIdentifierSha256;

            var blobClient = this.CreateBlobClient(moduleReference);

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

            var manifest = new OciManifest(2, configDescriptor, layerDescriptors);
            using var manifestStream = new MemoryStream();
            OciManifestSerialization.SerializeManifest(manifestStream, manifest);

            manifestStream.Position = 0;
            // BUG: the client closes the stream :(
            var manifestUploadResult = await blobClient.UploadManifestAsync(manifestStream, new UploadManifestOptions(ContentType.ApplicationVndOciImageManifestV1Json, moduleReference.Tag));
        }

        public string GetLocalPackageDirectory(OciArtifactModuleReference reference)
        {
            var baseDirectories = new[]
            {
                this.artifactCachePath,
                reference.Registry
            };

            // TODO: Directory convention problematic. /foo/bar:baz and /foo:bar will share directories
            var directories = baseDirectories
                .Concat(reference.Repository.Split('/', StringSplitOptions.RemoveEmptyEntries))
                .Append(reference.Tag)
                .ToArray();

            return Path.Combine(directories);
        }

        public string GetLocalPackageEntryPointPath(OciArtifactModuleReference reference) => Path.Combine(this.GetLocalPackageDirectory(reference), "main.json");

        private static Uri GetRegistryUri(OciArtifactModuleReference moduleReference) => new Uri($"https://{moduleReference.Registry}");

        private BicepRegistryBlobClient CreateBlobClient(OciArtifactModuleReference moduleReference) => this.clientFactory.CreateBlobClient(GetRegistryUri(moduleReference), moduleReference.Repository, this.tokenCredential);

        private static void CreateModuleDirectory(string modulePath)
        {
            try
            {
                // ensure that the directory exists
                Directory.CreateDirectory(modulePath);
            }
            catch (Exception exception)
            {
                throw new AcrManagerException($"Unable to create the local module directory \"{modulePath}\". {exception.Message}", exception);
            }
        }

        private async Task PullArtifactInternalAsync(OciArtifactModuleReference moduleReference)
        {
            var client = this.CreateBlobClient(moduleReference);
            var manifest = await DownloadManifestAsync(moduleReference, client);

            // this has to be after downloading the manifest so we don't create directories for non-existent modules
            string modulePath = GetLocalPackageDirectory(moduleReference);
            CreateModuleDirectory(modulePath);

            await ProcessManifest(client, manifest, modulePath);
        }

        private static async Task<OciManifest> DownloadManifestAsync(OciArtifactModuleReference moduleReference, BicepRegistryBlobClient client)
        {
            Response<DownloadManifestResult> manifestResponse;
            try
            {
                manifestResponse = await client.DownloadManifestAsync(moduleReference.Tag, new DownloadManifestOptions(ContentType.ApplicationVndOciImageManifestV1Json));
            }
            catch(RequestFailedException exception) when (exception.Status == 404)
            {
                // manifest does not exist
                throw new AcrManagerException("The module does not exist in the registry.", exception);
            }

            ValidateManifestResponse(manifestResponse);

            // the SDK doesn't expose all the manifest properties we need
            // so we need to deserialize the manifest ourselves to get everything
            var stream = manifestResponse.Value.Content;
            stream.Position = 0;
            return DeserializeManifest(stream);
        }

        private static void ValidateManifestResponse(Response<DownloadManifestResult> manifestResponse)
        {
            var digestFromRegistry = manifestResponse.Value.Digest;

            var stream = manifestResponse.Value.Content;
            stream.Position = 0;

            // TODO: The registry may use a different digest algorithm - we need to handle that
            string digestFromContent = DigestHelper.ComputeDigest(DigestHelper.AlgorithmIdentifierSha256, stream);

            if (!string.Equals(digestFromRegistry, digestFromContent, DigestComparison))
            {
                throw new AcrManagerException($"There is a mismatch in the manifest digests. Received content digest = {digestFromContent}, Digest in registry response = {digestFromRegistry}");
            }
        }

        private static async Task ProcessManifest(BicepRegistryBlobClient client, OciManifest manifest, string modulePath)
        {
            ProcessConfig(manifest.Config);
            if (manifest.Layers.Length != 1)
            {
                throw new InvalidModuleException("Expected a single layer in the OCI artifact.");
            }

            var layer = manifest.Layers.Single();

            await ProcessLayer(client, layer, modulePath);
        }

        private static void ValidateBlobResponse(Response<DownloadBlobResult> blobResponse, OciDescriptor descriptor)
        {
            var stream = blobResponse.Value.Content;

            if(descriptor.Size != stream.Length)
            {
                throw new InvalidModuleException($"Expected blob size of {descriptor.Size} bytes but received {stream.Length} bytes from the registry.");
            }

            stream.Position = 0;
            string digestFromContents = DigestHelper.ComputeDigest(DigestHelper.AlgorithmIdentifierSha256, stream);
            stream.Position = 0;

            if(!string.Equals(descriptor.Digest, digestFromContents, DigestComparison))
            {
                throw new InvalidModuleException($"There is a mismatch in the layer digests. Received content digest = {digestFromContents}, Requested digest = {descriptor.Digest}");
            }
        }

        private static async Task ProcessLayer(BicepRegistryBlobClient client, OciDescriptor layer, string modulePath)
        {
            if(!string.Equals(layer.MediaType, BicepMediaTypes.BicepModuleLayerV1Json, MediaTypeComparison))
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

            var layerPath = Path.Combine(modulePath, "main.json");

            using var fileStream = new FileStream(layerPath, FileMode.Create);
            await blobResult.Value.Content.CopyToAsync(fileStream);
        }

        private static void ProcessConfig(OciDescriptor config)
        {
            // media types are case insensitive
            if(!string.Equals(config.MediaType, BicepMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
            {
                throw new InvalidModuleException($"Did not expect config media type \"{config.MediaType}\".");
            }

            if(config.Size != 0)
            {
                throw new InvalidModuleException("Expected an empty config blob.");
            }
        }

        private static OciManifest DeserializeManifest(Stream stream)
        {
            try
            {

                return OciManifestSerialization.DeserializeManifest(stream);
            }
            catch(Exception exception)
            {
                throw new InvalidModuleException("Unable to deserialize the module manifest.", exception);
            }
        }

        private class AcrManagerException : Exception
        {
            public AcrManagerException(string message) : base(message)
            {
            }

            public AcrManagerException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        private class InvalidModuleException : AcrManagerException
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
