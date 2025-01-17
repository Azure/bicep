// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Indexing;
using Bicep.Core.Registry.Indexing.HttpClients;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Indexing
{
    [TestClass]
    public class RegistryIndexerTests //asdfg2
    {
        //asdfg: test after calling to get details, calling to get versions shouldn't require another call to the server

        private IConfigurationManager ConfigurationManagerWithModuleAliases(string moduleAliasesJson)
        {
            var configuration = BicepTestConstants.BuiltInConfiguration.With(
                moduleAliases: RegistryIndexerMocks.ModuleAliases(moduleAliasesJson));

            var configurationManager = StrictMock.Of<IConfigurationManager>();
            configurationManager.Setup(x => x.GetConfiguration(It.IsAny<Uri>())).Returns(configuration);
            return configurationManager.Object;
        }

        [TestMethod]
        public void GetRegistry_ShouldReturnSameObjectEachTime()
        {
            var indexer = RegistryIndexerMocks.CreateRegistryIndexer(null,
                RegistryIndexerMocks.MockPrivateMetadataProvider(
                    "private.contoso.io",
                    [
                        ("bicep/abc", "description", "https://contoso.com/hep", [ ("1.0.0", "abc 1.0.0 description", "https://contoso.com/help/abc") ]),
                        ("bicep/def", "description", "https://contoso.com/hep", [ ("1.0.0", "def 1.0.0 description", "https://contoso.com/help/def") ]),
                    ]));
            var configurationManagerMock = ConfigurationManagerWithModuleAliases(
                    """
                    {
                        "br": {
                            "contoso": {
                                "registry": "private.contoso.io"
                            }
                        }
                    }
                    """);

            var registry = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io");
            registry.Should().NotBeNull();
            registry.Registry.Should().Be("private.contoso.io");
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io").Should().BeSameAs(registry);
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io").Should().BeSameAs(registry);

            var registry2 = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com");
            registry2.Should().NotBeNull();
            registry2.Registry.Should().Be("mcr.microsoft.com");
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com").Should().BeSameAs(registry2);
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com").Should().BeSameAs(registry2);

            var registry3 = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid");
            registry3.Should().NotBeNull();
            registry3.Registry.Should().Be("unknown:and:invalid");
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid").Should().BeSameAs(registry3);
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown:and:invalid").Should().BeSameAs(registry3);

            var registry4 = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2");
            registry4.Should().NotBeNull();
            registry4.Registry.Should().Be("unknown 2");
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2").Should().BeSameAs(registry4);
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "unknown 2").Should().BeSameAs(registry4);

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
            var publicProvider = RegistryIndexerMocks.MockPublicMetadataProvider([]);
            var indexer = RegistryIndexerMocks.CreateRegistryIndexer(
                publicProvider,
                RegistryIndexerMocks.MockPrivateMetadataProvider(
                    "private.contoso.io",
                    [
                        ("bicep/abc", "description", "https://contoso.com/hep", [ ("1.0.0", "abc 1.0.0 description", "https://contoso.com/help/abc") ]),
                        ("bicep/def", "description", "https://contoso.com/hep", [ ("1.0.0", "def 1.0.0 description", "https://contoso.com/help/def") ]),
                    ]));
            var configurationManagerMock = ConfigurationManagerWithModuleAliases(
                    """
                    {
                        "br": {
                            "contoso": {
                                "registry": "private.contoso.io"
                            }
                        }
                    }
                    """);

            var registry2 = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "mcr.microsoft.com");
            registry2.Registry.Should().Be("mcr.microsoft.com");
            registry2.Should().Be(publicProvider.Object);
        }

        [TestMethod]
        public void TryGetCachedRegistry()
        {
            var publicProvider = RegistryIndexerMocks.MockPublicMetadataProvider([]);
            var indexer = RegistryIndexerMocks.CreateRegistryIndexer(publicProvider);

            indexer.TryGetCachedRegistry("mcr.microsoft.com").Should().NotBeNull();
            indexer.TryGetCachedRegistry("private.contoso.io").Should().BeNull();

            var registry2 = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "private.contoso.io");

            indexer.TryGetCachedRegistry("mcr.microsoft.com").Should().NotBeNull();
            indexer.TryGetCachedRegistry("private.contoso.io").Should().NotBeNull();
        }

    }
}
