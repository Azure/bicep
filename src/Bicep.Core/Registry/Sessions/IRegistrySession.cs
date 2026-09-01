// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;

namespace Bicep.Core.Registry.Sessions;

/// <summary>
/// A handle to push/pull/resolve a single OCI artifact against a specific (registry, repository) pair.
/// </summary>
public interface IRegistrySession : IAsyncDisposable
{
    Task PushAsync(
        IOciArtifactReference target,
        string? mediaType,
        string? artifactType,
        OciDescriptor config,
        IEnumerable<OciDescriptor> layers,
        OciManifestAnnotationsBuilder annotations,
        CancellationToken cancellationToken);

    Task<OciArtifactResult> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken);

    Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken);
}
