// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Bicep.Core.Registry.Oci
{
    public class OciDescriptor
    {
        public OciDescriptor(string mediaType, string digest, long size, IDictionary<string, string>? annotations)
        {
            this.MediaType = mediaType;
            this.Digest = digest;
            this.Size = size;
            this.Annotations = annotations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
        }
        [JsonPropertyName("mediaType")]
        public string MediaType { get; }

        [JsonPropertyName("digest")]
        public string Digest { get; }

        [JsonPropertyName("size")]
        public long Size { get; }

        // TODO: Skip serialization for empty annotations
        [JsonPropertyName("annotations")]
        public ImmutableDictionary<string, string> Annotations { get; }
    }
}
