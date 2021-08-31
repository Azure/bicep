// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    public class OciManifest
    {
        // TODO: Add top-level annotations
        public OciManifest(int schemaVersion, OciDescriptor config, IEnumerable<OciDescriptor> layers)
        {
            this.SchemaVersion = schemaVersion;
            this.Config = config;
            this.Layers = layers.ToImmutableArray();
        }

        public int SchemaVersion { get; }

        public OciDescriptor Config { get; }

        public ImmutableArray<OciDescriptor> Layers { get; }
    }
}
