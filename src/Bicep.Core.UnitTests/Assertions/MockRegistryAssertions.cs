// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Json;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class MockRegistryBlobClientExtensions
    {
        public static MockRegistryAssertions Should(this FakeRegistryBlobClient client) => new(client);
    }

    public class MockRegistryAssertions : ReferenceTypeAssertions<FakeRegistryBlobClient, MockRegistryAssertions>
    {
        public MockRegistryAssertions(FakeRegistryBlobClient client)
            : base(client)
        {
        }

        protected override string Identifier => nameof(FakeRegistryBlobClient);

        public AndConstraint<MockRegistryAssertions> HaveModuleWithSource(string tag, BinaryData expectedModuleContent, BinaryData? expectedSourceContent = null)
        {
            return this.HaveModuleCore(tag, expectedModuleContent, validateSource: true, expectingToHaveSource: true, expectedSourceContent: expectedSourceContent);
        }

        public AndConstraint<MockRegistryAssertions> HaveModuleWithNoSource(string tag, BinaryData expectedModuleContent)
        {
            return this.HaveModuleCore(tag, expectedModuleContent, validateSource: true, expectingToHaveSource: false, expectedSourceContent: null);
        }

        public AndConstraint<MockRegistryAssertions> HaveModule(string tag, BinaryData expectedModuleContent)
        {
            return this.HaveModuleCore(tag, expectedModuleContent, validateSource: false, expectingToHaveSource: false, expectedSourceContent: null);
        }

        private AndConstraint<MockRegistryAssertions> HaveModuleCore(string tag, BinaryData expectedModuleContent, bool validateSource, bool expectingToHaveSource, BinaryData? expectedSourceContent)
        {
            using (new AssertionScope())
            {
                this.Subject.ManifestTags.Should().ContainKey(tag, $"tag '{tag}' should exist");
                string digest = this.Subject.ManifestTags[tag];
                this.Subject.Manifests.Should().ContainKey(digest, $"tag '{tag}' resolves to digest '{digest}' that should exist");
                if (!this.Subject.Manifests.ContainsKey(digest))
                {
                    return new(this);
                }

                var manifest = this.Subject.ManifestObjects[digest];
                manifest.Should().NotBeNull();
                manifest.MediaType.Should().Be("application/vnd.oci.image.manifest.v1+json", "media type should be explicit for new versions of Bicep, and not null");
                manifest.ArtifactType.Should().Be("application/vnd.ms.bicep.module.artifact", "artifact type should be correct");

                var config = manifest.Config;
                config.MediaType.Should().Be("application/vnd.ms.bicep.module.config.v1+json", "config media type should be correct");

                this.Subject.Blobs.Should().ContainKey(config.Digest, "module config digest should exist");
                if (this.Subject.Blobs.ContainsKey(config.Digest))
                {
                    var configBytes = this.Subject.Blobs[config.Digest];
                    config.Size.Should().Be(configBytes.ToArray().Length, "Config size field should match config size");
                    JsonElement? configJson = null;
                    var convertToJson = () => configJson = JsonElementFactory.CreateElement(configBytes.ToString());
                    convertToJson.Should().NotThrow("Config should be a valid JSON object");
                    configJson!.Value.ValueKind.Should().Be(JsonValueKind.Object, "Config should be a JSON object");
                }

                manifest.Layers.Should().HaveCountLessThanOrEqualTo(2, "modules should have one or two layers");
                if (manifest.Layers.Count() <= 2)
                {
                    // main.json
                    var layer1 = manifest.Layers.First();

                    layer1.MediaType.Should().Be("application/vnd.ms.bicep.module.layer.v1+json", "layer media type should be correct");
                    this.Subject.Blobs.Should().ContainKey(layer1.Digest, "layer blob should exist");

                    var layerBytes = this.Subject.Blobs[layer1.Digest];
                    ((long)layerBytes.ToArray().Length).Should().Be(layer1.Size, "layer blob should match size");

                    var actualMainJsonStream = layerBytes.ToStream().FromJsonStream<JToken>();
                    var expectedMainJsonStream = expectedModuleContent.ToStream().FromJsonStream<JToken>();
                    expectedMainJsonStream.Should().NotBeNull();

                    actualMainJsonStream.Should().DeepEqual(expectedMainJsonStream, "module content should match");

                    // sources
                    if (validateSource)
                    {
                        if (expectingToHaveSource)
                        {
                            manifest.Layers.Should().HaveCount(2, "modules with source should have two layers");
                            if (manifest.Layers.Count() == 2)
                            {
                                var layer2 = manifest.Layers.Skip(1).Single();

                                layer2.MediaType.Should().Be("application/vnd.ms.bicep.module.source.v1.tar+gzip", "source layer media type should be correct");
                                this.Subject.Blobs.Should().ContainKey(layer2.Digest, "source layer blob should exist");

                                var layer2Bytes = this.Subject.Blobs[layer2.Digest].ToArray();
                                ((long)layer2Bytes.Length).Should().Be(layer2.Size, "source ayer blob should match size");

                                if (expectedSourceContent is { })
                                {
                                    var actualSourcesBytes = layer2Bytes!;
                                    actualSourcesBytes.Should().Equal(expectedSourceContent.ToArray(), "module sources should match");
                                }
                            }
                        }
                        else
                        {
                            manifest.Layers.Should().HaveCount(1, "modules with no source should have only one layer");
                        }
                    }
                }
            }

            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> HaveExtension(string tag, out Stream tgzStream)
        {
            this.Subject.ManifestTags.Should().ContainKey(tag, $"tag '{tag}' should exist");
            string digest = this.Subject.ManifestTags[tag];
            this.Subject.Manifests.Should().ContainKey(digest, $"tag '{tag}' resolves to digest '{digest}' that should exist");

            var manifest = this.Subject.ManifestObjects[digest];
            manifest.Should().NotBeNull();
            manifest.MediaType.Should().Be("application/vnd.oci.image.manifest.v1+json", "media type should be explicit for new versions of Bicep, and not null");
            manifest.ArtifactType.Should().Be(BicepMediaTypes.BicepExtensionArtifactType, "artifact type should be correct");

            var config = manifest.Config;
            config.MediaType.Should().Be(BicepMediaTypes.BicepExtensionConfigV1, "config media type should be correct");

            this.Subject.Blobs.Should().ContainKey(config.Digest, "config digest should exist");
            var configBytes = this.Subject.Blobs[config.Digest];
            config.Size.Should().Be(configBytes.ToArray().Length, "Config size field should match config size");
            JsonElement? configJson = null;
            var convertToJson = () => configJson = JsonElementFactory.CreateElement(configBytes.ToString());
            convertToJson.Should().NotThrow("Config should be a valid JSON object");
            configJson!.Value.ValueKind.Should().Be(JsonValueKind.Object, "Config should be a JSON object");

            manifest.Layers.Should().HaveCount(1);

            // types.tgz
            var tgzLayer = manifest.Layers.First();

            tgzLayer.MediaType.Should().Be(BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip, "layer media type should be correct");
            this.Subject.Blobs.Should().ContainKey(tgzLayer.Digest, "layer blob should exist");

            var layerBytes = this.Subject.Blobs[tgzLayer.Digest];
            ((long)layerBytes.ToArray().Length).Should().Be(tgzLayer.Size, "layer blob should match size");

            tgzStream = layerBytes.ToStream();
            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> OnlyHaveModule(string tag, BinaryData expectedMainJsonContent)
        {
            using (new AssertionScope())
            {
                this.Subject.Should().HaveModule(tag, expectedMainJsonContent);

                // there should only be one tag
                this.Subject.ManifestTags.Should().HaveCount(1);

                // there should be one manifest for one module
                this.Subject.ModuleManifestObjects.Should().HaveCount(1);

                // we should only have an empty config blob and a blob for each layer
                this.Subject.Blobs.Should().HaveCount(this.Subject.ModuleManifestObjects.Single().Value.Layers.Count() + 1);
            }
            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> OnlyHaveReachableModule(string tag, BinaryData expectedMainJsonContent)
        {
            using (new AssertionScope())
            {
                this.Subject.Should().HaveModule(tag, expectedMainJsonContent);

                // we should only have one reachable tag
                this.Subject.ManifestTags.Should().HaveCount(1);
            }
            return new(this);
        }
    }
}
