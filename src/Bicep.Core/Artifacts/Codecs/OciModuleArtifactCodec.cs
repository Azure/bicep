// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Packages;
using Bicep.Core.RegistryClients.Oci;
using Bicep.Core.Semantics;

namespace Bicep.Core.Artifacts.Codecs;

/// <summary>
/// Converts a module package to and from an OCI payload: a config, the template layer, an optional sources layer
/// and the manifest annotations.
/// </summary>
public static class OciModuleArtifactCodec
{
    public const string NewerVersionMightBeRequired = "A newer version of Bicep might be required to reference this artifact.";

    private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;

    public static OciArtifactPayload Encode(ModuleArtifactPackage package)
    {
        var layers = new List<OciBlob>
        {
            new(BicepMediaTypes.BicepModuleLayerV1Json, package.Template, new OciManifestAnnotationsBuilder().WithTitle("Compiled ARM template").Build()),
        };

        if (package.Sources is { } sources)
        {
            layers.Add(new(BicepMediaTypes.BicepSourceV1Layer, sources, new OciManifestAnnotationsBuilder().WithTitle("Source files").Build()));
        }

        var annotations = new OciManifestAnnotationsBuilder()
            .WithDescription(package.Description)
            .WithDocumentationUri(package.DocumentationUri)
            .WithCreatedTime(DateTime.Now);

        return OciArtifactPayload.Create(
            BicepMediaTypes.BicepModuleArtifactType,
            new(BicepMediaTypes.BicepModuleConfigV1, BinaryData.FromString("{}")),
            layers,
            annotations.Build());
    }

    /// <exception cref="InvalidArtifactException">The payload is not a valid Bicep module.</exception>
    public static ModuleArtifactPackage Decode(OciArtifactPayload payload)
    {
        var manifest = payload.Manifest;
        if (manifest.ArtifactType is not null && !manifest.ArtifactType.Equals(BicepMediaTypes.BicepModuleArtifactType, MediaTypeComparison))
        {
            throw new InvalidArtifactException(
               $"Expected a module, but the artifact type is \"{manifest.ArtifactType}\".",
               InvalidArtifactExceptionKind.WrongArtifactType);
        }

        if (manifest.Config.MediaType is not null && !manifest.Config.MediaType.Equals(BicepMediaTypes.BicepModuleConfigV1, MediaTypeComparison))
        {
            throw new InvalidArtifactException($"The module's config media type \"{manifest.Config.MediaType}\" is not supported by this version of Bicep. {NewerVersionMightBeRequired}");
        }

        // Ignore layers we don't recognize for now.
        var expectedLayerMediaType = BicepMediaTypes.BicepModuleLayerV1Json;
        var mainLayers = manifest.Layers.Where(l => l.MediaType.Equals(expectedLayerMediaType, MediaTypeComparison)).ToArray();

        var mainLayer = mainLayers.Length switch
        {
            0 => throw new InvalidArtifactException($"The module has no compiled template layer (media type \"{expectedLayerMediaType}\").", InvalidArtifactExceptionKind.UnknownLayerMediaType),
            1 => mainLayers.Single(),
            _ => throw new InvalidArtifactException($"The module has more than one compiled template layer (media type \"{expectedLayerMediaType}\").", InvalidArtifactExceptionKind.UnknownLayerMediaType)
        };

        string? documentationUri = null;
        manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation, out documentationUri);

        return new(
            Template: payload.GetLayer(mainLayer),
            Sources: payload.TryGetLayer(BicepMediaTypes.BicepSourceV1Layer),
            DocumentationUri: documentationUri,
            Description: DescriptionHelper.TryGetFromOciManifestAnnotations(manifest.Annotations));
    }
}
