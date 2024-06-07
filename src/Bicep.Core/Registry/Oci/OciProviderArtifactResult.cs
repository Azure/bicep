// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci
{
    public class OciProviderArtifactResult : OciArtifactResult
    {
        private readonly OciArtifactLayer mainLayer;

        public OciProviderArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers, OciArtifactLayer? config) :
            base(manifestBits, manifestDigest, layers)
        {
            var manifest = this.Manifest;
            if (manifest.ArtifactType is null || !manifest.ArtifactType.Equals(BicepMediaTypes.BicepProviderArtifactType, MediaTypeComparison))
            {
                throw new InvalidArtifactException($"Unknown artifactType: '{manifest.ArtifactType}'.", InvalidArtifactExceptionKind.WrongArtifactType);
            }
            if (!manifest.Config.MediaType.Equals(BicepMediaTypes.BicepProviderConfigV1, MediaTypeComparison))
            {
                throw new InvalidArtifactException($"Unknown config.mediaType: '{manifest.Config.MediaType}'.", InvalidArtifactExceptionKind.WrongArtifactType);
            }

            var expectedLayerMediaType = BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip;
            this.mainLayer = this.Layers.Where(l => l.MediaType.Equals(expectedLayerMediaType, MediaTypeComparison)).Single();
            Config = config;
        }

        public override OciArtifactLayer GetMainLayer() => this.mainLayer;

        public OciArtifactLayer? Config { get; }
    }
}
