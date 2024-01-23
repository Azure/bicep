// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Bicep.Core.Registry.Oci
{
    [JsonSerializable(typeof(OciManifest))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class OciManifestSerializationContext : JsonSerializerContext { }

    public class OciManifest
    {
        [JsonConstructor]
        public OciManifest(
            int schemaVersion,
            string? mediaType,
            string? artifactType,
            OciDescriptor config,
            ImmutableArray<OciDescriptor> layers,
            ImmutableDictionary<string, string> annotations)
        {
            this.Annotations = annotations;
            this.SchemaVersion = schemaVersion;
            this.MediaType = mediaType;
            this.ArtifactType = artifactType;
            this.Config = config;
            this.Layers = layers;
        }

        public int SchemaVersion { get; }

        public string? MediaType { get; }

        public string? ArtifactType { get; }

        public OciDescriptor Config { get; }

        public ImmutableArray<OciDescriptor> Layers { get; }

        /// <summary>
        /// Additional information provided through arbitrary metadata.
        /// </summary>
        public ImmutableDictionary<string, string> Annotations { get; }

        public static OciManifest? FromBinaryData(BinaryData data)
            => JsonSerializer.Deserialize(data, OciManifestSerializationContext.Default.OciManifest);
    }
}
