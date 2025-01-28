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
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Catalog
{
    [TestClass]
    public class PrivateAcrModuleMetadataProviderTests
    {
        [TestMethod]
        public async Task TryGetModulesAsync()
        {
            FakeContainerRegistryClient containerRegistryClient = new();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerRegistryClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:registry.contoso.io/test/module2:v1", "param p2 string", WithSource: true),
                    new("br:registry.contoso.io/test/module1:v2", "param p12 string", WithSource: false),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);
            var modules = await provider.TryGetModulesAsync();

            modules.Should().HaveCount(2);
        }

        [TestMethod]
        public async Task GetCachedModules()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:registry.contoso.io/test/module2:v1", "param p2 string", WithSource : true),
                    new("br:registry.contoso.io/test/module1:v2", "param p12 string", WithSource: false),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            provider.GetCachedModules().Should().HaveCount(0);

            var modules = await provider.TryGetModulesAsync();
            modules.Should().HaveCount(2);

            provider.GetCachedModules().Should().HaveCount(2);
            provider.GetCachedModules().Should().HaveCount(2);
        }

        [TestMethod]
        public async Task TryGetModulesAsync_ShouldCacheResult()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:registry.contoso.io/test/module2:v1", "param p2 string", WithSource: true),
                    new("br:registry.contoso.io/test/module1:v2", "param p12 string", WithSource : false),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            var modules = await provider.TryGetModulesAsync();
            modules.Should().HaveCount(2);
            containerClient.CallsToGetRepositoryNamesAsync.Should().Be(1);

            modules = await provider.TryGetModulesAsync();
            containerClient.CallsToGetRepositoryNamesAsync.Should().Be(1);
        }

        [TestMethod]
        public async Task GetDetails_ShouldBeCached()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "metadata description = 'this is module 1 version 1'\nparam p1 bool", WithSource: true, DocumentationUri: "http://contoso.com/help11"),
                    new("br:registry.contoso.io/test/module2:v1", "metadata description = 'this is module 2 version 1'\nparam p2 string", WithSource: true, DocumentationUri: "http://contoso.com/help21"),
                    new("br:registry.contoso.io/test/module1:v2", "metadata description = 'this is module 1 version 2'\nparam p12 string", WithSource: false, DocumentationUri: "http://contoso.com/help12"),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);
            provider.GetCachedModules().Should().BeEmpty();

            var module = await provider.TryGetModuleAsync("test/module1");
            module.Should().NotBeNull();
            provider.GetCachedModules().Should().NotBeEmpty();

            var details = await module!.TryGetDetailsAsync();
            details.Description.Should().Be("this is module 1 version 2");
            details.DocumentationUri.Should().Be("http://contoso.com/help12");
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);

            // Verify it's cached
            var details2 = await module!.TryGetDetailsAsync();
            details.Should().BeSameAs(details2);
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);

            var cached = provider.GetCachedModules();
            provider.GetCachedModules().Should().NotBeEmpty();
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);
        }

        [TestMethod]
        public async Task GetVersions_ShouldBeCached()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "metadata description = 'this is module 1 version 1'\nparam p1 bool", WithSource: true, DocumentationUri: "http://contoso.com/help11"),
                    new("br:registry.contoso.io/test/module2:v1", "metadata description = 'this is module 2 version 1'\nparam p2 string", WithSource: true, DocumentationUri: "http://contoso.com/help21"),
                    new("br:registry.contoso.io/test/module1:v2", "metadata description = 'this is module 1 version 2'\nparam p12 string", WithSource: false, DocumentationUri: "http://contoso.com/help12"),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            var module = await provider.TryGetModuleAsync("test/module1");
            module.Should().NotBeNull();
            module!.GetCachedVersions().Should().BeEmpty();
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(0);
            module.GetCachedVersions().Should().BeEmpty();

            var versions = await module!.TryGetVersionsAsync();
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);
            versions[0].Version.Should().Be("v1");
            module.GetCachedVersions()[0].Version.Should().Be("v1");

            var versions2 = await module!.TryGetVersionsAsync();
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);
            versions[0].Version.Should().Be("v1");
            module.GetCachedVersions()[0].Version.Should().Be("v1");
            versions.Should().BeEquivalentTo(versions2);
            versions[0].Version.Should().BeSameAs(versions2[0].Version);
        }

        [TestMethod]
        public async Task GetDetails_ShouldAlsoCacheVersions_BecauseGettingModuleDetailsRequiresGettingVersionDetails()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "metadata description = 'this is module 1 version 1'\nparam p1 bool", WithSource: true, DocumentationUri: "http://contoso.com/help11"),
                    new("br:registry.contoso.io/test/module2:v1", "metadata description = 'this is module 2 version 1'\nparam p2 string", WithSource: true, DocumentationUri: "http://contoso.com/help21"),
                    new("br:registry.contoso.io/test/module1:v2", "metadata description = 'this is module 1 version 2'\nparam p12 string", WithSource: false, DocumentationUri: "http://contoso.com/help12"),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            var module = await provider.TryGetModuleAsync("test/module1");
            module.Should().NotBeNull();
            module!.GetCachedVersions().Should().BeEmpty();

            var details = await module!.TryGetDetailsAsync();
            details.Description.Should().Be("this is module 1 version 2");
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);

            var versions = await module!.TryGetVersionsAsync();
            containerClient.CallsToGetAllManifestPropertiesAsync.Should().Be(1);
        }

        [TestMethod]
        public async Task ModuleWithNoVersionsThatHaveDetails()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "metadata hello = 'this is module 1 version 1'\nparam p1 bool", WithSource: true),
                    new("br:registry.contoso.io/test/module2:v1", "metadata hello = 'this is module 2 version 1'\nparam p2 string", WithSource: true),
                    new("br:registry.contoso.io/test/module1:v2", "metadata hello = 'this is module 1 version 2'\nparam p12 string", WithSource: false),
                ]);

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            var module = await provider.TryGetModuleAsync("test/module1");
            module.Should().NotBeNull();
            module!.GetCachedVersions().Should().BeEmpty();

            var details = await module!.TryGetDetailsAsync();
            details.Description.Should().BeNull();
            details.DocumentationUri.Should().BeNull();
        }
    }
}
