// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Modules
{
    public static class BicepModuleMediaTypes
    {
        // media types are case-insensitive (they are lowercase by convention only)
        public static readonly IEqualityComparer<string> MediaTypeComparer = StringComparer.OrdinalIgnoreCase;

        // Media types format - see https://github.com/opencontainers/image-spec/blob/main/manifest.md

        public const string BicepModuleArtifactType = "application/vnd.ms.bicep.module.artifact";
        public const string BicepModuleConfigV1 = "application/vnd.ms.bicep.module.config.v1+json";
        public const string BicepModuleLayerV1Json = "application/vnd.ms.bicep.module.layer.v1+json";
        public const string BicepSourceV1Layer = "application/vnd.ms.bicep.module.source.v1.tar+gzip";
    }
}
