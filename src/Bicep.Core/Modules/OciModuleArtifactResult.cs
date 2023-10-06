// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


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
            if (manifest.ArtifactType is not null && !manifest.ArtifactType.Equals(BicepModuleMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
            {
                throw new InvalidArtifactException(
                   $"Expected OCI manifest artifactType value of '{BicepModuleMediaTypes.BicepModuleArtifactType}' but found '{manifest.ArtifactType}'. {NewerVersionMightBeRequired}",
                   InvalidArtifactExceptionKind.WrongArtifactType);
            }
            if (manifest.Config.MediaType is not null && !manifest.Config.MediaType.Equals(BicepModuleMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
            {
                throw new InvalidArtifactException($"Did not expect config media type \"{manifest.Config.MediaType}\". {NewerVersionMightBeRequired}");
            }

            // Ignore layers we don't recognize for now.
            var expectedLayerMediaType = BicepModuleMediaTypes.BicepModuleLayerV1Json;
            var mainLayers = this.Layers.Where(l => l.MediaType.Equals(expectedLayerMediaType, MediaTypeComparison)).ToArray();
            if (mainLayers.Length == 0)
            {
                var gotMediaTypes = string.Join(", ", manifest.Layers.Select(l => l.MediaType).Order().ToArray());
                throw new InvalidArtifactException($"Expected to find a layer with media type {expectedLayerMediaType}, but found only layers of types {string.Join(", ", manifest.Layers.Select(l => l.MediaType).ToArray())}", InvalidArtifactExceptionKind.WrongModuleLayerMediaType);
            }
            else if (mainLayers.Length > 1)
            {
                throw new InvalidArtifactException($"Did not expect to find multiple layer media types of {string.Join(", ", mainLayers.Select(l => l.MediaType).ToArray())}", InvalidArtifactExceptionKind.WrongModuleLayerMediaType);
            }
            this.mainLayer = mainLayers.Single();
        }

        public override OciArtifactLayer GetMainLayer() => this.mainLayer;
    }
}