// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Bicep.Core.Registry.Oci;
using Bicep.Core.RegistryClient.Models;

namespace Bicep.Core.RegistryClient
{
    public class BicepRegistryBlobClient
    {
        private readonly Uri _endpoint;
        private readonly string _registryName;
        private readonly HttpPipeline _pipeline;
        private readonly HttpPipeline _acrAuthPipeline;
        private readonly ClientDiagnostics _clientDiagnostics;
        private readonly ContainerRegistryRestClient _restClient;
        private readonly AuthenticationRestClient _acrAuthClient;
        private readonly ContainerRegistryBlobRestClient _blobRestClient;
        private readonly string _repositoryName;

        public BicepRegistryBlobClient(Uri endpoint, TokenCredential credential, string repositoryName) : this(endpoint, credential, repositoryName, new ContainerRegistryClientOptions())
        {
        }

        public BicepRegistryBlobClient(Uri endpoint, TokenCredential credential, string repositoryName, ContainerRegistryClientOptions options)
        {
            Argument.AssertNotNull(endpoint, nameof(endpoint));
            Argument.AssertNotNull(credential, nameof(credential));
            Argument.AssertNotNull(options, nameof(options));

            _endpoint = endpoint;
            _registryName = endpoint.Host.Split('.')[0];
            _repositoryName = repositoryName;
            _clientDiagnostics = new ClientDiagnostics(options);

            _acrAuthPipeline = HttpPipelineBuilder.Build(options);
            _acrAuthClient = new AuthenticationRestClient(_clientDiagnostics, _acrAuthPipeline, endpoint.AbsoluteUri);

            _pipeline = HttpPipelineBuilder.Build(options, new ContainerRegistryChallengeAuthenticationPolicy(credential, options.AuthenticationScope, _acrAuthClient));
            _restClient = new ContainerRegistryRestClient(_clientDiagnostics, _pipeline, _endpoint.AbsoluteUri);
            _blobRestClient = new ContainerRegistryBlobRestClient(_clientDiagnostics, _pipeline, _endpoint.AbsoluteUri);
        }

        // allows mocking
        protected BicepRegistryBlobClient()
        {
        }

        public virtual async Task<Response<UploadBlobResult>> UploadBlobAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            string digest = DigestHelper.ComputeDigest(DigestHelper.AlgorithmIdentifierSha256, stream);

            ResponseWithHeaders<ContainerRegistryBlobStartUploadHeaders> startUploadResult =
                await _blobRestClient.StartUploadAsync(_repositoryName, cancellationToken).ConfigureAwait(false);

            stream.Position = 0;
            ResponseWithHeaders<ContainerRegistryBlobUploadChunkHeaders> uploadChunkResult =
                await _blobRestClient.UploadChunkAsync(startUploadResult.Headers.Location, stream, cancellationToken).ConfigureAwait(false);

            ResponseWithHeaders<ContainerRegistryBlobCompleteUploadHeaders> completeUploadResult =
                await _blobRestClient.CompleteUploadAsync(digest, uploadChunkResult.Headers.Location, null, cancellationToken).ConfigureAwait(false);

            return Response.FromValue(new UploadBlobResult(), completeUploadResult.GetRawResponse());
        }

        public virtual async Task<Response<UploadManifestResult>> UploadManifestAsync(Stream stream, UploadManifestOptions options = default, CancellationToken cancellationToken = default)
        {
            options ??= new UploadManifestOptions();

            string reference = options.Tag ?? DigestHelper.ComputeDigest(DigestHelper.AlgorithmIdentifierSha256, stream);
            stream.Position = 0;
            ResponseWithHeaders<ContainerRegistryCreateManifestHeaders> response = await _restClient.CreateManifestAsync(_repositoryName, reference, options.MediaType, stream, cancellationToken).ConfigureAwait(false);

            return Response.FromValue(new UploadManifestResult(), response.GetRawResponse());
        }

        public virtual async Task<Response<DownloadManifestResult>> DownloadManifestAsync(string reference, DownloadManifestOptions options = default, CancellationToken cancellationToken = default)
        {
            options ??= new DownloadManifestOptions();

            Response<ManifestWrapper> manifestWrapper = await _restClient.GetManifestAsync(_repositoryName, reference, options.MediaType.ToSerialString(), cancellationToken).ConfigureAwait(false);

            manifestWrapper.GetRawResponse().Headers.TryGetValue("Docker-Content-Digest", out var digest);

            Stream stream = manifestWrapper.GetRawResponse().ContentStream;
            stream.Position = 0;

            return Response.FromValue(new DownloadManifestResult(digest, stream), manifestWrapper.GetRawResponse());
        }

        public virtual async Task<Response<DownloadBlobResult>> DownloadBlobAsync(string digest, DownloadBlobOptions options = default, CancellationToken cancellationToken = default)
        {
            options ??= new DownloadBlobOptions();
            ResponseWithHeaders<Stream, ContainerRegistryBlobGetBlobHeaders> blobResult = await _blobRestClient.GetBlobAsync(_repositoryName, digest, cancellationToken).ConfigureAwait(false);
            return Response.FromValue(new DownloadBlobResult(digest, blobResult.Value), blobResult.GetRawResponse());
        }
    }
}
