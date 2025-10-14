// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
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

    public Task PushAsync(RegistryRef target, RegistryArtifact artifact, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(target.Tag))
        {
            throw new InvalidOperationException("Push operations require a tag when using Azure Container Registry.");
        }

        var annotationsBuilder = new OciManifestAnnotationsBuilder();
        foreach (var annotation in artifact.Annotations)
        {
            annotationsBuilder.Add(annotation.Key, annotation.Value);
        }

        return manager.PushArtifactAsync(
            cloud,
            new SessionOciArtifactReference(target),
            artifact.MediaType,
            artifact.ArtifactType,
            artifact.Config,
            artifact.Layers,
            annotationsBuilder,
            cancellationToken);
    }

    public async Task<RegistryArtifact> PullAsync(RegistryRef reference, CancellationToken cancellationToken)
    {
        var ociReference = new SessionOciArtifactReference(reference);
        var result = await manager.PullArtifactAsync(cloud, ociReference, cancellationToken).ConfigureAwait(false);

        var annotations = result.Manifest.Annotations ?? ImmutableDictionary<string, string>.Empty;

        var layerDescriptors = result.Layers
            .Select(layer => new OciDescriptor(layer.Data, layer.MediaType))
            .ToImmutableArray();

        var configDescriptor = result.Manifest.Config;
        if (result is OciExtensionArtifactResult extensionResult && extensionResult.Config is not null)
        {
            configDescriptor = new OciDescriptor(extensionResult.Config.Data, extensionResult.Config.MediaType);
        }

        return new RegistryArtifact(
            result.Manifest.MediaType,
            result.Manifest.ArtifactType,
            configDescriptor,
            layerDescriptors,
            annotations);
    }

    public async Task<RegistryManifestInfo> ResolveAsync(RegistryRef reference, CancellationToken cancellationToken)
    {
        var artifact = await PullAsync(reference, cancellationToken).ConfigureAwait(false);

        var manifest = new OciManifest(
            schemaVersion: 2,
            mediaType: artifact.MediaType,
            artifactType: artifact.ArtifactType,
            config: artifact.Config,
            layers: artifact.Layers,
            annotations: artifact.Annotations);

#pragma warning disable IL2026
        var manifestData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
#pragma warning restore IL2026
        var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, manifestData);

        return new RegistryManifestInfo(
            digest,
            artifact.MediaType,
            artifact.ArtifactType,
            artifact.Annotations);
    }

    private sealed class SessionOciArtifactReference : IOciArtifactReference
    {
        private readonly RegistryRef reference;

        public SessionOciArtifactReference(RegistryRef reference)
        {
            this.reference = reference;
        }

        public string Registry => reference.Host;

        public string Repository => reference.Repository;

        public string? Tag => reference.Tag;

        public string? Digest => reference.Digest;

        public string ArtifactId =>
            Digest is not null
                ? $"{Registry}/{Repository}@{Digest}"
                : $"{Registry}/{Repository}:{Tag}";

        public string FullyQualifiedReference => $"{OciArtifactReferenceFacts.SchemeWithColon}{ArtifactId}";
    }
}
