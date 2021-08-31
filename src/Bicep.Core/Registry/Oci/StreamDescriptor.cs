// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Bicep.Core.Registry.Oci
{
    public class StreamDescriptor
    {
        public StreamDescriptor(Stream stream, string mediaType)
        {
            Stream = stream;
            MediaType = mediaType;
        }

        public Stream Stream { get; }

        public string MediaType { get; }

        public void ResetStream() => this.Stream.Position = 0;
    }
}
