// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Azure;

internal sealed class AcrRegistrySession : IRegistrySession
{
    private readonly AzureContainerRegistryManager manager;
    private readonly CloudConfiguration cloud;

    public AcrRegistrySession(AzureContainerRegistryManager manager, CloudConfiguration cloud)
    {
        this.manager = manager;
        this.cloud = cloud;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task PushAsync(
        IOciArtifactReference target,
        string? mediaType,
        string? artifactType,
        OciDescriptor config,
        IEnumerable<OciDescriptor> layers,
        OciManifestAnnotationsBuilder annotations,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(target.Tag))
        {
            throw new InvalidOperationException("Push operations require a tag when using Azure Container Registry.");
        }

        return manager.PushArtifactAsync(
            cloud,
            target,
            mediaType,
            artifactType,
            config,
            layers,
            annotations,
            cancellationToken);
    }

    public Task<OciArtifactResult> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken) =>
        manager.PullArtifactAsync(cloud, reference, cancellationToken);

    public async Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken)
    {
        var result = await manager.PullArtifactAsync(cloud, reference, cancellationToken);
        return (result.ManifestDigest, result.Manifest);
    }
}
