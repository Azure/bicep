// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.Registry.Oci;

public class OciRegistryContentClient : IOciRegistryContentClient
{
    private readonly ContainerRegistryContentClient client;

    public OciRegistryContentClient(Uri endpoint, string repositoryName, ContainerRegistryClientOptions options)
    {
        client = new ContainerRegistryContentClient(endpoint, repositoryName, options);
    }

    public OciRegistryContentClient(Uri endpoint, string repositoryName, TokenCredential credential, ContainerRegistryClientOptions options)
    {
        client = new ContainerRegistryContentClient(endpoint, repositoryName, credential, options);
    }

    public Task<Response<SetManifestResult>> SetManifestAsync(OciImageManifest manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        => client.SetManifestAsync(manifest, tag, mediaType, cancellationToken);

    public Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        => client.SetManifestAsync(manifest, tag, mediaType, cancellationToken);

    public Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream content, CancellationToken cancellationToken = default)
        => client.UploadBlobAsync(content, cancellationToken);

    public Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default)
        => client.DownloadBlobContentAsync(digest, cancellationToken);

    public Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default)
        => client.GetManifestAsync(tagOrDigest, cancellationToken);
}
