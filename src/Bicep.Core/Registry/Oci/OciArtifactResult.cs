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
    public class OciArtifactResult
    {
        public OciArtifactResult(BinaryData manifestBits, string manifestDigest, ImmutableList<(string MediaType, BinaryData Data)> layers)
        {
            this.manifest = manifestBits;
            this.serializedManifest = OciManifest.FromBinaryData(manifestBits) ?? throw new InvalidOperationException("the manifest is not a valid OCI manifest");
            this.manifestDigest = manifestDigest;
            this.layers = layers;
        }

        private readonly BinaryData manifest;
        private readonly string manifestDigest;
        private readonly OciManifest serializedManifest;
        private readonly ImmutableList<(string MediaType, BinaryData Data)> layers;

        public Stream ToStream() => manifest.ToStream();

        public OciManifest Manifest => serializedManifest;

        public string ManifestDigest => manifestDigest;

        public IEnumerable<(string MediaType, BinaryData Data)> Layers => layers;
    }
}
