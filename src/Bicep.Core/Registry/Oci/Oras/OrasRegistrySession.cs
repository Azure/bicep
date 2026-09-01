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
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;
using OrasProject.Oras.Content;
using OrasProject.Oras.Oci;
using OrasProject.Oras.Registry.Remote;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasRepositoryClient = OrasProject.Oras.Registry.Remote.Repository;
using OrasReference = OrasProject.Oras.Registry.Reference;

namespace Bicep.Core.Registry.Oci.Oras;

internal sealed class OrasRegistrySession : IRegistrySession
{
    private readonly OrasRepositoryClient repositoryClient;

    public OrasRegistrySession(
        string registry,
        string repository,
        ICredentialProvider credentialProvider)
    {
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

    public async Task PushAsync(
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
            throw new InvalidOperationException("Push operations require a tag.");
        }

        var layersArray = layers.ToImmutableArray();
        var builtAnnotations = annotations.Build();

        var manifest = new OciManifest(
            schemaVersion: 2,
            mediaType: mediaType,
            artifactType: artifactType,
            config: config,
            layers: layersArray,
            annotations: builtAnnotations);

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
            Annotations = builtAnnotations.Any() ? builtAnnotations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : null,
        };

        await PushDescriptorAsync(config, cancellationToken);
        foreach (var layer in layersArray)
        {
            await PushDescriptorAsync(layer, cancellationToken);
        }

        await using var stream = manifestBinaryData.ToStream();
        await repositoryClient.PushAsync(manifestDescriptor, stream, target.Tag, cancellationToken);
    }

    public async Task<OciArtifactResult> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken)
    {
        var (manifestData, manifestDigest, manifest) = await ResolveManifestAsync(reference, cancellationToken);
        var layers = await FetchLayersAsync(manifest, cancellationToken);

        OciArtifactLayer? configLayer = null;
        if (!manifest.Config.IsEmpty())
        {
            configLayer = await FetchLayerAsync(manifest.Config, cancellationToken);
        }

        return OciArtifactResultFactory.Create(manifest, manifestData, manifestDigest, [.. layers], configLayer);
    }

    public async Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken)
    {
        var (_, digest, manifest) = await ResolveManifestAsync(reference, cancellationToken);
        return (digest, manifest);
    }

    private async Task<(BinaryData ManifestData, string Digest, OciManifest Manifest)> ResolveManifestAsync(IOciArtifactReference reference, CancellationToken cancellationToken)
    {
        var tagOrDigest = reference.Tag ?? reference.Digest ?? throw new InvalidOperationException("Reference must contain a tag or digest.");
        var (descriptor, stream) = await repositoryClient.FetchAsync(tagOrDigest, cancellationToken);
        await using var manifestContent = stream;
        var manifestBytes = await manifestContent.ReadAllAsync(descriptor, cancellationToken);
        var manifestBinaryData = BinaryData.FromBytes(manifestBytes);
        var manifest = OciManifest.FromBinaryData(manifestBinaryData) ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");
        return (manifestBinaryData, descriptor.Digest, manifest);
    }

    private async Task<IReadOnlyList<OciArtifactLayer>> FetchLayersAsync(OciManifest manifest, CancellationToken cancellationToken)
    {
        var tasks = manifest.Layers.Select(layer => FetchLayerAsync(layer, cancellationToken));
        return await Task.WhenAll(tasks);
    }

    private async Task<OciArtifactLayer> FetchLayerAsync(OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        var (fetchedDescriptor, stream) = await repositoryClient.Blobs.FetchAsync(descriptor.Digest, cancellationToken);
        await using var content = stream;
        var bytes = await content.ReadAllAsync(fetchedDescriptor, cancellationToken);
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
        await repositoryClient.Blobs.PushAsync(orasDescriptor, stream, cancellationToken);
    }

    private static bool UsePlainHttp(string registry)
    {
        if (Uri.TryCreate($"https://{registry}", UriKind.Absolute, out var uri) &&
            IPAddress.TryParse(uri.Host, out var address))
        {
            return IPAddress.IsLoopback(address);
        }

        return string.Equals(uri?.Host, "localhost", StringComparison.OrdinalIgnoreCase);
    }
}
