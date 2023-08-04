// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    public class OciDescriptor
    {
        public OciDescriptor(string mediaType, string digest, long size, IDictionary<string, string>? annotations)
        {
            this.MediaType = mediaType;
            this.Digest = digest;
            this.Size = size;

            // Don't output annotations if it's empty
            this.Annotations = (annotations?.Count > 0) ? annotations.ToImmutableDictionary() : null;
        }

        public string MediaType { get; }

        public string Digest { get; }

        public long Size { get; }

        public ImmutableDictionary<string, string>? Annotations { get; }
    }
}
