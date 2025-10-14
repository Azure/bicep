// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Providers;
using Bicep.Core.Registry.Sessions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.Registry;

[TestClass]
public class OciRegistryTransportFactoryTests
{
    [TestMethod]
    public void GetTransport_ShouldReturnAzureTransport_ForKnownAzureHosts()
    {
        var azureTransport = new Mock<IOciRegistryTransport>(MockBehavior.Strict).Object;
        var genericTransport = new Mock<IOciRegistryTransport>(MockBehavior.Strict).Object;
        var factory = CreateFactory(azureTransport, genericTransport);

        var azureReference = OciRegistryHelper.CreateModuleReference("example.azurecr.io", "modules/foo", "v1", null);
        var sovereignReference = OciRegistryHelper.CreateModuleReference("example.azurecr.cn", "modules/foo", "v1", null);
        var mcrReference = OciRegistryHelper.CreateModuleReference("mcr.microsoft.com", "bicep/extensions/az", "v1", null);

        factory.GetTransport(azureReference).Should().BeSameAs(azureTransport);
        factory.GetTransport(sovereignReference).Should().BeSameAs(azureTransport);
        factory.GetTransport(mcrReference).Should().BeSameAs(azureTransport);

        factory.GetTransport(azureReference.Registry).Should().BeSameAs(azureTransport);
    }

    [TestMethod]
    public void GetTransport_ShouldReturnGenericTransport_ForNonAzureHosts()
    {
        var azureTransport = new Mock<IOciRegistryTransport>(MockBehavior.Strict).Object;
        var genericTransport = new Mock<IOciRegistryTransport>(MockBehavior.Strict).Object;
        var factory = CreateFactory(azureTransport, genericTransport);

        var ghcrReference = OciRegistryHelper.CreateModuleReference("ghcr.io", "contoso/bicep/modules/app", "v1", null);
        var localhostReference = OciRegistryHelper.CreateModuleReference("localhost:5000", "demo/app", "latest", null);

        factory.GetTransport(ghcrReference).Should().BeSameAs(genericTransport);
        factory.GetTransport(localhostReference).Should().BeSameAs(genericTransport);
        factory.GetTransport(ghcrReference.Registry).Should().BeSameAs(genericTransport);
        factory.GetTransport(localhostReference.Registry).Should().BeSameAs(genericTransport);
    }
    private static OciRegistryTransportFactory CreateFactory(IOciRegistryTransport azureTransport, IOciRegistryTransport genericTransport)
    {
        var azureProvider = new Mock<IRegistryProvider>(MockBehavior.Strict);
        azureProvider.SetupGet(p => p.Name).Returns(WellKnownRegistryProviders.Acr);
        azureProvider.SetupGet(p => p.Priority).Returns(100);
        azureProvider.Setup(p => p.CanHandle(It.IsAny<string>()))
            .Returns<string>(registry => OciRegistryTransportFactory.IsAzureHost(registry));
        azureProvider.Setup(p => p.GetTransport(It.IsAny<string>())).Returns(azureTransport);
        azureProvider.Setup(p => p.CreateSession(It.IsAny<RegistryRef>(), It.IsAny<RegistryProviderContext>()))
            .Throws<NotSupportedException>();

        var genericProvider = new Mock<IRegistryProvider>(MockBehavior.Strict);
        genericProvider.SetupGet(p => p.Name).Returns(WellKnownRegistryProviders.Generic);
        genericProvider.SetupGet(p => p.Priority).Returns(0);
        genericProvider.Setup(p => p.CanHandle(It.IsAny<string>())).Returns(true);
        genericProvider.Setup(p => p.GetTransport(It.IsAny<string>())).Returns(genericTransport);
        genericProvider.Setup(p => p.CreateSession(It.IsAny<RegistryRef>(), It.IsAny<RegistryProviderContext>()))
            .Returns(Mock.Of<IRegistrySession>());

        var providerFactory = new RegistryProviderFactory(new[]
        {
            azureProvider.Object,
            genericProvider.Object,
        });

        return new OciRegistryTransportFactory(providerFactory);
    }
}
