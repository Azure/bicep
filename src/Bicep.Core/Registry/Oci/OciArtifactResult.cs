// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Containers.ContainerRegistry;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Threading;
using Azure;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Bicep.Core.Registry.Oci
{
    public record OciArtifactLayer(string Digest, string MediaType, BinaryData Data);
    public class OciArtifactResult
    {
        public OciArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers)
        {
            this.manifestBits = manifestBits;
            this.Manifest = OciManifest.FromBinaryData(manifestBits) ?? throw new InvalidModuleException("Unable to deserialize OCI manifest");
            this.ManifestDigest = manifestDigest;
            this.Layers = layers.ToImmutableList();
        }

        private readonly BinaryData manifestBits;

        public Stream ToStream() => manifestBits.ToStream();

        public OciManifest Manifest { get; init; }

        public string ManifestDigest { get; init; }

        public IEnumerable<OciArtifactLayer> Layers { get; init; }

        public BinaryData GetSingleLayerByMediaType(string mediaType)
        {
            var filtered = Layers.Where(l => BicepMediaTypes.MediaTypeComparer.Equals(l.MediaType, mediaType));
            if (filtered.Count() != 1)
            {
                throw new InvalidModuleException($"Expecting only a single layer with mediaType \"{mediaType}\", but found {filtered.Count()}");
            }
            else
            {
                return filtered.First().Data;
            }
        }
    }

}
