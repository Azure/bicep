// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrasProject.Oras.Exceptions;

namespace Bicep.Core.UnitTests.Artifacts.Registries;

[TestClass]
public class OciArtifactRegistryPublishTests
{
    [TestMethod]
    [DataRow(ArtifactType.Module)]
    [DataRow(ArtifactType.Extension)]
    public async Task Exists_UsesTheTargetReference(ArtifactType artifactType)
    {
        var reference = CreateReference(artifactType);
        var session = CreateSession();
        session.Setup(x => x.ResolveAsync(reference, CancellationToken.None))
            .ReturnsAsync(("digest", new OciManifest(2, null, null, new OciDescriptor("{}", "config"), [], [])));

        var registry = CreateRegistry(reference, session.Object);
        (await registry.CheckArtifactExists(reference)).Should().BeTrue();

        session.Setup(x => x.ResolveAsync(reference, CancellationToken.None)).ThrowsAsync(new NotFoundException("not found"));
        (await registry.CheckArtifactExists(reference)).Should().BeFalse();
    }

    [TestMethod]
    public async Task PublishModule_PassesTheTemplateDescriptionAndSourcesToTheRegistry()
    {
        var reference = CreateReference(ArtifactType.Module);
        var template = BinaryData.FromString("""{ "metadata": { "description": "My module" } }""");
        var sources = BinaryData.FromString("sources");
        var session = CreateSession();
        session.Setup(x => x.PushAsync(
                reference, BicepMediaTypes.OciImageManifest, BicepMediaTypes.BicepModuleArtifactType,
                It.IsAny<OciDescriptor>(), It.IsAny<IEnumerable<OciDescriptor>>(),
                It.IsAny<OciManifestAnnotationsBuilder>(), CancellationToken.None))
            .Callback<IOciArtifactReference, string?, string?, OciDescriptor, IEnumerable<OciDescriptor>, OciManifestAnnotationsBuilder, CancellationToken>(
                (_, _, _, config, layers, annotations, _) =>
                {
                    config.MediaType.Should().Be(BicepMediaTypes.BicepModuleConfigV1);
                    layers.Select(layer => layer.Data).Should().Equal(template, sources);
                    annotations.Build().Should().ContainKey(OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation).WhoseValue.Should().Be("My module");
                    annotations.Build().Should().ContainKey(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation).WhoseValue.Should().Be("https://docs");
                })
            .Returns(Task.CompletedTask);

        await CreateRegistry(reference, session.Object).PublishModule(reference, new ModulePackage(template, sources, "https://docs"));

        session.VerifyAll();
    }

    [TestMethod]
    public async Task PublishExtension_PassesThePackageToTheRegistry()
    {
        var reference = CreateReference(ArtifactType.Extension);
        var package = new ExtensionPackage(BinaryData.FromString("types"), LocalDeployEnabled: false, []);
        var session = CreateSession();
        session.Setup(x => x.PushAsync(
                reference, BicepMediaTypes.OciImageManifest, BicepMediaTypes.BicepExtensionArtifactType,
                It.IsAny<OciDescriptor>(), It.IsAny<IEnumerable<OciDescriptor>>(),
                It.IsAny<OciManifestAnnotationsBuilder>(), CancellationToken.None))
            .Callback<IOciArtifactReference, string?, string?, OciDescriptor, IEnumerable<OciDescriptor>, OciManifestAnnotationsBuilder, CancellationToken>(
                (_, _, _, config, layers, _, _) =>
                {
                    config.MediaType.Should().Be(BicepMediaTypes.BicepExtensionConfigV1);
                    layers.Single().Data.Should().BeSameAs(package.Types);
                })
            .Returns(Task.CompletedTask);

        await CreateRegistry(reference, session.Object).PublishExtension(reference, package);

        session.VerifyAll();
    }

    [TestMethod]
    [DataRow(ArtifactType.Module)]
    [DataRow(ArtifactType.Extension)]
    public async Task Publish_RejectsAPackageThatDoesNotMatchTheTargetType(ArtifactType targetType)
    {
        var reference = CreateReference(targetType);
        var session = CreateSession();
        var registry = CreateRegistry(reference, session.Object);

        Func<Task> publish = targetType == ArtifactType.Module
            ? () => registry.PublishExtension(reference, new ExtensionPackage(BinaryData.FromString("types"), LocalDeployEnabled: false, []))
            : () => registry.PublishModule(reference, new ModulePackage(BinaryData.FromString("{}"), null, null));

        await publish.Should().ThrowAsync<ArgumentException>().WithParameterName("reference");
        session.Verify(x => x.PushAsync(
            It.IsAny<IOciArtifactReference>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<OciDescriptor>(), It.IsAny<IEnumerable<OciDescriptor>>(),
            It.IsAny<OciManifestAnnotationsBuilder>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static OciArtifactReference CreateReference(ArtifactType type) =>
        new(BicepTestConstants.OciArtifactContext, type, "example.com", "test/artifact", "v1", null);

    private static Mock<IRegistrySession> CreateSession()
    {
        var session = StrictMock.Of<IRegistrySession>();
        session.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return session;
    }

    private static OciArtifactRegistry CreateRegistry(OciArtifactReference reference, IRegistrySession session)
    {
        var transportFactory = StrictMock.Of<IOciRegistryTransportFactory>();
        transportFactory.Setup(x => x.CreateSession(reference)).Returns(session);
        return new OciArtifactRegistry(BicepTestConstants.TestRegistryConfiguration, transportFactory.Object, NullLogger<OciArtifactRegistry>.Instance);
    }
}
