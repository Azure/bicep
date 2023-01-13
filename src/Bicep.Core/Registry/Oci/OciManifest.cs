// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Azure.Containers.ContainerRegistry.Specialized;

namespace Bicep.Core.Registry.Oci
{
    public class OciManifest
    {
        public OciManifest(int schemaVersion, OciDescriptor config, IEnumerable<OciDescriptor> layers, OciAnnotations annotations)
        {
            this.SchemaVersion = schemaVersion;
            this.Config = config;
            this.Layers = layers.ToImmutableArray();
            this.Annotations = annotations;
        }

        public int SchemaVersion { get; }

        public OciDescriptor Config { get; }

        public ImmutableArray<OciDescriptor> Layers { get; }

        public OciAnnotations Annotations { get; }
    }
}
