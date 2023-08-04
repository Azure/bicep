// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    public class OciManifest
    {
        public OciManifest(int schemaVersion, string? mediaType, string? artifactType, OciDescriptor config, IEnumerable<OciDescriptor> layers, OciDescriptor? subject = null, IDictionary<string, string>? annotations = null)
        {
            this.Annotations = (annotations?.Count > 0) ? annotations.ToImmutableDictionary() : null;
            this.SchemaVersion = schemaVersion;
            this.ArtifactType = artifactType;
            this.Config = config;
            this.Subject = subject;
            this.Layers = layers.ToImmutableArray();
            MediaType = mediaType;
        }

        public int SchemaVersion { get; }

        public string? MediaType { get; }
        public string? ArtifactType { get; }

        public OciDescriptor Config { get; }

        public ImmutableArray<OciDescriptor> Layers { get; }

        /// <summary>
        /// Reference to a separate manfest that this manifest is being attached to
        /// </summary>
        public OciDescriptor? Subject { get; }

        /// <summary>
        /// Additional information provided through arbitrary metadata.
        /// </summary>
        public ImmutableDictionary<string, string>? Annotations { get; }
    }
}
