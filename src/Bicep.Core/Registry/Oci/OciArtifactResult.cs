// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    public record OciArtifactLayer(string Digest, string MediaType, BinaryData Data);
    public abstract class OciArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers)
    {
        // media types are case-insensitive (they are lowercase by convention only)
        public static readonly StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;
        private readonly BinaryData manifestBits = manifestBits;

        public Stream ToStream() => manifestBits.ToStream();

        public OciManifest Manifest { get; init; } = OciManifest.FromBinaryData(manifestBits) ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");

        public string ManifestDigest { get; init; } = manifestDigest;

        public IEnumerable<OciArtifactLayer> Layers { get; init; } = layers.ToImmutableArray();

        public BinaryData? TryGetSingleLayerByMediaType(string mediaType)
        {
            return Layers.FirstOrDefault(l => BicepMediaTypes.MediaTypeComparer.Equals(l.MediaType, mediaType))?.Data;
        }

        public abstract OciArtifactLayer GetMainLayer();

    }
}
