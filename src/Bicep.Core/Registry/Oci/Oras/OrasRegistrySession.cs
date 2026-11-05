// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Sessions;
using OrasProject.Oras.Exceptions;
using OrasProject.Oras.Oci;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasRepositoryClient = OrasProject.Oras.Registry.Remote.Repository;

namespace Bicep.Core.Registry.Oci.Oras;

internal sealed class OrasRegistrySession(
    OrasOciRegistryTransport transport,
    IBicepCloudConfiguration cloud,
    string registry,
    string repository,
    bool useDockerCredentials = true) : IRegistrySession
{
    private const int MaxConcurrentLayerDownloads = 5;

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

        var layerDescriptors = layers.ToImmutableArray();
        var manifest = new OciManifest(2, mediaType, artifactType, config, layerDescriptors, annotations.Build());
#pragma warning disable IL2026
        var manifestData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
#pragma warning restore IL2026
        var manifestDescriptor = new Descriptor
        {
            Digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, manifestData),
            MediaType = BicepMediaTypes.OciImageManifest,
            Size = manifestData.ToMemory().Length,
        };

        await ExecuteAsync(async client =>
        {
            await PushDescriptorAsync(client, config, cancellationToken);
            foreach (var layer in layerDescriptors)
            {
                await PushDescriptorAsync(client, layer, cancellationToken);
            }

            await using var stream = manifestData.ToStream();
            await client.PushAsync(manifestDescriptor, stream, target.Tag, cancellationToken);
            return true;
        }, cancellationToken, anonymousFirst: false);
    }

    public Task<OciArtifactResult> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken) =>
        PullArtifactAsync(reference, cancellationToken);

    internal Task<OciArtifactResult> PullArtifactAsync(IOciArtifactAddressComponents reference, CancellationToken cancellationToken) =>
        ExecuteAsync(async client =>
        {
            var (manifestData, digest, manifest) = await ResolveManifestAsync(client, reference, cancellationToken);
            var layers = ImmutableArray.CreateBuilder<OciArtifactLayer>();
            foreach (var batch in manifest.Layers.Chunk(MaxConcurrentLayerDownloads))
            {
                layers.AddRange(await Task.WhenAll(batch.Select(layer => FetchLayerAsync(client, layer, cancellationToken))));
            }

            var config = !manifest.Config.IsEmpty() ? await FetchLayerAsync(client, manifest.Config, cancellationToken) : null;
            return OciArtifactResultFactory.Create(manifest, manifestData, digest, layers.ToImmutable(), config);
        }, cancellationToken);

    public Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken) =>
        ExecuteAsync(async client =>
        {
            var (_, digest, manifest) = await ResolveManifestAsync(client, reference, cancellationToken);
            return (digest, manifest);
        }, cancellationToken);

    private Task<T> ExecuteAsync<T>(Func<OrasRepositoryClient, Task<T>> operation, CancellationToken cancellationToken, bool anonymousFirst = true) =>
        transport.ExecuteAsync(cloud, registry,
            client => operation(OrasOciRegistryTransport.CreateRepository(client, registry, repository)),
            cancellationToken, anonymousFirst, useDockerCredentials);

    private static async Task<(BinaryData Data, string Digest, OciManifest Manifest)> ResolveManifestAsync(
        OrasRepositoryClient client, IOciArtifactAddressComponents reference, CancellationToken cancellationToken)
    {
        var tagOrDigest = reference.Tag ?? reference.Digest ?? throw new InvalidOperationException("Reference must contain a tag or digest.");
        var (descriptor, stream) = await client.FetchAsync(tagOrDigest, cancellationToken);
        await using var content = stream;
        var data = await ReadContentAsync(content, descriptor, cancellationToken);
        if (reference.Digest is { } expectedDigest && !string.Equals(expectedDigest, descriptor.Digest, StringComparison.Ordinal))
        {
            throw new InvalidArtifactException($"The manifest digest \"{descriptor.Digest}\" does not match the requested digest \"{expectedDigest}\".");
        }

        try
        {
            var manifest = OciManifest.FromBinaryData(data) ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");
            return (data, descriptor.Digest, manifest);
        }
        catch (JsonException exception)
        {
            throw new InvalidArtifactException("Unable to deserialize OCI manifest", exception);
        }
    }

    private static async Task<OciArtifactLayer> FetchLayerAsync(OrasRepositoryClient client, OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        try
        {
            var (_, stream) = await client.Blobs.FetchAsync(descriptor.Digest, cancellationToken);
            await using var content = stream;
            // Validate against the manifest, not a descriptor supplied by the blob response.
            var data = await ReadContentAsync(content, ToOrasDescriptor(descriptor), cancellationToken);
            return new OciArtifactLayer(descriptor.Digest, descriptor.MediaType, data);
        }
        catch (NotFoundException exception)
        {
            throw new InvalidArtifactException($"Module manifest refers to a non-existent blob with digest \"{descriptor.Digest}\".", exception);
        }
    }

    private static async Task<BinaryData> ReadContentAsync(Stream content, Descriptor descriptor, CancellationToken cancellationToken)
    {
        var data = await BinaryData.FromStreamAsync(content, cancellationToken);
        var size = data.ToMemory().Length;
        if (size != descriptor.Size)
        {
            throw new InvalidArtifactException($"Expected content size of {descriptor.Size} bytes but received {size} bytes from the registry.");
        }

        var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, data);
        if (!string.Equals(digest, descriptor.Digest, StringComparison.Ordinal))
        {
            throw new InvalidArtifactException($"There is a mismatch in the content digests. Received content digest = {digest}, Expected digest = {descriptor.Digest}");
        }

        return data;
    }

    private static async Task PushDescriptorAsync(OrasRepositoryClient client, OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        if (descriptor.Data is { } data)
        {
            await using var stream = data.ToStream();
            await client.Blobs.PushAsync(ToOrasDescriptor(descriptor), stream, cancellationToken);
        }
    }

    private static Descriptor ToOrasDescriptor(OciDescriptor descriptor) => new()
    {
        Digest = descriptor.Digest,
        MediaType = descriptor.MediaType,
        Size = descriptor.Size,
        Annotations = descriptor.Annotations.Any() ? descriptor.Annotations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : null,
    };
}
