// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Mock.Registry.Catalog;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Moq;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Catalog
{
    [TestClass]
    public class RegistryCatalogTests
    {
        [TestMethod]
        public void GetRegistry_ShouldReturnSameObjectEachTime()
        {
            var indexer = RegistryCatalogMocks.CreateCatalogWithMocks(null,
                RegistryCatalogMocks.MockPrivateMetadataProvider(
                    "private.contoso.io",
                    [
                        ("bicep/abc", "description", "https://contoso.com/help", [new("1.0.0", "abc 1.0.0 description", "https://contoso.com/help/abc")]),
                        ("bicep/def", "description", "https://contoso.com/help", [new("1.0.0", "def 1.0.0 description", "https://contoso.com/help/def")]),
                    ]));

            var registry = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io", useOrasSdk: false);
            registry.Should().NotBeNull();
            registry.Registry.Should().Be("private.contoso.io");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io", useOrasSdk: false).Should().BeSameAs(registry);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io", useOrasSdk: false).Should().BeSameAs(registry);

            var registry2 = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com", useOrasSdk: false);
            registry2.Should().NotBeNull();
            registry2.Registry.Should().Be("mcr.microsoft.com");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com", useOrasSdk: false).Should().BeSameAs(registry2);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com", useOrasSdk: false).Should().BeSameAs(registry2);

            var registry3 = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid", useOrasSdk: false);
            registry3.Should().NotBeNull();
            registry3.Registry.Should().Be("unknown:and:invalid");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid", useOrasSdk: false).Should().BeSameAs(registry3);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid", useOrasSdk: false).Should().BeSameAs(registry3);

            var registry4 = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2", useOrasSdk: false);
            registry4.Should().NotBeNull();
            registry4.Registry.Should().Be("unknown 2");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2", useOrasSdk: false).Should().BeSameAs(registry4);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2", useOrasSdk: false).Should().BeSameAs(registry4);

            registry.Should().NotBeSameAs(registry2);
            registry.Should().NotBeSameAs(registry3);
            registry.Should().NotBeSameAs(registry4);
            registry2.Should().NotBeSameAs(registry3);
            registry2.Should().NotBeSameAs(registry4);
            registry3.Should().NotBeSameAs(registry4);
        }

        [TestMethod]
        public void GetRegistry_ForMcrMicrosoftCom_ShouldReturnPublicRegistry()
        {
            var publicProvider = RegistryCatalogMocks.MockPublicMetadataProvider([]);
            var indexer = RegistryCatalogMocks.CreateCatalogWithMocks(
                publicProvider,
                RegistryCatalogMocks.MockPrivateMetadataProvider(
                    "private.contoso.io",
                    [
                        ("bicep/abc", "description", "https://contoso.com/hep", [new("1.0.0", "abc 1.0.0 description", "https://contoso.com/help/abc")]),
                        ("bicep/def", "description", "https://contoso.com/hep", [new("1.0.0", "def 1.0.0 description", "https://contoso.com/help/def")]),
                    ]));

            var registry = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com", useOrasSdk: false);
            registry.Registry.Should().Be("mcr.microsoft.com");
            registry.Should().Be(publicProvider.Object);

            registry.Should().BeAssignableTo<IPublicModuleMetadataProvider>();
        }

        [TestMethod]
        public void TryGetCachedRegistry()
        {
            var publicProvider = RegistryCatalogMocks.MockPublicMetadataProvider([]);
            var indexer = RegistryCatalogMocks.CreateCatalogWithMocks(publicProvider);

            indexer.TryGetCachedRegistry("mcr.microsoft.com", useOrasSdk: false).Should().NotBeNull();
            indexer.TryGetCachedRegistry("private.contoso.io", useOrasSdk: false).Should().BeNull();

            var registry = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io", useOrasSdk: false);

            indexer.TryGetCachedRegistry("mcr.microsoft.com", useOrasSdk: false).Should().NotBeNull();
            indexer.TryGetCachedRegistry("private.contoso.io", useOrasSdk: false).Should().NotBeNull();

            registry.Should().BeAssignableTo<IRegistryModuleMetadataProvider>();
            registry.Should().NotBeAssignableTo<IPublicModuleMetadataProvider>();
        }

        [TestMethod]
        public void ProvidersAreCachedSeparatelyForEachTransport()
        {
            var catalog = new RegistryModuleCatalog(
                StrictMock.Of<IPublicModuleMetadataProvider>().Object,
                new PrivateAcrModuleMetadataProviderFactory(),
                StrictMock.Of<IOciRegistryTransportFactory>().Object,
                BicepTestConstants.BuiltInOnlyConfigurationManager);
            var cloud = BicepTestConstants.BuiltInConfiguration.Cloud;
            const string registry = "example.azurecr.io";

            var sdk = catalog.GetProviderForRegistry(cloud, registry, useOrasSdk: false);
            catalog.TryGetCachedRegistry(registry, true).Should().BeNull();
            var oras = catalog.GetProviderForRegistry(cloud, registry, true);

            sdk.Should().NotBeSameAs(oras);
            catalog.GetProviderForRegistry(cloud, registry, false).Should().BeSameAs(sdk);
            catalog.GetProviderForRegistry(cloud, registry, true).Should().BeSameAs(oras);
            catalog.TryGetCachedRegistry(registry, false).Should().BeSameAs(sdk);
            catalog.TryGetCachedRegistry(registry, true).Should().BeSameAs(oras);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CatalogOperationsUseTheSelectedTransport(bool useOrasSdk)
        {
            var cloud = BicepTestConstants.BuiltInConfiguration.Cloud;
            var reference = OciRegistryHelper.CreateModuleReference("example.azurecr.io", "modules/foo", "v1", null);
            var clientFactory = RegistryHelper.CreateMockRegistryClient(new RegistryHelper.RepoDescriptor(reference.Registry, reference.Repository, ["v1"]));
            var registryManager = new AzureContainerRegistryManager(clientFactory);
            await registryManager.PushArtifactAsync(cloud, reference, BicepMediaTypes.OciImageManifest, BicepMediaTypes.BicepModuleArtifactType,
                new("{}", BicepMediaTypes.BicepModuleConfigV1),
                [new("{}", BicepMediaTypes.BicepModuleLayerV1Json)], new(), CancellationToken.None);
            var artifact = await registryManager.PullArtifactAsync(cloud, reference, CancellationToken.None);
            var transport = StrictMock.Of<IOciRegistryTransport>();
            transport.Setup(x => x.GetRepositoryNamesAsync(cloud, reference.Registry, 10000, It.IsAny<CancellationToken>()))
                .ReturnsAsync([reference.Repository]);
            transport.Setup(x => x.GetRepositoryTagsAsync(cloud, reference.Registry, reference.Repository, It.IsAny<CancellationToken>()))
                .ReturnsAsync(["v1"]);
            transport.Setup(x => x.PullArtifactAsync(cloud,
                It.Is<IOciArtifactAddressComponents>(address => address.Repository == reference.Repository && address.Tag == "v1"),
                It.IsAny<CancellationToken>())).ReturnsAsync(artifact);
            var factory = StrictMock.Of<IOciRegistryTransportFactory>();
            factory.Setup(x => x.GetTransport(reference.Registry, useOrasSdk)).Returns(transport.Object);
            var catalog = new RegistryModuleCatalog(
                StrictMock.Of<IPublicModuleMetadataProvider>().Object,
                new PrivateAcrModuleMetadataProviderFactory(), factory.Object,
                BicepTestConstants.BuiltInOnlyConfigurationManager);

            var provider = catalog.GetProviderForRegistry(cloud, reference.Registry, useOrasSdk);
            var modules = await provider.TryGetModulesAsync();
            var versions = await modules.Single().TryGetVersionsAsync();

            versions.Should().ContainSingle().Which.IsBicepModule.Should().BeTrue();
            transport.VerifyAll();
            factory.Verify(x => x.GetTransport(reference.Registry, !useOrasSdk), Times.Never);
        }
    }
}
