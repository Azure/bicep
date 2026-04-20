// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Azure;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry;

[TestClass]
public class OciRegistryTransportFactoryTests
{
    [DataTestMethod]
    [DataRow("example.azurecr.io")]
    [DataRow("example.azurecr.cn")]
    [DataRow("example.azurecr.us")]
    [DataRow("mcr.microsoft.com")]
    public void IsAzureSdkHost_ReturnsTrue_ForKnownAzureHosts(string host)
    {
        OciRegistryTransportFactory.IsAzureSdkHost(host).Should().BeTrue();
    }

    [DataTestMethod]
    [DataRow("ghcr.io")]
    [DataRow("localhost:5000")]
    [DataRow("docker.io")]
    public void IsAzureSdkHost_ReturnsFalse_ForNonAzureHosts(string host)
    {
        OciRegistryTransportFactory.IsAzureSdkHost(host).Should().BeFalse();
    }

    [TestMethod]
    public void GetTransport_AlwaysReturnsAzureTransport()
    {
        var (factory, azureTransport) = CreateFactory();

        factory.GetTransport("example.azurecr.io").Should().BeSameAs(azureTransport);
        factory.GetTransport("ghcr.io").Should().BeSameAs(azureTransport);

        var reference = OciRegistryHelper.CreateModuleReference("ghcr.io", "contoso/bicep/modules/app", "v1", null);
        factory.GetTransport(reference).Should().BeSameAs(azureTransport);
    }

    [TestMethod]
    public void CreateSession_AzureHost_ReturnsAcrSession()
    {
        var (factory, _) = CreateFactory();
        var reference = OciRegistryHelper.CreateModuleReference("example.azurecr.io", "modules/foo", "v1", null);

        var session = factory.CreateSession(reference, BicepTestConstants.BuiltInConfiguration.Cloud);

        session.Should().BeOfType<AcrRegistrySession>();
    }

    [TestMethod]
    public void CreateSession_NonAzureHost_OciDisabled_FallsBackToAcrSession()
    {
        var (factory, _) = CreateFactory();
        var reference = OciRegistryHelper.CreateModuleReference("ghcr.io", "contoso/modules/app", "v1", null);

        var session = factory.CreateSession(reference, BicepTestConstants.BuiltInConfiguration.Cloud);

        session.Should().BeOfType<AcrRegistrySession>();
    }

    private static (OciRegistryTransportFactory factory, AzureContainerRegistryManager azure) CreateFactory()
    {
        var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;
        var azure = new AzureContainerRegistryManager(clientFactory);
        var docker = new DockerCredentialProvider(TestEnvironment.Default, new MockFileSystem());
        return (new OciRegistryTransportFactory(azure, docker), azure);
    }
}
