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

            var registry = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io");
            registry.Should().NotBeNull();
            registry.Registry.Should().Be("private.contoso.io");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io").Should().BeSameAs(registry);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io").Should().BeSameAs(registry);

            var registry2 = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com");
            registry2.Should().NotBeNull();
            registry2.Registry.Should().Be("mcr.microsoft.com");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com").Should().BeSameAs(registry2);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com").Should().BeSameAs(registry2);

            var registry3 = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid");
            registry3.Should().NotBeNull();
            registry3.Registry.Should().Be("unknown:and:invalid");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid").Should().BeSameAs(registry3);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid").Should().BeSameAs(registry3);

            var registry4 = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2");
            registry4.Should().NotBeNull();
            registry4.Registry.Should().Be("unknown 2");
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2").Should().BeSameAs(registry4);
            indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2").Should().BeSameAs(registry4);

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

            var registry = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com");
            registry.Registry.Should().Be("mcr.microsoft.com");
            registry.Should().Be(publicProvider.Object);

            registry.Should().BeAssignableTo<IPublicModuleMetadataProvider>();
        }

        [TestMethod]
        public void TryGetCachedRegistry()
        {
            var publicProvider = RegistryCatalogMocks.MockPublicMetadataProvider([]);
            var indexer = RegistryCatalogMocks.CreateCatalogWithMocks(publicProvider);

            indexer.TryGetCachedRegistry("mcr.microsoft.com").Should().NotBeNull();
            indexer.TryGetCachedRegistry("private.contoso.io").Should().BeNull();

            var registry = indexer.GetProviderForRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io");

            indexer.TryGetCachedRegistry("mcr.microsoft.com").Should().NotBeNull();
            indexer.TryGetCachedRegistry("private.contoso.io").Should().NotBeNull();

            registry.Should().BeAssignableTo<IRegistryModuleMetadataProvider>();
            registry.Should().NotBeAssignableTo<IPublicModuleMetadataProvider>();
        }
    }
}
