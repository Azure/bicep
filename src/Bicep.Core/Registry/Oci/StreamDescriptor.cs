// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Bicep.Core.Registry.Oci
{
    public class StreamDescriptor
    {
        public StreamDescriptor(Stream stream, string mediaType, IDictionary<string, string>? annotations = null)
        {
            Stream = stream;
            MediaType = mediaType;
            Annotations = annotations?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
        }

        public Stream Stream { get; }

        public string MediaType { get; }

        public ImmutableDictionary<string, string> Annotations { get; }

        public void ResetStream() => this.Stream.Position = 0;
    }
}
