// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;

namespace Bicep.Core.RegistryClients.Oci.AzureSdk;

internal sealed class AcrRegistrySession : IRegistrySession
{
    private readonly AzureContainerRegistryManager manager;
    private readonly IBicepCloudConfiguration cloud;

    public AcrRegistrySession(AzureContainerRegistryManager manager, IBicepCloudConfiguration cloud)
    {
        this.manager = manager;
        this.cloud = cloud;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task PushAsync(IOciArtifactReference target, OciArtifactPayload payload, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(target.Tag))
        {
            throw new InvalidOperationException("Push operations require a tag when using Azure Container Registry.");
        }

        return manager.PushArtifactAsync(cloud, target, payload, cancellationToken);
    }

    public Task<OciArtifactPayload> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken) =>
        manager.PullArtifactAsync(cloud, reference, cancellationToken);

    public async Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken)
    {
        var payload = await manager.PullArtifactAsync(cloud, reference, cancellationToken);
        return (payload.ManifestDigest, payload.Manifest);
    }
}
