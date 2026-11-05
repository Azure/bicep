// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using System.Net;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.AzureApi;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Azure;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry;

[TestClass]
public class OciRegistryTransportFactoryTests
{
    [TestMethod]
    [DataRow("example.azurecr.io")]
    [DataRow("example.azurecr.cn")]
    [DataRow("example.azurecr.us")]
    [DataRow("example.azurecr.de")]
    [DataRow("example.azurecr.gov")]
    [DataRow("EXAMPLE.azurecr.io:443")]
    public void IsAzureRegistry_ReturnsTrue_ForAcrHosts(string host)
    {
        OciRegistryTransportFactory.IsAzureRegistry(host).Should().BeTrue();
    }

    [TestMethod]
    [DataRow("ghcr.io")]
    [DataRow("localhost:5000")]
    [DataRow("docker.io")]
    [DataRow("mcr.microsoft.com")]
    [DataRow("mcr.azure.cn")]
    [DataRow("example.azurecr.io.attacker.example")]
    public void IsAzureRegistry_ReturnsFalse_ForOtherHosts(string host)
    {
        OciRegistryTransportFactory.IsAzureRegistry(host).Should().BeFalse();
    }

    [TestMethod]
    [DataRow("example.azurecr.io", false)]
    [DataRow("example.azurecr.io", true)]
    [DataRow("mcr.microsoft.com", false)]
    [DataRow("mcr.microsoft.com", true)]
    [DataRow("ghcr.io", false)]
    [DataRow("ghcr.io", true)]
    [DataRow("localhost:5000", false)]
    [DataRow("localhost:5000", true)]
    public void GetTransport_SelectsExclusivelyByFeatureFlag(string registry, bool useOrasSdk)
    {
        var (factory, transport, azure) = CreateFactory();
        var reference = CreateReference(registry, useOrasSdk);
        var expected = useOrasSdk ? (IOciRegistryTransport)transport : azure;
        factory.GetTransport(registry, useOrasSdk).Should().BeSameAs(expected);
        factory.GetTransport(reference).Should().BeSameAs(expected);
    }

    [TestMethod]
    [DataRow("example.azurecr.io", false)]
    [DataRow("example.azurecr.io", true)]
    [DataRow("mcr.microsoft.com", false)]
    [DataRow("mcr.microsoft.com", true)]
    [DataRow("ghcr.io", false)]
    [DataRow("ghcr.io", true)]
    public async Task CreateSession_SelectsExclusivelyByFeatureFlag(string registry, bool useOrasSdk)
    {
        var (factory, _, _) = CreateFactory();
        var reference = CreateReference(registry, useOrasSdk);
        await using var session = factory.CreateSession(reference, BicepTestConstants.BuiltInConfiguration.Cloud);
        session.Should().BeOfType(useOrasSdk ? typeof(OrasRegistrySession) : typeof(AcrRegistrySession));
    }

    [TestMethod]
    public void GetTransport_UsesAcrSdkWhenExplicitlyDisabled()
    {
        var (factory, _, azure) = CreateFactory();
        factory.GetTransport("example.azurecr.io", useOrasSdk: false).Should().BeSameAs(azure);
    }

    [TestMethod]
    public async Task DisabledFlagUsesSdkWithoutCreatingOrasClients()
    {
        var sdk = StrictMock.Of<IContainerRegistryClientFactory>();
        var blob = StrictMock.Of<ContainerRegistryContentClient>();
        var cloud = BicepTestConstants.BuiltInConfiguration.Cloud;
        var reference = CreateReference("example.azurecr.io", false);
        blob.Setup(x => x.GetManifestAsync("v1", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(403, "SDK request"));
        sdk.Setup(x => x.CreateAnonymousBlobClient(cloud, new Uri("https://example.azurecr.io"), "modules/foo")).Returns(blob.Object);
        sdk.Setup(x => x.CreateAuthenticatedBlobClient(cloud, new Uri("https://example.azurecr.io"), "modules/foo")).Returns(blob.Object);
        var (_, oras, _) = CreateFactory();
        var factory = new OciRegistryTransportFactory(oras, new AzureContainerRegistryManager(sdk.Object));
        await using var session = factory.CreateSession(reference, cloud);

        var action = () => session.PullAsync(reference, CancellationToken.None);

        await action.Should().ThrowAsync<RequestFailedException>().WithMessage("SDK request");
        sdk.VerifyAll();
    }

    [TestMethod]
    public async Task EnabledFlagDoesNotFallBackToSdkOnOrasFailure()
    {
        using var http = new MockHttpMessageHandler();
        http.When("*").Respond(HttpStatusCode.NotFound);
        var clients = StrictMock.Of<IHttpClientFactory>();
        clients.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(http, disposeHandler: false));
        var sdk = StrictMock.Of<IContainerRegistryClientFactory>();
        var oras = new OrasOciRegistryTransport(
            new RegistryConfiguration(PermitUntrustedRegistries: false),
            new DockerCredentialProvider(TestEnvironment.Default, new InMemoryFileExplorer()),
            StrictMock.Of<ITokenCredentialFactory>().Object, clients.Object);
        var factory = new OciRegistryTransportFactory(oras, new AzureContainerRegistryManager(sdk.Object));
        var reference = CreateReference("example.azurecr.io", true);
        await using var session = factory.CreateSession(reference, BicepTestConstants.BuiltInConfiguration.Cloud);

        var action = () => session.PullAsync(reference, CancellationToken.None);

        await action.Should().ThrowAsync<ExternalArtifactException>().WithMessage("*does not exist*");
        sdk.VerifyNoOtherCalls();
    }

    private static OciArtifactReference CreateReference(string registry, bool useOrasSdk)
    {
        var reference = OciRegistryHelper.CreateModuleReference(registry, "modules/foo", "v1", null);
        var features = StrictMock.Of<IFeatureProvider>();
        features.Setup(x => x.OciEnabled).Returns(useOrasSdk);
        return new OciArtifactReference(features.Object, reference.Configuration, reference.Type, reference.AddressComponents);
    }

    private static (OciRegistryTransportFactory Factory, OrasOciRegistryTransport Transport, AzureContainerRegistryManager Azure) CreateFactory()
    {
        var docker = new DockerCredentialProvider(TestEnvironment.Default, new InMemoryFileExplorer());
        var transport = new OrasOciRegistryTransport(
            new RegistryConfiguration(PermitUntrustedRegistries: false),
            docker,
            StrictMock.Of<ITokenCredentialFactory>().Object,
            StrictMock.Of<IHttpClientFactory>().Object);
        var azure = new AzureContainerRegistryManager(StrictMock.Of<IContainerRegistryClientFactory>().Object);
        return (new OciRegistryTransportFactory(transport, azure), transport, azure);
    }
}
