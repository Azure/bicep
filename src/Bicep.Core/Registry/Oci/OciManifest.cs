// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    public class OciManifest
    {
        public OciManifest(int schemaVersion, string? artifactType, OciDescriptor config, IEnumerable<OciDescriptor> layers, IDictionary<string, string>? annotations = null)
        {
            this.Annotations = annotations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
            this.SchemaVersion = schemaVersion;
            this.ArtifactType = artifactType;
            this.Config = config;
            this.Layers = layers.ToImmutableArray();
        }

        public int SchemaVersion { get; }

        public string? ArtifactType { get; }

        public OciDescriptor Config { get; }

        public ImmutableArray<OciDescriptor> Layers { get; }

        /// <summary>
        /// Additional information provided through arbitrary metadata.
        /// </summary>
        public ImmutableDictionary<string, string> Annotations { get; }
    }
}
