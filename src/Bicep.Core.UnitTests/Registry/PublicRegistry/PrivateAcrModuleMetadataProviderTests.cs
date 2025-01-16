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


        private static readonly MockHttpMessageHandler MockHttpMessageHandler = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            MockHttpMessageHandler
                .When(HttpMethod.Get, "*")
                .Respond("application/json", "asdfg ModuleIndexJson");
        }

        [TestMethod]
        public async Task Asdfg()
        {
            var moduleName = "module1";
            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{moduleName}".ToLowerInvariant();
            var bicepModuleContents = "// hello";
            var documentationUri = "https://contoso.com/hep";

            var (clientFactory, blobClients) = RegistryHelper.CreateMockRegistryClients((registryStr, repository));

            var client = new MockContainerRegistryClient();

            var blobClient = blobClients[(registryUri, repository)];

            await RegistryHelper.PublishModuleToRegistryAsync(clientFactory, BicepTestConstants.FileSystem, "modulename", $"br:example.com/test/{moduleName}:v1", bicepModuleContents, publishSource: false, documentationUri);

            var manifest = blobClient.Manifests.Single().Value.ToObjectFromJson<OciManifest>(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


            //// compile and publish modules using throwaway file system
            //var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
            //    new MockFileSystem(),
            //    [.. options.PublishedModules.Select(x => (x, "", true))]);


            //asdfg var client = new MockRegistryBlobClient();

            //asdfg2 use this???
            var clientFactory2 = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory2.Setup(m => m.CreateAuthenticatedRegistryClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>())).Returns(client);

            //clientFactory2.Setup(m => m.)

            var acrManager = new AzureContainerRegistryManager(clientFactory2.Object);
            client.MockRepositoryNames = ["abc", "def", "bicep/abc", "bicep/def"];
            var asdfg1 = acrManager.GetCatalogAsync(BicepTestConstants.BuiltInConfiguration.Cloud, "registry.contoso.io");
        }

        //asdfg
        //[TestMethod]
        //public void GetModules_ForwardsCompatibleWithOriginalVersion()
        //{
        //    // Earlier Bicep versions should not be confused by new metadata formats
        //    var metadataStream = new MemoryStream(Encoding.UTF8.GetBytes(ModuleIndexJson));
        //    ModuleMetadata_Original[] metadata = [.. JsonSerializer.Deserialize<ModuleMetadata_Original[]>(metadataStream)!];

        //    metadata.Length.Should().BeGreaterThanOrEqualTo(29);
        //    metadata.Select(m => m.moduleName).Should().Contain("samples/array-loop");
        //    metadata.First(m => m.moduleName == "samples/array-loop").tags.Should().Contain("1.0.1");
        //    metadata.First(m => m.moduleName == "samples/array-loop").tags.Should().Contain("1.0.2");
        //}

        //[TestMethod]
        //public async Task GetModules_Count_SanityCheck()
        //{
        //    PublicModuleMetadataProvider provider = new(CreateTypedClient());
        //    (await provider.TryUpdateCacheAsync()).Should().BeTrue();
        //    var modules = await provider.TryGetModulesAsync();
        //    modules.Should().HaveCount(50);
        //}

        //[TestMethod]
        //public async Task GetModules_IfOnlyLastTagHasDescription()
        //{
        //    PublicModuleMetadataProvider provider = new(CreateTypedClient());
        //    (await provider.TryUpdateCacheAsync()).Should().BeTrue();
        //    var modules = await provider.TryGetModulesAsync();
        //    var m = modules.Should().Contain(m => m.ModuleName == "bicep/samples/hello-world")
        //        .Which;
        //    m.Description.Should().Be("A \"שָׁלוֹם עוֹלָם\" sample Bicep registry module");
        //    m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.4/modules/samples/hello-world/README.md");
        //}

        //[TestMethod]
        //public async Task GetModules_IfMultipleTagsHaveDescriptions()
        //{
        //    PublicModuleMetadataProvider provider = new(CreateTypedClient());
        //    (await provider.TryUpdateCacheAsync()).Should().BeTrue();
        //    var modules = await provider.TryGetModulesAsync();
        //    var m = modules.Should().Contain(m => m.ModuleName == "bicep/lz/sub-vending")
        //        .Which;
        //    m.Description.Should().Be("This module is designed to accelerate deployment of landing zones (aka Subscriptions) within an Azure AD Tenant.");
        //    m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.4.2/modules/lz/sub-vending/README.md");
        //}

        //[TestMethod]
        //public async Task GetModuleVersions_SortsBySemver() //asdfg test for private
        //{
        //    PublicModuleMetadataProvider provider = new(CreateTypedClient());
        //    var versions = (await provider.TryGetModuleVersionsAsync("bicep/samples/array-loop")).Select(x => x.Version);//asdfg test

        //    versions.Should().Equal(
        //          "1.10.1",
        //          "1.0.3",
        //          "1.0.2",
        //          "1.0.2-preview",
        //          "1.0.1");
        //}
    }
}
