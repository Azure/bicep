// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Bicep.Core.Registry.Oci
{
    public class OciArtifactResult
    {
        public OciArtifactResult(BinaryData manifestBits, string manifestDigest, ImmutableDictionary<string, BinaryData> layers)
        {
            this.manifest = manifestBits;
            this.serializedManifest = OciManifest.FromBinaryData(manifestBits) ?? throw new InvalidOperationException("the manifest is not a valid OCI manifest");
            this.manifestDigest = manifestDigest;
            this.layers = layers;
        }

        private readonly BinaryData manifest;
        private readonly string manifestDigest;
        private readonly OciManifest serializedManifest;
        private readonly ImmutableDictionary<string, BinaryData> layers;

        public Stream ToStream() => manifest.ToStream();

        public OciManifest Manifest => serializedManifest;

        public string ManifestDigest => manifestDigest;

        public IEnumerable<BinaryData> Layers => layers.Values;
    }
}