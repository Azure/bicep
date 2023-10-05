// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Bicep.Core.Registry.Oci
{
    public static class BicepMediaTypes
    {
        // media types are case-insensitive (they are lowercase by convention only)
        public static readonly IEqualityComparer<string> MediaTypeComparer = StringComparer.OrdinalIgnoreCase;

        // Media types format - see https://github.com/opencontainers/image-spec/blob/main/manifest.md

        // Provider Media Types
        public const string BicepProviderArtifactType = "application/vnd.ms.bicep.provider.artifact";
        public const string BicepProviderConfigV1 = "application/vnd.ms.bicep.provider.config.v1+json";        
        public const string BicepProviderArtifactLayerV1TarGzip = "application/vnd.ms.bicep.provider.layer.v1.tar+gzip";
    }
}
