// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.Registry.Oci;

/// <summary>
/// Client for interacting with OCI registry content. Makes it possible to mock other functionality that might be addede here.
/// </summary>
public interface IOciRegistryContentClient
{
    Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default);
    Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream content, CancellationToken cancellationToken = default);
    Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default);
    Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default);
}
