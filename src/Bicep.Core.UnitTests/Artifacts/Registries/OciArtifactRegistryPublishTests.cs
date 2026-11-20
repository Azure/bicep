// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;
using Bicep.Core.RegistryClients.Oci;
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
    public async Task PublishModule_PushesTheEncodedPackage()
    {
        var reference = CreateReference(ArtifactType.Module);
        var template = BinaryData.FromString("""{ "metadata": { "description": "My module" } }""");
        var sources = BinaryData.FromString("sources");
        var session = CreateSession();
        session.Setup(x => x.PushAsync(reference, It.IsAny<OciArtifactPayload>(), CancellationToken.None))
            .Callback<IOciArtifactReference, OciArtifactPayload, CancellationToken>((_, payload, _) =>
            {
                payload.Manifest.ArtifactType.Should().Be(BicepMediaTypes.BicepModuleArtifactType);
                var decoded = OciModuleArtifactCodec.Decode(payload);
                decoded.Template.ToString().Should().Be(template.ToString());
                decoded.Sources!.ToString().Should().Be(sources.ToString());
                decoded.Description.Should().Be("My module");
                decoded.DocumentationUri.Should().Be("https://docs");
            })
            .Returns(Task.CompletedTask);

        await CreateRegistry(reference, session.Object).PublishModule(reference, ModuleArtifactPackage.FromTemplate(template, sources, "https://docs"));

        session.VerifyAll();
    }

    [TestMethod]
    public async Task PublishExtension_PushesTheEncodedPackage()
    {
        var reference = CreateReference(ArtifactType.Extension);
        var package = new ExtensionArtifactPackage(BinaryData.FromString("types"), []);
        var session = CreateSession();
        session.Setup(x => x.PushAsync(reference, It.IsAny<OciArtifactPayload>(), CancellationToken.None))
            .Callback<IOciArtifactReference, OciArtifactPayload, CancellationToken>((_, payload, _) =>
            {
                payload.Manifest.ArtifactType.Should().Be(BicepMediaTypes.BicepExtensionArtifactType);
                OciExtensionArtifactCodec.Decode(payload).Types.ToString().Should().Be("types");
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
            ? () => registry.PublishExtension(reference, new ExtensionArtifactPackage(BinaryData.FromString("types"), []))
            : () => registry.PublishModule(reference, new ModuleArtifactPackage(BinaryData.FromString("{}"), null, null));

        await publish.Should().ThrowAsync<ArgumentException>().WithParameterName("reference");
        session.Verify(x => x.PushAsync(It.IsAny<IOciArtifactReference>(), It.IsAny<OciArtifactPayload>(), It.IsAny<CancellationToken>()), Times.Never);
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
        transportFactory.Setup(x => x.CreateSession(reference.Context.Cloud, reference.Registry, reference.Repository, reference.UseOrasSdk)).Returns(session);
        return new OciArtifactRegistry(BicepTestConstants.TestRegistryConfiguration, transportFactory.Object, NullLogger<OciArtifactRegistry>.Instance);
    }
}
