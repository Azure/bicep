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
    public class PrivateAcrModuleMetadataProviderTests //asdfg2
    {
        //asdfg
        //private PublicModuleMetadataHttpClient CreateTypedClient() { //asdfg
        //    var httpClient = MockHttpMessageHandler.ToHttpClient();
        //    return new PublicModuleMetadataHttpClient(httpClient);
        //}


        //private static readonly MockHttpMessageHandler MockHttpMessageHandler = new();

        //[ClassInitialize]
        //public static void ClassInitialize(TestContext _)
        //{
        //    MockHttpMessageHandler
        //        .When(HttpMethod.Get, "*")
        //        .Respond("application/json", "asdfg ModuleIndexJson");
        //}

        //asdfg: test after calling to get details, calling to get versions shouldn't require another call to the server

        [TestMethod]
        public async Task Asdfg()
        {
            //var moduleName = "module1";
            //var registryStr = "example.com";
            //var registryUri = new Uri($"https://{registryStr}");
            //var repository = $"test/{moduleName}".ToLowerInvariant();

            //asdfg
            //var bicepModuleContents = "// hello";
            //var documentationUri = "https://contoso.com/hep";

            //var (clientFactory, blobClients) = RegistryHelper.CreateMockRegistryClients((registryStr, repository));

            //var client = new MockContainerRegistryClient();

            //var blobClient = blobClients[(registryUri, repository)];

            //await RegistryHelper.PublishModuleToRegistryAsync(clientFactory, BicepTestConstants.FileSystem, "modulename", $"br:example.com/test/{moduleName}:v1", bicepModuleContents, publishSource: false, documentationUri);

            //var manifest = blobClient.Manifests.Single().Value.ToObjectFromJson<OciManifest>(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


            //// compile and publish modules using throwaway file system
            //var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
            //    new MockFileSystem(),
            //    [.. options.PublishedModules.Select(x => (x, "", true))]);


            //asdfg var client = new MockRegistryBlobClient();

            //asdfg2 use this???
            //var clientFactory2 = StrictMock.Of<IContainerRegistryClientFactory>();
            //clientFactory2.Setup(m => m.CreateAuthenticatedRegistryClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>())).Returns(client);

            //clientFactory2.Setup(m => m.)

            //var acrManager = new AzureContainerRegistryManager(clientFactory2.Object);
            //client.MockRepositoryNames = ["abc", "def", "bicep/abc", "bicep/def"];
            //var asdfg1 = acrManager.GetCatalogAsync(BicepTestConstants.BuiltInConfiguration.Cloud, "registry.contoso.io");

            var indexer = RegistryIndexerMocks.CreateRegistryIndexer(null,
                RegistryIndexerMocks.MockPrivateMetadataProvider(
                    "registry.contoso.io",
                    [
                        ("bicep/abc", "description", "https://contoso.com/hep", [ ("1.0.0", "abc 1.0.0 description", "https://contoso.com/help/abc") ]),
                        ("bicep/def", "description", "https://contoso.com/hep", [ ("1.0.0", "def 1.0.0 description", "https://contoso.com/help/def") ]),
                    ]));

            var configuration = BicepTestConstants.BuiltInConfiguration.With(
                moduleAliases: RegistryIndexerMocks.ModuleAliases(
                    """
                    {
                        "br": {
                            "contoso": {
                                "registry": "private.contoso.io"
                            }
                        }
                    }
                    """));
            var configurationManager = StrictMock.Of<IConfigurationManager>(); //asdfg extract
            configurationManager.Setup(x => x.GetConfiguration(It.IsAny<Uri>())).Returns(configuration);

            var registry = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "registry.contoso.io");
            registry.Should().NotBeNull();
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "registry.contoso.io").Should().BeSameAs(registry); //asdfg separate test

            var modules = await registry.TryGetModulesAsync();
            modules.Should().HaveCount(2);

            //modules.Should().SatisfyRespectively(
            //    x =>
            //    {
            //        x.ModuleName.Should().Be("bicep/abc");
            //        (await x.TryGetVersionsAsync()).Should().HaveCount(1);
            //        x.TryGetVersionsAsync().Result[0].Should().BeEquivalentTo(
            //            new RegistryModuleVersionMetadata("1.0.0", new RegistryMetadataDetails("abc 1.0.0 description", "https://contoso.com/help/abc")));
            //    },
            //    x => x.ModuleName.Should().Be("bicep/def")
            //);
        }

        [TestMethod]
        public async Task Asdfg2()
        {
            var indexer = RegistryIndexerMocks.CreateRegistryIndexer(null,
                RegistryIndexerMocks.MockPrivateMetadataProvider(
                    "registry.contoso.io",
                    [
                        ("bicep/abc", "description", "https://contoso.com/hep", [ ("1.0.0", "abc 1.0.0 description", "https://contoso.com/help/abc") ]),
                        ("bicep/def", "description", "https://contoso.com/hep", [ ("1.0.0", "def 1.0.0 description", "https://contoso.com/help/def") ]),
                    ]));

            var configuration = BicepTestConstants.BuiltInConfiguration.With(
                moduleAliases: RegistryIndexerMocks.ModuleAliases(
                    """
                    {
                        "br": {
                            "contoso": {
                                "registry": "private.contoso.io"
                            }
                        }
                    }
                    """));
            var configurationManager = StrictMock.Of<IConfigurationManager>(); //asdfg extract
            configurationManager.Setup(x => x.GetConfiguration(It.IsAny<Uri>())).Returns(configuration);

            var registry = indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "registry.contoso.io");
            registry.Should().NotBeNull();
            indexer.GetRegistry(BicepTestConstants.BuiltInConfiguration.Cloud, "registry.contoso.io").Should().BeSameAs(registry); //asdfg separate test

            var modules = await registry.TryGetModulesAsync();
            modules.Should().HaveCount(2);
        }
    }
}
