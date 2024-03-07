// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;


namespace Bicep.Core.Modules
{
    public class OciModuleArtifactResult : OciArtifactResult
    {
        private readonly OciArtifactLayer mainLayer;
        public const string NewerVersionMightBeRequired = "A newer version of Bicep might be required to reference this artifact.";
        public OciModuleArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers) :
            base(manifestBits, manifestDigest, layers)
        {
            var manifest = this.Manifest;
            if (manifest.ArtifactType is not null && !manifest.ArtifactType.Equals(BicepMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
            {
                throw new InvalidArtifactException(
                   $"Expected OCI manifest artifactType value of '{BicepMediaTypes.BicepModuleArtifactType}' but found '{manifest.ArtifactType}'. {NewerVersionMightBeRequired}",
                   InvalidArtifactExceptionKind.WrongArtifactType);
            }
            if (manifest.Config.MediaType is not null && !manifest.Config.MediaType.Equals(BicepMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
            {
                throw new InvalidArtifactException($"Did not expect config media type \"{manifest.Config.MediaType}\". {NewerVersionMightBeRequired}");
            }

            // Ignore layers we don't recognize for now.
            var expectedLayerMediaType = BicepMediaTypes.BicepModuleLayerV1Json;
            var mainLayers = this.Layers.Where(l => l.MediaType.Equals(expectedLayerMediaType, MediaTypeComparison)).ToArray();

            this.mainLayer = mainLayers.Length switch
            {
                0 => throw new InvalidArtifactException($"Expected to find a layer with media type {expectedLayerMediaType}, but found none.", InvalidArtifactExceptionKind.UnknownLayerMediaType),
                1 => mainLayers.Single(),
                _ => throw new InvalidArtifactException($"Did not expect to find multiple layer media types of {string.Join(", ", mainLayers.Select(l => l.MediaType).Order().Distinct())}", InvalidArtifactExceptionKind.UnknownLayerMediaType)
            };
        }

        public override OciArtifactLayer GetMainLayer() => this.mainLayer;
    }
}
