// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class MockRegistryBlobClientExtensions
    {
        public static MockRegistryAssertions Should(this MockRegistryBlobClient client) => new MockRegistryAssertions(client);
    }

    public class MockRegistryAssertions : ReferenceTypeAssertions<MockRegistryBlobClient, MockRegistryAssertions>
    {
        public MockRegistryAssertions(MockRegistryBlobClient client)
            : base(client)
        {
        }

        protected override string Identifier => nameof(MockRegistryBlobClient);

        public AndConstraint<MockRegistryAssertions> HaveModule(string tag, Stream expectedMainJsonContent)
        {
            using (new AssertionScope())
            {
                this.Subject.ManifestTags.Should().ContainKey(tag, $"tag '{tag}' should exist");

                string manifestDigest = this.Subject.ManifestTags[tag];

                this.Subject.Manifests.Should().ContainKey(manifestDigest, $"tag '{tag}' resolves to digest '{manifestDigest}' that should exist");

                string digest = this.Subject.ManifestTags[tag];
                this.Subject.Manifests.Should().ContainKey(digest, $"tag '{tag}' resolves to digest '{digest}' that should exist");
                if (!this.Subject.Manifests.ContainsKey(digest))
                {
                    // Avoid null reference exceptions because they will block the assertion failure message
                    return new(this);
                }

                var manifest = this.Subject.ManifestObjects[digest];
                manifest.Should().NotBeNull();
                manifest.MediaType.Should().BeNull("module media type should be correct");
                manifest.ArtifactType.Should().Be("application/vnd.ms.bicep.module.artifact", "module artifact type should be correct");

                var config = manifest.Config;
                config.MediaType.Should().Be("application/vnd.ms.bicep.module.config.v1+json", "config media type should be correct");
                config.Size.Should().Be(0, "module config size should be zero");

                this.Subject.Blobs.Should().ContainKey(config.Digest, "module config digest should exist");
                if (this.Subject.Blobs.ContainsKey(config.Digest))
                {
                    var configBytes = this.Subject.Blobs[config.Digest];
                    configBytes.Bytes.Should().BeEmpty("module config blob should be empty");
                }

                manifest.Layers.Should().HaveCount(1, "modules should have a single layer");
                if (manifest.Layers.Count() == 1)
                {
                    var layer = manifest.Layers.Single();

                    layer.MediaType.Should().Be("application/vnd.ms.bicep.module.layer.v1+json", "module layer media type should be correct");
                    this.Subject.Blobs.Should().ContainKey(layer.Digest, "layer blob should exist");

                    var layerBytes = this.Subject.Blobs[layer.Digest];
                    layerBytes.Should().NotBeNull();
                    ((long)layerBytes.Bytes.Length).Should().Be(layer.Size, "module layer blob should match size");
                    var actualMainJsonToken = layerBytes.ToStream().FromJsonStream<JToken>();

                    expectedMainJsonContent.Position = 0;
                    var expectedToken = expectedMainJsonContent.FromJsonStream<JToken>();

                    actualMainJsonToken.Should().DeepEqual(expectedToken, "module main.json content should match");
                }
            }

            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> OnlyHaveModule(string tag, Stream expectedMainJsonContent)
        {
            using (new AssertionScope())
            {
                this.Subject.Should().HaveModule(tag, expectedMainJsonContent);

                // we should only have an empty config blob and the module layer blob
                this.Subject.Blobs.Should().HaveCount(2);

                // there should be one manifest for one module
                this.Subject.Manifests.Should().HaveCount(1);

                // there should only be one tag
                this.Subject.ManifestTags.Should().HaveCount(1);
            }

            return new(this);
        }
    }
}
