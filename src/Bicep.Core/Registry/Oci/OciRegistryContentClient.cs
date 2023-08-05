// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Core;
using Bicep.Core.Extensions;
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

    public async Task<IEnumerable<(string digest, string? artifactType)>> GetReferrersAsync(string mainManifestDigest)
    {
        Debug.Assert(mainManifestDigest != null);

        IEnumerable<(string digest, string? artifactType)>? referrers = null;

        var request = _client.Pipeline.CreateRequest();
        request.Method = RequestMethod.Get;
        request.Uri.Reset(_endpoint);
        request.Uri.AppendPath("/v2/", false);
        request.Uri.AppendPath(_repositoryName, true);
        request.Uri.AppendPath("/referrers/", false);
        request.Uri.AppendPath(mainManifestDigest);

        var response = await _client.Pipeline.SendRequestAsync(request, CancellationToken.None);
        if (response.IsError)
        {
            throw new Exception($"Unable to retrieve source manifests. Referrers API failed with status code {response.Status}");
        }

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var referrersResponse = JsonSerializer.Deserialize<JsonElement>(response.Content.ToString());
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        /* Example JSON result:
            {
              "schemaVersion": 2,
              "mediaType": "application/vnd.oci.image.index.v1+json",
              "manifests": [
                {
                  "mediaType": "application/vnd.oci.image.manifest.v1+json",
                  "digest": "sha256:210a9f9e8134fc77940ea17f971adcf8752e36b513eb7982223caa1120774284",
                  "size": 811,
                  "artifactType": "application/vnd.ms.bicep.module.sources"
                },
                ...
        */

        referrers = referrersResponse.TryGetPropertyByPath("manifests")
            ?.EnumerateArray()
            .Select<JsonElement, (string? digest, string? artifactType)>(
                m => (m.GetProperty("digest").GetString(), m.GetProperty("artifactType").GetString()))
            .Where(m => m.digest is not null)
            .Select(m => (m.artifactType!, m.digest));

        return referrers ?? Enumerable.Empty<(string, string?)>();
    }
}
