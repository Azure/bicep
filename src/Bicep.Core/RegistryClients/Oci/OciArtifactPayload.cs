// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Artifacts;

namespace Bicep.Core.RegistryClients.Oci;

/// <summary>
/// The content of a blob to push, before it has a descriptor.
/// </summary>
public sealed record OciBlob(string MediaType, BinaryData Data, ImmutableDictionary<string, string>? Annotations = null);

/// <summary>
/// An OCI manifest and the blobs it references, keyed by digest. It's the same model for pushing and pulling, and it
/// knows nothing about Bicep.
/// </summary>
/// <remarks>
/// The manifest bytes are the truth: a pulled payload keeps the exact bytes it was given (so that the digest matches),
/// and a created payload serializes its manifest once. An empty config (see <see cref="OciDescriptor.IsEmpty"/>) may
/// have no blob, because pulling skips it.
/// </remarks>
public sealed class OciArtifactPayload
{
    public OciArtifactPayload(BinaryData manifestData, ImmutableDictionary<string, BinaryData> blobsByDigest)
    {
        this.ManifestData = manifestData;
        this.BlobsByDigest = blobsByDigest;
        this.Manifest = OciManifest.FromBinaryData(manifestData) ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");
        this.ManifestDigest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, manifestData);

        foreach (var layer in this.Manifest.Layers)
        {
            if (!blobsByDigest.ContainsKey(layer.Digest))
            {
                throw new ArgumentException($"The payload has no blob for the layer with digest \"{layer.Digest}\".", nameof(blobsByDigest));
            }
        }
    }

    public BinaryData ManifestData { get; }

    public ImmutableDictionary<string, BinaryData> BlobsByDigest { get; }

    public OciManifest Manifest { get; }

    public string ManifestDigest { get; }

    public BinaryData? Config => this.TryGetBlob(this.Manifest.Config);

    public BinaryData? TryGetBlob(OciDescriptor descriptor) => this.BlobsByDigest.TryGetValue(descriptor.Digest, out var data) ? data : null;

    public BinaryData GetLayer(OciDescriptor layer) => this.BlobsByDigest[layer.Digest];

    /// <summary>
    /// Gets the first layer with the media type, if any.
    /// </summary>
    public BinaryData? TryGetLayer(string mediaType) =>
        this.Manifest.Layers.FirstOrDefault(layer => BicepMediaTypes.MediaTypeComparer.Equals(layer.MediaType, mediaType)) is { } layer
            ? this.GetLayer(layer)
            : null;

    public static OciArtifactPayload Create(
        string? artifactType,
        OciBlob config,
        IEnumerable<OciBlob> layers,
        ImmutableDictionary<string, string> annotations,
        string? mediaType = BicepMediaTypes.OciImageManifest)
    {
        var blobsByDigest = ImmutableDictionary.CreateBuilder<string, BinaryData>();
        OciDescriptor AddBlob(OciBlob blob)
        {
            var descriptor = new OciDescriptor(blob.Data, blob.MediaType, blob.Annotations);
            blobsByDigest[descriptor.Digest] = blob.Data;
            return descriptor;
        }

        var configDescriptor = AddBlob(config);
        var layerDescriptors = layers.Select(AddBlob).ToImmutableArray();
        var manifest = new OciManifest(2, mediaType, artifactType, configDescriptor, layerDescriptors, annotations);
        var manifestData = BinaryData.FromBytes(JsonSerializer.SerializeToUtf8Bytes(manifest, OciManifestSerializationContext.Default.OciManifest));

        return new(manifestData, blobsByDigest.ToImmutable());
    }
}
