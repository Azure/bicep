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

        public OciModuleArtifactResult(BinaryData manifestBits, string manifestDigest, IEnumerable<OciArtifactLayer> layers) : base(manifestBits, manifestDigest, layers)
        {
            // Must have a single layer with mediaType "application/vnd.ms.bicep.module.layer.v1+json"
            var expectedMediaType = BicepModuleMediaTypes.BicepModuleLayerV1Json;
            // Ignore layers we don't recognize for now.
            var bicepModuleLayerV1JsonCount = layers.Count(l => BicepMediaTypes.MediaTypeComparer.Equals(l.MediaType, expectedMediaType));
            if (bicepModuleLayerV1JsonCount != 1)
            {
                throw new InvalidModuleException($"Expected a single layer with mediaType \"{expectedMediaType}\", but found {filtered.Count()}");
            }
        }
    }
}