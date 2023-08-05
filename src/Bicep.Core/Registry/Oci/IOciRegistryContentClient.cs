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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Bicep.Core.Emit.ResourceDependencyVisitor;

namespace Bicep.Core.Registry.Oci;

/// <summary>
/// Client for interacting with OCI registry content. Makes it possible to mock GetReferrersAsync.
/// </summary>
public interface IOciRegistryContentClient
{
    #region ContainerRegistryContentClient functionality forwarded

    Task<Response<SetManifestResult>> SetManifestAsync(BinaryData manifest, string? tag = default, ManifestMediaType? mediaType = default, CancellationToken cancellationToken = default);
    Task<Response<UploadRegistryBlobResult>> UploadBlobAsync(Stream content, CancellationToken cancellationToken = default);
    Task<Response<DownloadRegistryBlobResult>> DownloadBlobContentAsync(string digest, CancellationToken cancellationToken = default);
    Task<Response<GetManifestResult>> GetManifestAsync(string tagOrDigest, CancellationToken cancellationToken = default);

    #endregion ContainerRegistryContentClient functionality forwarded

    /// <summary>
    /// Retrieves all manifests that reference the main manifest as its attached parent ("Subject")
    /// </summary>
    /// <param name="manifestDigest">A manifest digest.</param>
    /// <returns>The info for all manifests whose Subject points to the given manifestDigest</returns>
    Task<IEnumerable<(string digest, string? artifactType)>> GetReferrersAsync(string manifestDigest);
}
