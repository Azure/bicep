// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Core;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Azure.Containers.ContainerRegistry.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public class AzureContainerRegistryManager
    {
        // media types are case-insensitive (they are lowercase by convention only)
        private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;

        private readonly string artifactCachePath;
        private readonly TokenCredential tokenCredential;
        private readonly IContainerRegistryClientFactory clientFactory;

        public AzureContainerRegistryManager(string artifactCachePath, TokenCredential tokenCredential, IContainerRegistryClientFactory clientFactory)
        {
            this.artifactCachePath = artifactCachePath;
            this.tokenCredential = tokenCredential;
            this.clientFactory = clientFactory;
        }

        public async Task<OciClientResult> PullArtifactAsync(OciArtifactModuleReference moduleReference)
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

            var manifest = new OciManifest();
            config.ResetStream();
            var configDescriptor = DescriptorFactory.CreateDescriptor(algorithmIdentifier, config);

            config.ResetStream();
            var configUploadResult = await blobClient.UploadBlobAsync(config.Stream);
            manifest.Config = configDescriptor;

            var layerDescriptors = new List<OciBlobDescriptor>(layers.Length);
            foreach (var layer in layers)
            {
                layer.ResetStream();
                var layerDescriptor = DescriptorFactory.CreateDescriptor(algorithmIdentifier, layer);

                layer.ResetStream();
                var layerUploadResult = await blobClient.UploadBlobAsync(layer.Stream);

                manifest.Layers.Add(layerDescriptor);
            }

            // BUG: the client closes the stream :(
            var manifestUploadResult = await blobClient.UploadManifestAsync(manifest, new UploadManifestOptions() { Tag = moduleReference.Tag });
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

        private ContainerRegistryBlobClient CreateBlobClient(OciArtifactModuleReference moduleReference) => this.clientFactory.CreateBlobClient(GetRegistryUri(moduleReference), moduleReference.Repository, this.tokenCredential);

        private static void CreateModuleDirectory(string modulePath)
        {
            try
            {
                // ensure that the directory exists
                Directory.CreateDirectory(modulePath);
            }
            catch (Exception exception)
            {
                throw new AcrManagerException($"Unable to create the local module directory \"{modulePath}\".", exception);
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

        private static async Task<OciManifest> DownloadManifestAsync(OciArtifactModuleReference moduleReference, ContainerRegistryBlobClient client)
        {
            Response<DownloadManifestResult> manifestResponse;
            try
            {
                manifestResponse = await client.DownloadManifestAsync(moduleReference.Tag);
            }
            catch(RequestFailedException exception) when (exception.Status == 404)
            {
                // manifest does not exist
                throw new AcrManagerException("The module does not exist in the registry.", exception);
            }

            return manifestResponse.Value.Manifest;
        }

        private static async Task ProcessManifest(ContainerRegistryBlobClient client, OciManifest manifest, string modulePath)
        {
            ProcessConfig(manifest.Config);
            if (manifest.Layers.Count != 1)
            {
                throw new InvalidModuleException("Expected a single layer in the OCI artifact.");
            }

            var layer = manifest.Layers.Single();

            await ProcessLayer(client, layer, modulePath);
        }

        private static async Task ProcessLayer(ContainerRegistryBlobClient client, OciBlobDescriptor layer, string modulePath)
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

            var layerPath = Path.Combine(modulePath, "main.json");

            using var fileStream = new FileStream(layerPath, FileMode.Create);
            await blobResult.Value.Content.CopyToAsync(fileStream);
        }

        private static void ProcessConfig(OciBlobDescriptor config)
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

        //private static OciManifest DeserializeManifest(Stream stream)
        //{
        //    try
        //    {

        //        return OciManifestSerialization.DeserializeManifest(stream);
        //    }
        //    catch(Exception exception)
        //    {
        //        throw new InvalidModuleException("Unable to deserialize the module manifest.", exception);
        //    }
        //}

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
