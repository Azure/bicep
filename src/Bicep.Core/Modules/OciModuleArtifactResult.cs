// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Bicep.Core.Modules
{
    public class OciModuleArtifactResult : OciArtifactResult
    {
        private readonly OciArtifactLayer mainLayer;
        public const string NewerVersionMightBeRequired = "A newer version of Bicep might be required to reference this artifact.";
        public OciModuleArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers, OciArtifactLayer? config = null) :
            base(manifestBits, manifestDigest, layers)
        {
            var manifest = this.Manifest;
            if (manifest.ArtifactType is not null && !manifest.ArtifactType.Equals(BicepMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
            {
                throw new InvalidArtifactException(
                   $"Expected OCI manifest artifactType value of '{BicepMediaTypes.BicepModuleArtifactType}' but found '{manifest.ArtifactType}'. {NewerVersionMightBeRequired}",
                   InvalidArtifactExceptionKind.WrongArtifactType);
            }

            this.IsLayered = manifest.Config.MediaType is { } configMediaType &&
                configMediaType.Equals(BicepMediaTypes.BicepModuleConfigV2, MediaTypeComparison);

            if (manifest.Config.MediaType is not null &&
                !this.IsLayered &&
                !manifest.Config.MediaType.Equals(BicepMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
            {
                throw new InvalidArtifactException($"Did not expect config media type \"{manifest.Config.MediaType}\". {NewerVersionMightBeRequired}");
            }

            // Ignore layers we don't recognize for now.
            var expectedLayerMediaType = BicepMediaTypes.BicepModuleLayerV1Json;
            var mainLayers = this.Layers.Where(l => l.MediaType.Equals(expectedLayerMediaType, MediaTypeComparison)).ToArray();

            if (this.IsLayered)
            {
                this.Config = config;
                this.mainLayer = ResolveEntryPointLayer(mainLayers, config);

                return;
            }

            this.mainLayer = mainLayers.Length switch
            {
                0 => throw new InvalidArtifactException($"Expected to find a layer with media type {expectedLayerMediaType}, but found none.", InvalidArtifactExceptionKind.UnknownLayerMediaType),
                1 => mainLayers.Single(),
                _ => throw new InvalidArtifactException($"Did not expect to find multiple layer media types of {string.Join(", ", mainLayers.Select(l => l.MediaType).Order().Distinct())}", InvalidArtifactExceptionKind.UnknownLayerMediaType)
            };
        }

        /// <summary>
        /// True if the artifact uses a layered manifest, i.e. the entry point and its nested
        /// deployment templates are stored as separate layers linked by digest.
        /// </summary>
        public bool IsLayered { get; }

        public OciArtifactLayer? Config { get; }

        public override OciArtifactLayer GetMainLayer() => this.mainLayer;

        private static OciArtifactLayer ResolveEntryPointLayer(IReadOnlyList<OciArtifactLayer> layers, OciArtifactLayer? config)
        {
            if (config is null)
            {
                throw new InvalidArtifactException($"Expected a config blob with media type {BicepMediaTypes.BicepModuleConfigV2}, but found none.");
            }

            OciModuleV2Config? configData;
            try
            {
                configData = JsonSerializer.Deserialize(config.Data, OciModuleV2ConfigSerializationContext.Default.OciModuleV2Config);
            }
            catch (Exception exception)
            {
                throw new InvalidArtifactException($"Failed to deserialize the artifact config blob. {NewerVersionMightBeRequired}", exception);
            }

            if (configData?.EntryPointDigest is not { } entryPointDigest || string.IsNullOrWhiteSpace(entryPointDigest))
            {
                throw new InvalidArtifactException($"The artifact config blob does not specify an entry point digest. {NewerVersionMightBeRequired}");
            }

            return layers.FirstOrDefault(l => OciArtifactReferenceFacts.DigestComparer.Equals(l.Digest, entryPointDigest))
                ?? throw new InvalidArtifactException($"The artifact config blob refers to entry point digest \"{entryPointDigest}\", but no such layer exists in the manifest.");
        }
    }
}
