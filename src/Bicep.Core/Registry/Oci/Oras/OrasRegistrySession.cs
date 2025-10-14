// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;
using OrasProject.Oras.Content;
using OrasProject.Oras.Oci;
using OrasProject.Oras.Registry.Remote;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasRepositoryClient = OrasProject.Oras.Registry.Remote.Repository;
using OrasReference = OrasProject.Oras.Registry.Reference;
using SessionRegistryArtifact = Bicep.Core.Registry.Sessions.RegistryArtifact;

namespace Bicep.Core.Registry.Oci.Oras;

internal sealed class OrasRegistrySession : IRegistrySession
{
    private readonly OrasRepositoryClient repositoryClient;

    public OrasRegistrySession(
        string registry,
        string repository,
        ICredentialChain credentialChain)
    {
        var credentialProvider = new CredentialChainCredentialProvider(credentialChain);
        var client = new Client(httpClient: null, credentialProvider: credentialProvider);
        client.SetUserAgent($"bicep/{ThisAssembly.AssemblyFileVersion}");

        repositoryClient = new OrasRepositoryClient(new RepositoryOptions
        {
            Client = client,
            Reference = OrasReference.Parse($"{registry}/{repository}"),
            PlainHttp = UsePlainHttp(registry),
        });
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public async Task PushAsync(RegistryRef target, SessionRegistryArtifact artifact, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(target.Tag))
        {
            throw new InvalidOperationException("Push operations require a tag.");
        }

        var manifest = new OciManifest(
            schemaVersion: 2,
            mediaType: artifact.MediaType,
            artifactType: artifact.ArtifactType,
            config: artifact.Config,
            layers: artifact.Layers,
            annotations: artifact.Annotations);

#pragma warning disable IL2026
        var manifestBinaryData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
#pragma warning restore IL2026
        var manifestBytes = manifestBinaryData.ToArray();
        var manifestDigest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, manifestBinaryData);
        var manifestDescriptor = new Descriptor
        {
            Digest = manifestDigest,
            MediaType = ManifestMediaType.OciImageManifest.ToString(),
            Size = manifestBytes.Length,
            Annotations = artifact.Annotations.Any() ? artifact.Annotations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : null,
        };

        await PushDescriptorAsync(artifact.Config, cancellationToken).ConfigureAwait(false);
        foreach (var layer in artifact.Layers)
        {
            await PushDescriptorAsync(layer, cancellationToken).ConfigureAwait(false);
        }

        await using var stream = manifestBinaryData.ToStream();
        await repositoryClient.PushAsync(manifestDescriptor, stream, target.Tag, cancellationToken).ConfigureAwait(false);
    }

    public async Task<SessionRegistryArtifact> PullAsync(RegistryRef reference, CancellationToken cancellationToken)
    {
        var manifest = await ResolveManifestAsync(reference, cancellationToken).ConfigureAwait(false);
        var layers = await FetchLayersAsync(manifest.Manifest, cancellationToken).ConfigureAwait(false);

        OciArtifactLayer? configLayer = null;
        if (!manifest.Manifest.Config.IsEmpty())
        {
            configLayer = await FetchLayerAsync(manifest.Manifest.Config, cancellationToken).ConfigureAwait(false);
        }

        var annotations = manifest.Manifest.Annotations ?? ImmutableDictionary<string, string>.Empty;

        var configDescriptor = configLayer is null
            ? manifest.Manifest.Config
            : CreateDescriptor(configLayer);

        var layerDescriptors = layers.Select(CreateDescriptor).ToImmutableArray();

        return new SessionRegistryArtifact(
            manifest.Manifest.MediaType,
            manifest.Manifest.ArtifactType,
            configDescriptor,
            layerDescriptors,
            annotations);
    }

    public async Task<RegistryManifestInfo> ResolveAsync(RegistryRef reference, CancellationToken cancellationToken)
    {
        var manifest = await ResolveManifestAsync(reference, cancellationToken).ConfigureAwait(false);
        return new RegistryManifestInfo(
            manifest.Descriptor.Digest,
            manifest.Manifest.MediaType,
            manifest.Manifest.ArtifactType,
            manifest.Manifest.Annotations ?? ImmutableDictionary<string, string>.Empty);
    }

    private async Task<(Descriptor Descriptor, OciManifest Manifest)> ResolveManifestAsync(RegistryRef reference, CancellationToken cancellationToken)
    {
        var tagOrDigest = reference.Tag ?? reference.Digest ?? throw new InvalidOperationException("Reference must contain a tag or digest.");
        var (descriptor, stream) = await repositoryClient.FetchAsync(tagOrDigest, cancellationToken).ConfigureAwait(false);
        await using var manifestContent = stream;
        var manifestBytes = await manifestContent.ReadAllAsync(descriptor, cancellationToken).ConfigureAwait(false);
        var manifestBinaryData = BinaryData.FromBytes(manifestBytes);
        var manifest = OciManifest.FromBinaryData(manifestBinaryData) ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");
        return (descriptor, manifest);
    }

    private async Task<IReadOnlyList<OciArtifactLayer>> FetchLayersAsync(OciManifest manifest, CancellationToken cancellationToken)
    {
        var tasks = manifest.Layers.Select(layer => FetchLayerAsync(layer, cancellationToken));
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task<OciArtifactLayer> FetchLayerAsync(OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        var (fetchedDescriptor, stream) = await repositoryClient.Blobs.FetchAsync(descriptor.Digest, cancellationToken).ConfigureAwait(false);
        await using var content = stream;
        var bytes = await content.ReadAllAsync(fetchedDescriptor, cancellationToken).ConfigureAwait(false);
        return new OciArtifactLayer(descriptor.Digest, descriptor.MediaType, BinaryData.FromBytes(bytes));
    }

    private async Task PushDescriptorAsync(OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        if (descriptor.Data is null)
        {
            return;
        }

        var orasDescriptor = new Descriptor
        {
            Digest = descriptor.Digest,
            MediaType = descriptor.MediaType,
            Size = descriptor.Size,
            Annotations = descriptor.Annotations.Any()
                ? descriptor.Annotations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : null,
        };

        await using var stream = descriptor.Data.ToStream();
        await repositoryClient.Blobs.PushAsync(orasDescriptor, stream, cancellationToken).ConfigureAwait(false);
    }

    private static bool UsePlainHttp(string registry)
    {
        if (Uri.TryCreate($"https://{registry}", UriKind.Absolute, out var uri) &&
            IPAddress.TryParse(uri.Host, out var address))
        {
            return IPAddress.IsLoopback(address);
        }

        return registry.Contains("localhost", StringComparison.OrdinalIgnoreCase);
    }

    private static OciDescriptor CreateDescriptor(OciArtifactLayer layer) =>
        new(layer.Data, layer.MediaType);
}
