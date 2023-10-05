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
    public class OciArtifactResult
    {
        public OciArtifactResult(BinaryData manifestBits, string manifestDigest, ImmutableArray<(string MediaType, BinaryData Data)> layers)
        {
            this.manifestBits = manifestBits;
            this.Manifest = OciManifest.FromBinaryData(manifestBits) ?? throw new InvalidModuleException("Unable to deserialize OCI manifest");
            this.ManifestDigest = manifestDigest;
            this.Layers = layers;
        }

        private readonly BinaryData manifestBits;

        public Stream ToStream() => manifestBits.ToStream();

        public OciManifest Manifest { get; init; }

        public string ManifestDigest { get; init; }

        public IEnumerable<(string MediaType, BinaryData Data)> Layers { get; init; }

        public BinaryData GetSingleLayerByMediaType(string mediaType)
        {
            var filtered = Layers.Where(l => BicepMediaTypes.MediaTypeComparer.Equals(l.MediaType, mediaType));
            if (filtered.Count() > 1)
            {
                throw new InvalidModuleException($"Expecting only a single layer with mediaType \"{mediaType}\", but found {filtered.Count()}");
            }
            else
            {
                return filtered.FirstOrDefault().Data;
            }
        }
    }
}
