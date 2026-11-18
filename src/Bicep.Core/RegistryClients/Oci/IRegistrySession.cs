// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.RegistryClients.Oci;

/// <summary>
/// A handle to push/pull/resolve a single OCI artifact against a specific (registry, repository) pair.
/// </summary>
public interface IRegistrySession : IAsyncDisposable
{
    Task PushAsync(IOciArtifactReference target, OciArtifactPayload payload, CancellationToken cancellationToken);

    Task<OciArtifactPayload> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken);

    Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken);
}
