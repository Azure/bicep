// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.RegistryClients.Oci;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Artifacts.Codecs;

[TestClass]
public class OciExtensionArtifactCodecTests
{
    [TestMethod]
    public void Decode_ReturnsTheEncodedPackage_WithoutBinaries()
    {
        var package = new ExtensionArtifactPackage(BinaryData.FromString("types"), []);

        var payload = OciExtensionArtifactCodec.Encode(package);
        var decoded = OciExtensionArtifactCodec.Decode(OciModuleArtifactCodecTests.AsPulled(payload));

        payload.Manifest.Layers.Should().ContainSingle();
        decoded.Types.ToString().Should().Be("types");
        decoded.Binaries.Should().BeEmpty();
    }

    [TestMethod]
    public void Decode_ReturnsTheEncodedPackage_WithEveryBinary()
    {
        var package = new ExtensionArtifactPackage(
            BinaryData.FromString("types"),
            [.. SupportedArchitectures.All.Select(architecture => new ExtensionBinary(architecture, BinaryData.FromString(architecture.Name)))]);

        var payload = OciExtensionArtifactCodec.Encode(package);
        var decoded = OciExtensionArtifactCodec.Decode(OciModuleArtifactCodecTests.AsPulled(payload));

        decoded.Types.ToString().Should().Be("types");
        decoded.Binaries.Select(binary => (binary.Architecture, binary.Data.ToString()))
            .Should().Equal(package.Binaries.Select(binary => (binary.Architecture, binary.Data.ToString())));
    }

    [TestMethod]
    public void Decode_RejectsAModule()
    {
        var payload = OciModuleArtifactCodec.Encode(new(BinaryData.FromString("{}"), null, null));

        var decode = () => OciExtensionArtifactCodec.Decode(payload);

        decode.Should().Throw<InvalidArtifactException>().WithMessage(
            "The OCI artifact is not a valid Bicep artifact. Expected an extension, but the artifact type is \"application/vnd.ms.bicep.module.artifact\".");
    }

    [TestMethod]
    public void Decode_RejectsMoreThanOneTypesLayer()
    {
        var types = new OciBlob(BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip, BinaryData.FromString("types"));
        var payload = CreatePayload(BinaryData.FromString("{}"), [types, types]);

        var decode = () => OciExtensionArtifactCodec.Decode(payload);

        decode.Should().Throw<InvalidArtifactException>().WithMessage(
            "The OCI artifact is not a valid Bicep artifact. The extension has more than one types layer (media type \"application/vnd.ms.bicep.provider.layer.v1.tar+gzip\").");
    }

    [TestMethod]
    public void Decode_RejectsAConfigThatIsNotJson()
    {
        var payload = CreatePayload(BinaryData.FromString("not json"), [new(BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip, BinaryData.FromString("types"))]);

        var decode = () => OciExtensionArtifactCodec.Decode(payload);

        decode.Should().Throw<InvalidArtifactException>().WithMessage("The OCI artifact is not a valid Bicep artifact. The extension's config is not valid JSON.");
    }

    private static OciArtifactPayload CreatePayload(BinaryData config, IEnumerable<OciBlob> layers) =>
        OciArtifactPayload.Create(BicepMediaTypes.BicepExtensionArtifactType, new(BicepMediaTypes.BicepExtensionConfigV1, config), layers, []);
}
