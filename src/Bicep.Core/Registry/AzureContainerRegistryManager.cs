// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
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
    public class AzureContainerRegistryManager : IOciRegistryTransport
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

        public async Task<string[]> GetRepositoryNamesAsync(
            CloudConfiguration cloud,
            string registry,
            int maxResults,
            CancellationToken cancellationToken = default)
        {
            var registryUri = GetRegistryUri(registry);

            // Note: This won't work for MCR
            static async Task<string[]> GetCatalogAsync(ContainerRegistryClient client, int maxResults, CancellationToken cancellationToken)
            {
                List<string> catalog = [];

                await foreach (var repository in client.GetRepositoryNamesAsync(cancellationToken))
                {
                    // CONSIDER: Allow user to configure a filter for repository names
                    //   Question: If the user has a module alias set up in bicepconfig.json that doesn't match
                    //     the filter, should we still show it in the list?
                    //   Question: What if the user specifically presses CTRL+ENTER on a path that doesn't match the filter?

                    if (catalog.Count >= maxResults)
                    {
                        Trace.WriteLine($"Stopping catalog enumeration after reaching {maxResults} repositories.");
                        break;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    catalog.Add(repository);
                }

                return [.. catalog];
            }

            try
            {
                // Try authenticated client first.
                Trace.WriteLine($"Attempt to list catalog for registry {registryUri} using authentication.");
                return await GetCatalogInternalAsync(anonymousAccess: false, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException exception) when (exception.Status == 401 || exception.Status == 403)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to list catalog for registry {registryUri} failed, received code {exception.Status}. Falling back to anonymous.");
                return await GetCatalogInternalAsync(anonymousAccess: true, cancellationToken).ConfigureAwait(false);
            }
            catch (CredentialUnavailableException)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to pull catalog for registry {registryUri} failed due to missing login step. Falling back to anonymous.");
                return await GetCatalogInternalAsync(anonymousAccess: true, cancellationToken).ConfigureAwait(false);
            }

            async Task<string[]> GetCatalogInternalAsync(bool anonymousAccess, CancellationToken cancellationToken)
            {
                var client = CreateContainerClient(cloud, registryUri, anonymousAccess);
                return await GetCatalogAsync(client, maxResults, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IReadOnlyList<string>> GetRepositoryTagsAsync(
            CloudConfiguration cloud,
            string registry,
            string repository,
            CancellationToken cancellationToken = default)
        {
            var registryUri = GetRegistryUri(registry);

            try
            {
                // Try authenticated client first.
                Trace.WriteLine($"Attempting to list repository tags for module {registryUri}/{repository} using authentication.");
                return await GetTagsInternalAsync(anonymousAccess: false, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException exception) when (exception.Status == 401 || exception.Status == 403)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to list repository tags for module {registryUri}/{repository} failed, received code {exception.Status}. Falling back to anonymous.");
                return await GetTagsInternalAsync(anonymousAccess: true, cancellationToken).ConfigureAwait(false);
            }
            catch (CredentialUnavailableException)
            {
                // Fall back to anonymous client.
                Trace.WriteLine($"Authenticated attempt to list repository tags for module {registryUri}/{repository} failed due to missing login step. Falling back to anonymous.");
                return await GetTagsInternalAsync(anonymousAccess: true, cancellationToken).ConfigureAwait(false);
            }

            async Task<IReadOnlyList<string>> GetTagsInternalAsync(bool anonymousAccess, CancellationToken cancellationToken)
            {
                var client = CreateContainerClient(cloud, registryUri, anonymousAccess);

                var tags = new List<string>();
                await foreach (var manifestProps in client.GetRepository(repository).GetAllManifestPropertiesAsync(cancellationToken: cancellationToken))
                {
                    foreach (var tag in manifestProps.Tags)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        tags.Add(tag);
                    }
                }

                return [.. tags];
            }
        }

        public async Task<OciArtifactResult> PullArtifactAsync(
            CloudConfiguration cloud,
            IOciArtifactAddressComponents artifactReference,
            CancellationToken cancellationToken = default)
        {
            async Task<OciArtifactResult> DownloadManifestInternalAsync(bool anonymousAccess, CancellationToken cancellationToken)
            {
                var client = CreateBlobClient(cloud, artifactReference, anonymousAccess);
                return await DownloadManifestAndLayersAsync(artifactReference, client, cancellationToken).ConfigureAwait(false);
            }

            try
            {
                // Try anonymous auth first.
                Trace.WriteLine($"Attempting to pull artifact for module {artifactReference.ArtifactId} with anonymous authentication.");
                return await DownloadManifestInternalAsync(anonymousAccess: true, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidArtifactException invalidArtifactException)
            {
                Trace.WriteLine($"Anonymous authentication failed with invalid artifact exception: {invalidArtifactException.Message}. Not retrying.");
                throw;
            }
            catch (RequestFailedException requestedFailedException) when (requestedFailedException.Status is 401 or 403)
            {
                Trace.WriteLine($"Anonymous authentication failed with status code {requestedFailedException.Status}. Retrying with authenticated client.");
            }
            catch (Exception exception)
            {
                Trace.WriteLine($"Anonymous authentication failed with unexpected exception {exception.Message}. Retrying with authenticated client.");
            }

            // Fall back to authenticated client.
            return await DownloadManifestInternalAsync(anonymousAccess: false, cancellationToken).ConfigureAwait(false);
        }

        public async Task PushArtifactAsync(
            CloudConfiguration cloud,
            IOciArtifactReference artifactReference,
            string? mediaType,
            string? artifactType,
            OciDescriptor config,
            IEnumerable<OciDescriptor> layers,
            OciManifestAnnotationsBuilder annotations,
            CancellationToken cancellationToken = default)
        {
            // push is not supported anonymously
            var blobClient = this.CreateBlobClient(cloud, artifactReference, anonymousAccess: false);

            _ = await blobClient.UploadBlobAsync(config.Data, cancellationToken).ConfigureAwait(false);

            var layerDescriptors = layers.ToImmutableArray();
            foreach (var layer in layerDescriptors)
            {
                layerDescriptors.Add(layer);
                _ = await blobClient.UploadBlobAsync(layer.Data, cancellationToken).ConfigureAwait(false);
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

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var manifestBinaryData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            _ = await blobClient.SetManifestAsync(manifestBinaryData, artifactReference.Tag, mediaType: ManifestMediaType.OciImageManifest, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private static Uri GetRegistryUri(IOciArtifactAddressComponents artifactReference) => GetRegistryUri(artifactReference.Registry);
        private static Uri GetRegistryUri(string loginServer) => new($"https://{loginServer}");

        private ContainerRegistryContentClient CreateBlobClient(
            CloudConfiguration cloud,
            IOciArtifactAddressComponents artifactReference,
            bool anonymousAccess) => anonymousAccess
            ? this.clientFactory.CreateAnonymousBlobClient(cloud, GetRegistryUri(artifactReference), artifactReference.Repository)
            : this.clientFactory.CreateAuthenticatedBlobClient(cloud, GetRegistryUri(artifactReference), artifactReference.Repository);

        private ContainerRegistryClient CreateContainerClient(
            CloudConfiguration cloud,
            Uri registryUri,
            bool anonymousAccess) => anonymousAccess
            ? this.clientFactory.CreateAnonymousContainerClient(cloud, registryUri)
            : this.clientFactory.CreateAuthenticatedContainerClient(cloud, registryUri);

        private static async Task<OciArtifactResult> DownloadManifestAndLayersAsync(IOciArtifactAddressComponents artifactReference, ContainerRegistryContentClient client, CancellationToken cancellationToken)
        {
            GetManifestResult manifestResult;
            try
            {
                // either Tag or Digest is null (enforced by reference parser)
                var tagOrDigest = artifactReference.Tag
                    ?? artifactReference.Digest
                    ?? throw new ArgumentNullException($"The specified artifact reference '{artifactReference.ArtifactId}' has both {nameof(artifactReference.Tag)} and {nameof(artifactReference.Digest)} set to null.");

                manifestResult = await client.GetManifestAsync(tagOrDigest, cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                // manifest does not exist
                Trace.WriteLine($"Manifest for module {artifactReference.ArtifactId} could not be found in the registry.");
                throw new OciArtifactRegistryException("The artifact does not exist in the registry.", exception);
            }

            if (manifestResult.Manifest.ToArray().Length == 0)
            {
                throw new InvalidArtifactException("Invalid manifest");
            }

            // the Value is disposable, but we are not calling it because we need to pass the stream outside of this scope (and it will GC correctly)
            using var stream = manifestResult.Manifest.ToStream();

            // BUG: The SDK internally consumed the stream for validation purposes and left position at the end
            stream.Position = 0;
            ValidateManifestResponse(manifestResult);

            var deserializedManifest = OciManifest.FromBinaryData(manifestResult.Manifest) ?? throw new InvalidOperationException("the manifest is not a valid OCI manifest");
            var layerTasks = deserializedManifest.Layers.AsParallel().WithDegreeOfParallelism(5)
                .Select(async layer => new OciArtifactLayer(layer.Digest, layer.MediaType, await PullLayerAsync(client, layer, cancellationToken)));
            var layers = await Task.WhenAll(layerTasks);

            var config = !deserializedManifest.Config.IsEmpty() ?
                new OciArtifactLayer(deserializedManifest.Config.Digest, deserializedManifest.Config.MediaType, await PullLayerAsync(client, deserializedManifest.Config, cancellationToken)) :
                null;

            return deserializedManifest.ArtifactType switch
            {
                BicepMediaTypes.BicepModuleArtifactType or null => new OciModuleArtifactResult(manifestResult.Manifest, manifestResult.Digest, layers),
                BicepMediaTypes.BicepExtensionArtifactType => new OciExtensionArtifactResult(manifestResult.Manifest, manifestResult.Digest, layers, config),
                _ => throw new InvalidArtifactException($"artifacts of type: \'{deserializedManifest.ArtifactType}\' are not supported by this Bicep version. {OciModuleArtifactResult.NewerVersionMightBeRequired}")
            };
        }

        private static async Task<BinaryData> PullLayerAsync(ContainerRegistryContentClient client, OciDescriptor layer, CancellationToken cancellationToken = default)
        {
            DownloadRegistryBlobResult blobResult;
            try
            {
                blobResult = await client.DownloadBlobContentAsync(layer.Digest, cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                throw new InvalidArtifactException($"Module manifest refers to a non-existent blob with digest \"{layer.Digest}\".", exception);
            }

            ValidateBlobResponse(blobResult, layer);

            return blobResult.Content;
        }

        private static void ValidateBlobResponse(DownloadRegistryBlobResult blobResult, OciDescriptor descriptor)
        {
            var data = blobResult.Content;
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

        private static void ValidateManifestResponse(GetManifestResult manifest)
        {
            var digestFromRegistry = manifest.Digest;
            string digestFromContent = OciDescriptor.ComputeDigest(DigestAlgorithmIdentifier, manifest.Manifest);

            if (!string.Equals(digestFromRegistry, digestFromContent, DigestComparison))
            {
                throw new OciArtifactRegistryException($"There is a mismatch in the manifest digests. Received content digest = {digestFromContent}, Digest in registry response = {digestFromRegistry}");
            }
        }
    }
}
