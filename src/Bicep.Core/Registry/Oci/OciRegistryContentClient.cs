// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.Modules;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Bicep.Core.Emit.ResourceDependencyVisitor;

namespace Bicep.Core.Registry.Oci;

public class OciRegistryContentClient : IOciRegistryContentClient
{
    private readonly ContainerRegistryContentClient _client;
    private readonly Uri _endpoint;
    private readonly string _repositoryName;

    public OciRegistryContentClient(Uri endpoint, string repositoryName, ContainerRegistryClientOptions options)
    {
        _client = new ContainerRegistryContentClient(endpoint, repositoryName, options);
        _endpoint = endpoint;
        _repositoryName = repositoryName;
    }

    public OciRegistryContentClient(Uri endpoint, string repositoryName, TokenCredential credential, ContainerRegistryClientOptions options)
    {
        _client = new ContainerRegistryContentClient(endpoint, repositoryName, credential, options);
        _endpoint = endpoint;
        _repositoryName = repositoryName;
    }

    #region ContainerRegistryContentClient functionality forwarded

    public Task<Response<SetManifestResult>> SetManifestAsync(OciImageManifest manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        => _client.SetManifestAsync(manifest, tag, mediaType, cancellationToken);

    public Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default)
        => _client.SetManifestAsync(manifest, tag, mediaType, cancellationToken);

    public Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream content, CancellationToken cancellationToken = default)
        => _client.UploadBlobAsync(content, cancellationToken);

    public Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default)
        => _client.DownloadBlobContentAsync(digest, cancellationToken);

    public Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default)
        => _client.GetManifestAsync(tagOrDigest, cancellationToken);

    #endregion ContainerRegistryContentClient functionality forwarded

    public async Task<Response> SendGetReferrersRequestAsync(string manifestDigest)
    {
        Debug.Assert(manifestDigest != null);

        // https://github.com/opencontainers/distribution-spec/blob/main/spec.md#listing-referrers
        var request = _client.Pipeline.CreateRequest();
        request.Method = RequestMethod.Get;
        request.Uri.Reset(_endpoint);
        request.Uri.AppendPath("/v2/", false);
        request.Uri.AppendPath(_repositoryName, true);
        request.Uri.AppendPath("/referrers/", false);
        request.Uri.AppendPath(manifestDigest);

        return await _client.Pipeline.SendRequestAsync(request, CancellationToken.None);
    }
}
