// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Moq;
using RichardSzalay.MockHttp;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Bicep.Core.UnitTests.Registry.Catalog
{
    [TestClass]
    public class PrivateAcrModuleMetadataProviderTests
    {
        [NotNull] public TestContext? TestContext { get; set; }

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
                    new("br:registry.contoso.io/test/module2:v1", "param p2 string", WithSource: true),
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
                    new("br:registry.contoso.io/test/module1:v2", "param p12 string", WithSource: false),
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
        public async Task Module_WithNoVersionsHavingDetails()
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

        [TestMethod]
        public async Task NoVersionsWhichAreValidBicepModules()
        {
            var containerClient = new FakeContainerRegistryClient();
            var fileSystem = new MockFileSystem();

            var registry = "registry.contoso.io";
            var repositoryPath = $"test";
            var repositoryNames = new[] { "repo1", "repo2" };

            var clientFactory = RegistryHelper.CreateMockRegistryClient([.. repositoryNames.Select(name => new RepoDescriptor(registry, $"{repositoryPath}/{name}", ["v1"]))]);

            var services = new ServiceBuilder()
                .WithContainerRegistryClientFactory(clientFactory);

            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var configurationManager = new ConfigurationManager(fileExplorer);
            var featureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager, fileExplorer), BicepTestConstants.FeatureOverrides);

            await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "br:registry.contoso.io/test/repo1:v1", new BinaryData(""));
            await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "br:registry.contoso.io/test/repo1:v2", new BinaryData(""));

            var provider = new PrivateAcrModuleMetadataProvider(
                    BicepTestConstants.BuiltInConfiguration.Cloud,
                    "registry.contoso.io",
                    clientFactory);

            var module = await provider.TryGetModuleAsync("test/repo1");
            module.Should().NotBeNull();
            module!.GetCachedVersions().Should().BeEmpty();

            var details = await module!.TryGetDetailsAsync();
            details.Description.Should().Be("Not a valid Bicep module. Found artifact type application/vnd.ms.bicep.provider.artifact");
            details.DocumentationUri.Should().BeNull();
        }

        [TestMethod]
        public async Task IgnoreVersionsWhichAreNotValidBicepModules()
        {
            var containerClient = new FakeContainerRegistryClient();
            var fileSystem = new MockFileSystem();

            var registry = "registry.contoso.io";
            var repositoryPath = $"test";
            var repositoryNames = new[] { "repo1" };

            var clientFactory = RegistryHelper.CreateMockRegistryClient([..
                repositoryNames.Select(name => new RepoDescriptor(registry, $"{repositoryPath}/{name}", ["v1", "v2", "v3"]))]);

            var services = new ServiceBuilder()
                .WithContainerRegistryClientFactory(clientFactory);

            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var configurationManager = new ConfigurationManager(fileExplorer);
            var featureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager, fileExplorer), BicepTestConstants.FeatureOverrides);

            // Only v2 is a module
            await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "br:registry.contoso.io/test/repo1:v1", new BinaryData(""));
            await RegistryHelper.PublishModuleToRegistryAsync(services, clientFactory, fileSystem, new ModuleToPublish("br:registry.contoso.io/test/repo1:v2", "metadata description = 'this is module 1 version 2'", WithSource: true, "https://docs/m1v2"));
            await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "br:registry.contoso.io/test/repo1:v3", new BinaryData(""));

            var provider = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            var module = await provider.TryGetModuleAsync("test/repo1");
            module.Should().NotBeNull();
            module!.GetCachedVersions().Should().BeEmpty();

            // Version details
            var versions = await module.TryGetVersionsAsync();
            versions.Select(v => (v.Version, v.IsBicepModule, v.Details)).Should().BeEquivalentTo([
                ("v1", false, new RegistryMetadataDetails("Not a valid Bicep module. Found artifact type application/vnd.ms.bicep.provider.artifact", null)),
                ("v2", true, new RegistryMetadataDetails("this is module 1 version 2", "https://docs/m1v2")),
                ("v3", false, new RegistryMetadataDetails("Not a valid Bicep module. Found artifact type application/vnd.ms.bicep.provider.artifact", null)),
            ]);

            // Module details (should pull from v2 since that's the only valid Bicep module)
            var details = await module!.TryGetDetailsAsync();
            details.Description.Should().Be("this is module 1 version 2");
            details.DocumentationUri.Should().Be("https://docs/m1v2");
        }

        class PrivateAcrModuleMetadataProviderThatThrows : PrivateAcrModuleMetadataProvider
        {
            public PrivateAcrModuleMetadataProviderThatThrows(
                CloudConfiguration cloud,
                string registry,
                IContainerRegistryClientFactory clientFactory)
                : base(cloud, registry, clientFactory) { }

            protected override Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
            {
                throw new Exception("I like throwing");
            }
        }

        [TestMethod]
        public async Task GetCachedVersions_ShouldNotThrowOnErrors()
        {
            var containerClient = new FakeContainerRegistryClient();
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                new MockFileSystem(),
                containerClient,
                [
                    new("br:registry.contoso.io/test/module1:v1", "metadata description = 'this is module 1 version 1'\nparam p1 bool", WithSource: true, DocumentationUri: "http://contoso.com/help11"),
                ]);

            var provider = new PrivateAcrModuleMetadataProviderThatThrows(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);

            var module = await provider.TryGetModuleAsync("test/module1");
            module.Should().NotBeNull();

            module!.GetCachedVersions().Should().BeEmpty();
            (await module!.TryGetVersionsAsync()).Should().BeEmpty();
            module.GetCachedVersions().Should().BeEmpty();

            var provider2NoThrow = new PrivateAcrModuleMetadataProvider(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                "registry.contoso.io",
                clientFactory);
            var module2 = await provider2NoThrow.TryGetModuleAsync("test/module1");

            module2!.GetCachedVersions().Should().BeEmpty();
            (await module2!.TryGetVersionsAsync()).Should().NotBeEmpty();
            module2.GetCachedVersions().Should().NotBeEmpty();
        }
    }
}
