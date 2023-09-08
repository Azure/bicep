// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Bicep.Core.Registry.Oci
{
    public class OciDescriptor
    {
        [JsonConstructor]
        public OciDescriptor(string mediaType, string digest, long size, ImmutableDictionary<string, string>? annotations)
        {
            this.MediaType = mediaType;
            this.Digest = digest;
            this.Size = size;
            this.Annotations = annotations ?? ImmutableDictionary<string, string>.Empty;
        }
        public string MediaType { get; }

        public string Digest { get; }

        public long Size { get; }

        public ImmutableDictionary<string, string>? Annotations { get; }
    }
}
