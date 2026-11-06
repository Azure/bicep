// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class ArtifactPublisherTests
    {
        [TestMethod]
        [DataRow(ArtifactType.Module, RegistryCapabilities.Default, false)]
        [DataRow(ArtifactType.Module, RegistryCapabilities.Publish, true)]
        [DataRow(ArtifactType.Extension, RegistryCapabilities.Default, false)]
        [DataRow(ArtifactType.Extension, RegistryCapabilities.Publish, true)]
        public void CanPublish_UsesRegistryCapabilities(ArtifactType artifactType, RegistryCapabilities capabilities, bool expected)
        {
            var reference = new MockArtifactReference(BicepTestConstants.DummyBicepFile, "target", artifactType);
            var registry = CreateRegistry();
            registry.Setup(x => x.GetCapabilities(artifactType, reference)).Returns(capabilities);

            CreatePublisher(registry.Object).CanPublish(reference).Should().Be(expected);
        }

        [TestMethod]
        public async Task Exists_UsesTheTargetArtifactType()
        {
            var moduleReference = new MockArtifactReference(BicepTestConstants.DummyBicepFile, "module", ArtifactType.Module);
            var extensionReference = new MockArtifactReference(BicepTestConstants.DummyBicepFile, "extension", ArtifactType.Extension);
            var registry = CreateRegistry();
            registry.Setup(x => x.CheckArtifactExists(ArtifactType.Module, moduleReference)).ReturnsAsync(true);
            registry.Setup(x => x.CheckArtifactExists(ArtifactType.Extension, extensionReference)).ReturnsAsync(false);

            var publisher = CreatePublisher(registry.Object);

            (await publisher.ExistsAsync(moduleReference)).Should().BeTrue();
            (await publisher.ExistsAsync(extensionReference)).Should().BeFalse();
        }

        [TestMethod]
        public async Task PublishModule_PassesTheTemplateDescriptionToTheRegistry()
        {
            var reference = new MockArtifactReference(BicepTestConstants.DummyBicepFile, "target");
            var template = BinaryData.FromString("""{ "metadata": { "description": "My module" } }""");
            var sources = BinaryData.FromString("sources");
            var registry = CreateRegistry();
            registry.Setup(x => x.PublishModule(reference, template, sources, "https://docs", "My module")).Returns(Task.CompletedTask);

            await CreatePublisher(registry.Object).PublishAsync(reference, new ModulePackage(template, sources, "https://docs"));

            registry.Verify(x => x.PublishModule(reference, template, sources, "https://docs", "My module"), Times.Once);
        }

        [TestMethod]
        public async Task PublishExtension_PassesThePackageToTheRegistry()
        {
            var reference = new MockArtifactReference(BicepTestConstants.DummyBicepFile, "target", ArtifactType.Extension);
            var package = new ExtensionPackage(BinaryData.FromString("types"), LocalDeployEnabled: false, []);
            var registry = CreateRegistry();
            registry.Setup(x => x.PublishExtension(reference, package)).Returns(Task.CompletedTask);

            await CreatePublisher(registry.Object).PublishAsync(reference, package);

            registry.Verify(x => x.PublishExtension(reference, package), Times.Once);
        }

        [TestMethod]
        public async Task Publish_RejectsAPackageThatDoesNotMatchTheTargetType()
        {
            var reference = new MockArtifactReference(BicepTestConstants.DummyBicepFile, "target", ArtifactType.Module);
            var package = new ExtensionPackage(BinaryData.FromString("types"), LocalDeployEnabled: false, []);

            var publish = () => CreatePublisher(CreateRegistry().Object).PublishAsync(reference, package);

            await publish.Should().ThrowAsync<ArgumentException>();
        }

        private static Mock<IArtifactRegistry> CreateRegistry()
        {
            var registry = StrictMock.Of<IArtifactRegistry>();
            registry.SetupGet(x => x.Scheme).Returns("mock");
            registry.Setup(x => x.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(true);
            return registry;
        }

        private static IArtifactPublisher CreatePublisher(params IArtifactRegistry[] registries) => new ArtifactPublisher(new MockArtifactRegistryProvider(registries));

        private class MockArtifactReference : ExternalArtifactReference
        {
            public MockArtifactReference(BicepSourceFile referencingFile, string reference, ArtifactType type = ArtifactType.Module)
                : base(TemplateSpecArtifactContext.Create(referencingFile.LoadConfiguration(), referencingFile.LoadFeatures()), "mock", type)
            {
                this.Reference = reference;
            }

            public string Reference { get; }

            public override string UnqualifiedReference => this.Reference;

            public override bool Equals(object? obj) => obj is MockArtifactReference other && this.Reference.Equals(other.Reference);

            public override int GetHashCode() => this.Reference.GetHashCode();

        }

        private class MockArtifactRegistryProvider(IEnumerable<IArtifactRegistry> registries) : ArtifactRegistryProvider(registries)
        {
        }
    }
}
