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

        public AndConstraint<MockRegistryAssertions> HaveModuleWithoutSource(string digest, Stream expectedMainJsonContent)
        {
            this.Subject.Should().HaveModule(digest, expectedMainJsonContent);
            this.Subject.Should().NotHaveAnyAttachments(digest);
            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> HaveModuleWithSource(string digest, Stream expectedMainJsonContent, byte[]? expectedSourceContents = null)
        {
            this.Subject.Should().HaveModule(digest, expectedMainJsonContent);
            this.Subject.Should().HaveSourceAttachedToModule(digest, expectedSourceContents);
            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> HaveModule(string tag, Stream expectedMainJsonContent)
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
            manifest.MediaType.Should().BeNull("media type should be correct");
            manifest.ArtifactType.Should().Be("application/vnd.ms.bicep.module.artifact", "artifact type should be correct");

            var config = manifest.Config;
            config.MediaType.Should().Be("application/vnd.ms.bicep.module.config.v1+json", "config media type should be correct");
            config.Size.Should().Be(0, "config size should be empty");

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

                layer.MediaType.Should().Be("application/vnd.ms.bicep.module.layer.v1+json", "layer media type should be correct");
                this.Subject.Blobs.Should().ContainKey(layer.Digest, "layer blob should exist");

                var layerBytes = this.Subject.Blobs[layer.Digest];
                ((long)layerBytes.Bytes.Length).Should().Be(layer.Size, "layer blob should match size");

                var actualMainJsonStream = layerBytes.ToStream().FromJsonStream<JToken>();
                expectedMainJsonContent.Position = 0;
                var expectedMainJsonStream = expectedMainJsonContent.FromJsonStream<JToken>();
                expectedMainJsonStream.Should().NotBeNull();

                actualMainJsonStream.Should().DeepEqual(expectedMainJsonStream, "module content should match");
            }

            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> OnlyHaveModule(string tag, Stream expectedMainJsonContent)
        {
            this.Subject.Should().HaveModule(tag, expectedMainJsonContent);

            // we should only have an empty config blob and the main.json for modules and a config
            //   and source contents for the module source
            var expectedBlobs = this.Subject.SourceManifestObjects.Any() ? 4 : 2;
            this.Subject.Blobs.Should().HaveCount(expectedBlobs);

            // there should be one manifest for one module
            this.Subject.ModuleManifestObjects.Should().HaveCount(1);
            this.Subject.SourceManifestObjects.Should().HaveCountLessThanOrEqualTo(1);

            // there should only be one tag
            this.Subject.ManifestTags.Should().HaveCount(1);

            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> HaveSourceAttachedToModule(string moduleTag, byte[]? expectedSourceContents = null)
        {
            this.Subject.ManifestTags.Should().ContainKey(moduleTag, $"tag '{moduleTag}' should exist");
            string moduleManifestDigest = this.Subject.ManifestTags[moduleTag];
            this.Subject.Manifests.Should().ContainKey(moduleManifestDigest);

            var referrers = this.Subject.ManifestObjects.Where(m => m.Value.Subject?.Digest == moduleManifestDigest);
            var sourceReferrers = referrers.Where(m => m.Value.ArtifactType == "application/vnd.ms.bicep.module.source").ToArray();
            sourceReferrers.Count().Should().BeLessThanOrEqualTo(1, "there should only be a single attached sources manifest");
            sourceReferrers.Should().HaveCount(1, "there should be a source manifest pointing to the module manifest");

            if (sourceReferrers.Count() == 1)
            {
                var manifest = sourceReferrers.Single().Value;

                var config = manifest.Config;
                config.MediaType.Should().Be("application/vnd.ms.bicep.module.source.config.v1+json", "source config media type should be correct");
                config.Size.Should().NotBe(0, "source config size should be correct"); // len("{}") == 2

                this.Subject.Blobs.Should().ContainKey(config.Digest, "source module config digest should exist");
                if (this.Subject.Blobs.ContainsKey(config.Digest))
                {
                    var configContent = new TextBytes(this.Subject.Blobs[config.Digest].Bytes).Text;
                    configContent.Should().Be("{}", "module config blob should be empty JSON object");
                }

                manifest.MediaType.Should().BeNull("Source manifest media type should be correct");
                manifest.ArtifactType.Should().Be("application/vnd.ms.bicep.module.source", "Source manifest artifact type should be correct");

                manifest.Layers.Should().HaveCount(1, "Source manifest should have a single layer");
                if (manifest.Layers.Count() == 1)
                {
                    var layer = manifest.Layers.Single();

                    layer.MediaType.Should().Be("application/vnd.ms.bicep.module.source.v1+zip", "Source manifest layer media type should be correct");

                    this.Subject.Blobs.Should().ContainKey(layer.Digest, "source blob should exist");
                    var sourcesBlob = this.Subject.Blobs[layer.Digest];

                    var sourcesBytes = sourcesBlob.ToArray();
                    if (expectedSourceContents != null)
                    {
                        sourcesBytes.Should().BeEquivalentTo(expectedSourceContents, "source blob contents should be as expected");
                    }

                    layer.Size.Should().Be(sourcesBytes.Length, "Source blob length should be correct in the source manifest");
                }
            }

            return new(this);
        }

        public AndConstraint<MockRegistryAssertions> NotHaveAnyAttachments(string moduleTag)
        {
            this.Subject.ManifestTags.Should().ContainKey(moduleTag, $"tag '{moduleTag}' should exist");
            string moduleManifestDigest = this.Subject.ManifestTags[moduleTag];
            this.Subject.Manifests.Should().ContainKey(moduleManifestDigest);

            var referrers = this.Subject.ManifestObjects.Where(m => m.Value.Subject?.Digest == moduleManifestDigest);
            referrers.Should().HaveCount(0, $"Expecting no referrers, but found {referrers.Count()}");

            return new(this);
        }
    }
}
