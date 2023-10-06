// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;

namespace Bicep.Core.Registry.Oci
{
    public record OciArtifactLayer(string Digest, string MediaType, BinaryData Data);
    public abstract class OciArtifactResult
    {
        // media types are case-insensitive (they are lowercase by convention only)
        public static readonly StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;

        public OciArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers)
        {
            this.manifestBits = manifestBits;
            this.Manifest = OciManifest.FromBinaryData(manifestBits) ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");
            this.ManifestDigest = manifestDigest;
            this.Layers = layers.ToImmutableArray();
        }

        private readonly BinaryData manifestBits;

        public Stream ToStream() => manifestBits.ToStream();

        public OciManifest Manifest { get; init; }

        public string ManifestDigest { get; init; }

        public IEnumerable<OciArtifactLayer> Layers { get; init; }

        public BinaryData? TryGetSingleLayerByMediaType(string mediaType)
        {
            return Layers.FirstOrDefault(l => BicepMediaTypes.MediaTypeComparer.Equals(l.MediaType, mediaType))?.Data;
        }

        public abstract OciArtifactLayer GetMainLayer();

    }
}
