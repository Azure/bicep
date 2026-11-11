// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.RegistryClients.Oci;

namespace Bicep.Core.Artifacts.Codecs;

/// <summary>
/// Converts an extension package to and from an OCI payload: a config, the types layer and one layer per binary.
/// </summary>
public static class OciExtensionArtifactCodec
{
    private const StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;

    public static OciArtifactPayload Encode(ExtensionArtifactPackage package)
    {
        OciExtensionV1Config config = package.Binaries.Any()
            ? new(localDeployEnabled: true, supportedArchitectures: [.. package.Binaries.Select(x => x.Architecture.Name)])
            : new(null, null);

        var layers = new List<OciBlob>
        {
            new(BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip, package.Types, new OciManifestAnnotationsBuilder().WithTitle("types.tgz").Build()),
        };

        foreach (var binary in package.Binaries)
        {
            layers.Add(new(BicepMediaTypes.GetExtensionArtifactLayerV1Binary(binary.Architecture), binary.Data, new OciManifestAnnotationsBuilder().WithTitle("extension.bin").Build()));
        }

        var annotations = new OciManifestAnnotationsBuilder()
            .WithBicepSerializationFormatV1()
            .WithCreatedTime(DateTime.Now);

        return OciArtifactPayload.Create(
            BicepMediaTypes.BicepExtensionArtifactType,
            new(BicepMediaTypes.BicepExtensionConfigV1, BinaryData.FromString(JsonSerializer.Serialize(config, OciExtensionV1ConfigSerializationContext.Default.OciExtensionV1Config))),
            layers,
            annotations.Build());
    }

    /// <summary>
    /// Decodes the payload, keeping the binary of every architecture that this Bicep version knows.
    /// </summary>
    /// <exception cref="InvalidArtifactException">The payload is not a valid Bicep extension.</exception>
    public static ExtensionArtifactPackage Decode(OciArtifactPayload payload)
    {
        var manifest = payload.Manifest;
        if (manifest.ArtifactType is null || !manifest.ArtifactType.Equals(BicepMediaTypes.BicepExtensionArtifactType, MediaTypeComparison))
        {
            throw new InvalidArtifactException($"Expected an extension, but the artifact type is \"{manifest.ArtifactType}\".", InvalidArtifactExceptionKind.WrongArtifactType);
        }

        if (!manifest.Config.MediaType.Equals(BicepMediaTypes.BicepExtensionConfigV1, MediaTypeComparison))
        {
            throw new InvalidArtifactException(
                $"The extension's config media type \"{manifest.Config.MediaType}\" is not supported by this version of Bicep. {OciModuleArtifactCodec.NewerVersionMightBeRequired}",
                InvalidArtifactExceptionKind.WrongArtifactType);
        }

        var expectedLayerMediaType = BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip;
        var typesLayers = manifest.Layers.Where(l => l.MediaType.Equals(expectedLayerMediaType, MediaTypeComparison)).ToArray();
        var typesLayer = typesLayers.Length switch
        {
            0 => throw new InvalidArtifactException($"The extension has no types layer (media type \"{expectedLayerMediaType}\").", InvalidArtifactExceptionKind.UnknownLayerMediaType),
            1 => typesLayers[0],
            _ => throw new InvalidArtifactException($"The extension has more than one types layer (media type \"{expectedLayerMediaType}\").", InvalidArtifactExceptionKind.UnknownLayerMediaType),
        };

        var config = payload.Config is { } configData ? DeserializeConfig(configData) : null;

        var binaries = ImmutableArray.CreateBuilder<ExtensionBinary>();
        if (config?.LocalDeployEnabled == true && config.SupportedArchitectures is { } architectureNames)
        {
            foreach (var architecture in SupportedArchitectures.All)
            {
                if (architectureNames.Contains(architecture.Name) &&
                    payload.TryGetLayer(BicepMediaTypes.GetExtensionArtifactLayerV1Binary(architecture)) is { } binary)
                {
                    binaries.Add(new(architecture, binary));
                }
            }
        }

        return new(payload.GetLayer(typesLayer), binaries.ToImmutable());
    }

    private static OciExtensionV1Config? DeserializeConfig(BinaryData configData)
    {
        try
        {
            return JsonSerializer.Deserialize(configData, OciExtensionV1ConfigSerializationContext.Default.OciExtensionV1Config);
        }
        catch (JsonException exception)
        {
            throw new InvalidArtifactException("The extension's config is not valid JSON.", exception, InvalidArtifactExceptionKind.InvalidArtifactContents);
        }
    }
}
