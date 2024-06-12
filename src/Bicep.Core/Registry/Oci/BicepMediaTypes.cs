// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        public static string GetProviderArtifactLayerV1Binary(SupportedArchitecture architecture)
            => $"application/vnd.ms.bicep.provider.layer.v1.{architecture.Name}.binary";

        // Module Media Types
        public const string BicepModuleArtifactType = "application/vnd.ms.bicep.module.artifact";
        public const string BicepModuleConfigV1 = "application/vnd.ms.bicep.module.config.v1+json";
        public const string BicepModuleLayerV1Json = "application/vnd.ms.bicep.module.layer.v1+json";
        public const string BicepSourceV1Layer = "application/vnd.ms.bicep.module.source.v1.tar+gzip";
    }
}
