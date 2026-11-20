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
public class OciModuleArtifactCodecTests
{
    [TestMethod]
    [DataRow(false, null, null)]
    [DataRow(true, null, null)]
    [DataRow(true, "https://docs", "My module")]
    public void Decode_ReturnsTheEncodedPackage(bool withSources, string? documentationUri, string? description)
    {
        var package = new ModuleArtifactPackage(
            BinaryData.FromString("""{ "resources": {} }"""),
            withSources ? BinaryData.FromString("sources") : null,
            documentationUri,
            description);

        var payload = OciModuleArtifactCodec.Encode(package);
        var decoded = OciModuleArtifactCodec.Decode(AsPulled(payload));

        payload.Manifest.ArtifactType.Should().Be(BicepMediaTypes.BicepModuleArtifactType);
        decoded.Template.ToString().Should().Be(package.Template.ToString());
        decoded.Sources?.ToString().Should().Be(package.Sources?.ToString());
        decoded.DocumentationUri.Should().Be(documentationUri);
        decoded.Description.Should().Be(description);
    }

    [TestMethod]
    public void FromTemplate_TakesTheDescriptionFromTheTemplate()
    {
        var package = ModuleArtifactPackage.FromTemplate(BinaryData.FromString("""{ "metadata": { "description": "My module" } }"""), null, null);

        package.Description.Should().Be("My module");
    }

    [TestMethod]
    public void Decode_RejectsAPayloadWithoutATemplateLayer()
    {
        var payload = OciArtifactPayload.Create(
            BicepMediaTypes.BicepModuleArtifactType,
            new(BicepMediaTypes.BicepModuleConfigV1, BinaryData.FromString("{}")),
            [new("application/unknown", BinaryData.FromString("{}"))],
            []);

        var decode = () => OciModuleArtifactCodec.Decode(payload);

        decode.Should().Throw<InvalidArtifactException>().WithMessage(
            "The OCI artifact is not a valid Bicep artifact. The module has no compiled template layer (media type \"application/vnd.ms.bicep.module.layer.v1+json\").");
    }

    // A pulled payload has no blob for an empty config, because the transports don't download it.
    internal static OciArtifactPayload AsPulled(OciArtifactPayload payload) =>
        new(payload.ManifestData, payload.Manifest.Config.IsEmpty() ? payload.BlobsByDigest.Remove(payload.Manifest.Config.Digest) : payload.BlobsByDigest);
}
