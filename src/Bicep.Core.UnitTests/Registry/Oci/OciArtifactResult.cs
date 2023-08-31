// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bicep.Core.UnitTests.Registry.Oci
{
    /// <summary>
    /// Tests related to deserialization of OciArtifactResult class
    /// </summary>
    [TestClass]
    public class OciManifestTests
    {
        [TestMethod]
        [Description("Verify that a valid OCI manifest can be deserialized from a valid JSON")]
        public void FromBinaryData_ValidInput_DeserializesSuccessfully()
        {
            const string jsonManifest = """
{
  "schemaVersion": 2,
  "mediaType": "application/vnd.oci.image.manifest.v1+json",
  "artifactType": "application/vnd.ms.bicep.provider.artifact",
  "config": {
    "mediaType": "application/vnd.ms.bicep.provider.config.v1+json",
    "digest": "sha256:44136fa355b3678a1146ad16f7e8649e94fb4fc21fe77e8310c060f61caaff8a",
    "size": 2
  },
  "layers": [
    {
      "mediaType": "application/vnd.ms.bicep.provider.layer.v1.tar+gzip",
      "digest": "sha256:f1f31b47972a60b1905f3252a4819dad964d505cf530a9cd3d67cee8a3c295f5",
      "size": 9346232
    }
  ],
  "annotations": {
    "bicep.serialization.format": "v1",
    "org.opencontainers.image.created": "2023-05-04T16:40:05Z"
  }
}
""";
            var binaryData = new BinaryData(Encoding.UTF8.GetBytes(jsonManifest));

            var got = OciManifest.FromBinaryData(binaryData).Should().BeOfType<OciManifest>().Subject;

            got.SchemaVersion.Should().Be(2);
            got.ArtifactType.Should().Be(BicepMediaTypes.BicepProviderArtifactType);
            got.Config.MediaType.Should().Be(BicepMediaTypes.BicepProviderConfigV1);
            got.Config.Digest.Should().Be("sha256:44136fa355b3678a1146ad16f7e8649e94fb4fc21fe77e8310c060f61caaff8a");
            got.Config.Size.Should().Be(2);
            got.Layers.Should().HaveCount(1);
            got.Layers[0].MediaType.Should().Be(BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip);
            got.Annotations.Should().HaveCount(2);
        }
    }
}
