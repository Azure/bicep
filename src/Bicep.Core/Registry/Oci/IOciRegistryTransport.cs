// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;

namespace Bicep.Core.Registry.Oci;

public interface IOciRegistryTransport
{
    Task<string[]> GetRepositoryNamesAsync(CloudConfiguration cloud, string registry, int maxResults, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRepositoryTagsAsync(CloudConfiguration cloud, string registry, string repository, CancellationToken cancellationToken = default);

    Task<OciArtifactResult> PullArtifactAsync(CloudConfiguration cloud, IOciArtifactAddressComponents artifactReference, CancellationToken cancellationToken = default);

    Task PushArtifactAsync(
        CloudConfiguration cloud,
        IOciArtifactReference artifactReference,
        string? mediaType,
        string? artifactType,
        OciDescriptor config,
        IEnumerable<OciDescriptor> layers,
        OciManifestAnnotationsBuilder annotations,
        CancellationToken cancellationToken = default);
}
