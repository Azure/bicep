// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Modules;

namespace Bicep.Core.Registry.Oci;

/// <summary>
/// Builds the right concrete <see cref="OciArtifactResult"/> subclass given a manifest and materialized layers.
/// </summary>
internal static class OciArtifactResultFactory
{
    public static OciArtifactResult Create(OciManifest manifest, BinaryData manifestData, string manifestDigest, ImmutableArray<OciArtifactLayer> layers, OciArtifactLayer? configLayer)
    {
        return manifest.ArtifactType switch
        {
            BicepMediaTypes.BicepExtensionArtifactType => new OciExtensionArtifactResult(manifestData, manifestDigest, layers, configLayer),
            BicepMediaTypes.BicepModuleArtifactType or null => new OciModuleArtifactResult(manifestData, manifestDigest, layers),
            _ => throw new InvalidArtifactException($"artifacts of type: '{manifest.ArtifactType}' are not supported by this Bicep version."),
        };
    }
}
