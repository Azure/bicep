// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Artifacts;
using Bicep.Core.Configuration;
using OrasProject.Oras.Exceptions;
using OrasProject.Oras.Oci;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasRepositoryClient = OrasProject.Oras.Registry.Remote.Repository;

namespace Bicep.Core.RegistryClients.Oci.Oras;

internal sealed class OrasRegistrySession(
    OrasOciRegistryTransport transport,
    IBicepCloudConfiguration cloud,
    string registry,
    string repository,
    bool useDockerCredentials = true) : IRegistrySession
{
    private const int MaxConcurrentLayerDownloads = 5;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public async Task PushAsync(IOciArtifactReference target, OciArtifactPayload payload, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(target.Tag))
        {
            throw new InvalidOperationException("Push operations require a tag.");
        }

        var manifestDescriptor = new Descriptor
        {
            Digest = payload.ManifestDigest,
            MediaType = BicepMediaTypes.OciImageManifest,
            Size = payload.ManifestData.ToMemory().Length,
        };

        await ExecuteAsync(async client =>
        {
            await PushBlobAsync(client, payload, payload.Manifest.Config, cancellationToken);
            foreach (var layer in payload.Manifest.Layers)
            {
                await PushBlobAsync(client, payload, layer, cancellationToken);
            }

            await using var stream = payload.ManifestData.ToStream();
            await client.PushAsync(manifestDescriptor, stream, target.Tag, cancellationToken);
            return true;
        }, cancellationToken, anonymousFirst: false);
    }

    public Task<OciArtifactPayload> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken) =>
        PullArtifactAsync(reference, cancellationToken);

    internal Task<OciArtifactPayload> PullArtifactAsync(IOciArtifactAddressComponents reference, CancellationToken cancellationToken) =>
        ExecuteAsync(async client =>
        {
            var (manifestData, _, manifest) = await ResolveManifestAsync(client, reference, cancellationToken);
            var blobsByDigest = ImmutableDictionary.CreateBuilder<string, BinaryData>();
            foreach (var batch in manifest.Layers.Chunk(MaxConcurrentLayerDownloads))
            {
                foreach (var (layer, data) in await Task.WhenAll(batch.Select(layer => FetchBlobAsync(client, layer, cancellationToken))))
                {
                    blobsByDigest[layer.Digest] = data;
                }
            }

            if (!manifest.Config.IsEmpty())
            {
                blobsByDigest[manifest.Config.Digest] = (await FetchBlobAsync(client, manifest.Config, cancellationToken)).Data;
            }

            return new OciArtifactPayload(manifestData, blobsByDigest.ToImmutable());
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

    private static async Task<(OciDescriptor Descriptor, BinaryData Data)> FetchBlobAsync(OrasRepositoryClient client, OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        try
        {
            var (_, stream) = await client.Blobs.FetchAsync(descriptor.Digest, cancellationToken);
            await using var content = stream;
            // Validate against the manifest, not a descriptor supplied by the blob response.
            var data = await ReadContentAsync(content, ToOrasDescriptor(descriptor), cancellationToken);
            return (descriptor, data);
        }
        catch (NotFoundException exception)
        {
            throw new InvalidArtifactException($"The manifest refers to a blob that does not exist in the registry (digest \"{descriptor.Digest}\").", exception);
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

    private static async Task PushBlobAsync(OrasRepositoryClient client, OciArtifactPayload payload, OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        if (payload.TryGetBlob(descriptor) is { } data)
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
