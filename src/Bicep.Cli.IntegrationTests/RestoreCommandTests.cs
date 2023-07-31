// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Azure;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Microsoft.WindowsAzure.ResourceStack.Common.Memory;
using System.Text;
using Bicep.Core.Emit;
using Azure.Identity;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class RestoreCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Restore_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("restore");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Restore_ShouldSucceed(DataSet dataSet)
        {
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);

            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);
            TestContext.WriteLine($"Cache root = {settings.FeatureOverrides.CacheRootDirectory}");
            var (output, error, result) = await Bicep(settings, "restore", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            if (dataSet.HasExternalModules)
            {
                // ensure something got restored
                settings.FeatureOverrides.Should().HaveValidModules();
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Restore_ShouldSucceedWithAnonymousClient(DataSet dataSet)
        {
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);

            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            // create client that mocks missing az or PS login
            var clientWithCredentialUnavailable = StrictMock.Of<ContainerRegistryContentClient>();
            clientWithCredentialUnavailable
                .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CredentialUnavailableException("Mock credential unavailable exception"));

            // authenticated client creation will produce a client that will fail due to missing login
            // this will force fallback to the anonymous client
            var clientFactoryForRestore = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactoryForRestore
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(clientWithCredentialUnavailable.Object);

            // anonymous client creation will redirect to the working client factory containing mock published modules
            clientFactoryForRestore
                .Setup(m => m.CreateAnonymousBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<RootConfiguration, Uri, string>(clientFactory.CreateAnonymousBlobClient);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactoryForRestore.Object, templateSpecRepositoryFactory);
            TestContext.WriteLine($"Cache root = {settings.FeatureOverrides.CacheRootDirectory}");
            var (output, error, result) = await Bicep(settings, "restore", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            if (dataSet.HasExternalModules)
            {
                // ensure something got restored
                settings.FeatureOverrides.Should().HaveValidModules();
            }
        }

        /// <summary>
        /// Validates that we can restore a module published by an older version of Bicep that did not set artifactType in the OCI manifest.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Restore_ArtifactWithoutArtifactType_ShouldSucceed()
        {
            var registry = "example.com";
            var registryUri = new Uri("https://" + registry);
            var repository = "hello/there";
            var dataSet = DataSets.Empty;

            var client = new FakeRegistryClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

            var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, BicepTestConstants.TemplateSpecRepositoryFactory);

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var containerRegistryManager = new AzureContainerRegistryManager(clientFactory.Object);
            var configuration = BicepTestConstants.BuiltInConfiguration;

            using (var compiledStream = new BufferedMemoryStream())
            {
                OciModuleReference.TryParse(null, $"{registry}/{repository}:v1", configuration, new Uri("file:///main.bicep"), out var moduleReference, out _).Should().BeTrue();

                compiledStream.Write(TemplateEmitter.UTF8EncodingWithoutBom.GetBytes(dataSet.Compiled!));
                compiledStream.Position = 0;

                await containerRegistryManager.PushArtifactAsync(
                    configuration: configuration,
                    moduleReference: moduleReference!,
                    // intentionally setting artifactType to null to simulate a publish done by an older version of Bicep
                    artifactType: null,
                    config: new StreamDescriptor(Stream.Null, BicepMediaTypes.BicepModuleConfigV1),
                    layers: new StreamDescriptor(compiledStream, BicepMediaTypes.BicepModuleLayerV1Json));
            }

            /*
             * TODO: Publish via code
             */

            client.Blobs.Should().HaveCount(2);
            client.Manifests.Should().HaveCount(1);
            client.ManifestTags.Should().HaveCount(1);

            string digest = client.Manifests.Single().Key;

            var bicep = $@"
module empty 'br:{registry}/{repository}@{digest}' = {{
  name: 'empty'
}}
";

            var restoreBicepFilePath = Path.Combine(tempDirectory, "restored.bicep");
            File.WriteAllText(restoreBicepFilePath, bicep);

            var (output, error, result) = await Bicep(settings, "restore", restoreBicepFilePath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task Restore_With_Force_Should_Overwrite_Existing_Cache()
        {
            var registry = "example.com";
            var registryUri = new Uri("https://" + registry);
            var repository = "hello/there";

            var client = new FakeRegistryClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

            var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, BicepTestConstants.TemplateSpecRepositoryFactory);

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var publishedBicepFilePath = Path.Combine(tempDirectory, "module.bicep");
            File.WriteAllText(publishedBicepFilePath,@"
param p1 string
output o1 string = p1");

            var (publishOutput, publishError, publishResult) = await Bicep(settings, "publish", publishedBicepFilePath, "--target", $"br:{registry}/{repository}:v1");
            using (new AssertionScope())
            {
                publishResult.Should().Be(0);
                publishOutput.Should().BeEmpty();
                publishError.Should().BeEmpty();
            }

            client.Blobs.Should().HaveCount(2);
            client.Manifests.Should().HaveCount(1);
            client.ManifestTags.Should().HaveCount(1);

            string digest = client.Manifests.Single().Key;

            var bicep = $@"
module mymodule 'br:{registry}/{repository}:v1' = {{
  name: 'mymodule'
  params: {{
    p1: 'p1'
  }}
}}
";

            var clientBicepPath = Path.Combine(tempDirectory, "client.bicep");
            File.WriteAllText(clientBicepPath, bicep);

            var (output, error, result) = await Bicep(settings, "restore", clientBicepPath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            // Build client.bicep
            (output, error, result) = await Bicep(settings, "build", clientBicepPath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            // Republish with new required parameter
            File.WriteAllText(publishedBicepFilePath,
                @"
param p1 string
param p2 string
output o1 string = '${p1}${p2}'");

            (publishOutput, publishError, publishResult) = await Bicep(settings, "publish", publishedBicepFilePath, "--target", $"br:{registry}/{repository}:v1", "--force");
            using (new AssertionScope())
            {
                publishResult.Should().Be(0);
                publishOutput.Should().BeEmpty();
                publishError.Should().BeEmpty();
            }

            // Restore without force (won't update local cache)
            (output, error, result) = await Bicep(settings, "restore", clientBicepPath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            // Build should still succeed
            (output, error, result) = await Bicep(settings, "build", clientBicepPath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            // Restore with --force
            (output, error, result) = await Bicep(settings, "restore", clientBicepPath, "--force");
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            // Build should fail because of the new required parameter missing
            (output, error, result) = await Bicep(settings, "build", clientBicepPath);
            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().Contain("missing the following required properties: \"p2\"");
            }
        }

        [TestMethod]
        public async Task Restore_ByDigest_ShouldSucceed()
        {
            var registry = "example.com";
            var registryUri = new Uri("https://" + registry);
            var repository = "hello/there";

            var client = new FakeRegistryClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

            var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, BicepTestConstants.TemplateSpecRepositoryFactory);

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var publishedBicepFilePath = Path.Combine(tempDirectory, "published.bicep");
            File.WriteAllText(publishedBicepFilePath, string.Empty);

            var (publishOutput, publishError, publishResult) = await Bicep(settings, "publish", publishedBicepFilePath, "--target", $"br:{registry}/{repository}:v1");
            using (new AssertionScope())
            {
                publishResult.Should().Be(0);
                publishOutput.Should().BeEmpty();
                publishError.Should().BeEmpty();
            }

            client.Blobs.Should().HaveCount(2);
            client.Manifests.Should().HaveCount(1);
            client.ManifestTags.Should().HaveCount(1);

            string digest = client.Manifests.Single().Key;

            var bicep = $@"
module empty 'br:{registry}/{repository}@{digest}' = {{
  name: 'empty'
}}
";

            var restoreBicepFilePath = Path.Combine(tempDirectory, "restored.bicep");
            File.WriteAllText(restoreBicepFilePath, bicep);

            var (output, error, result) = await Bicep(settings, "restore", restoreBicepFilePath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSetsWithExternalModules), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Restore_NonExistentModules_ShouldFail(DataSet dataSet)
        {
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            // do not publish modules to the registry

            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);
            TestContext.WriteLine($"Cache root = {settings.FeatureOverrides.CacheRootDirectory}");
            var (output, error, result) = await Bicep(settings, "restore", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().ContainAll(": Error BCP192: Unable to restore the module with reference ", "The module does not exist in the registry.");
            }
        }

        [TestMethod]
        public async Task Restore_AggregateExceptionWithInnerRequestFailedExceptions_ShouldFail()
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(outputDirectory);
            var compiledFilePath = Path.Combine(outputDirectory, "main.bicep");
            File.WriteAllText(compiledFilePath, @"module foo 'br:fake/fake:v1'");

            var client = StrictMock.Of<ContainerRegistryContentClient>();
            client
                .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateException(new RequestFailedException("Mock registry request failure 1."), new RequestFailedException("Mock registry request failure 2.")));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), new Uri("https://fake"), "fake"))
                .Returns(client.Object);

            var templateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>();

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, templateSpecRepositoryFactory.Object);
            var (output, error, result) = await Bicep(settings, "restore", compiledFilePath);
            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().Contain("main.bicep(1,12) : Error BCP192: Unable to restore the module with reference \"br:fake/fake:v1\": One or more errors occurred. (Mock registry request failure 1.) (Mock registry request failure 2.)");
            }
        }

        [TestMethod]
        public async Task Restore_RequestFailedException_ShouldFail()
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(outputDirectory);
            var compiledFilePath = Path.Combine(outputDirectory, "main.bicep");
            File.WriteAllText(compiledFilePath, @"module foo 'br:fake/fake:v1'");

            var client = StrictMock.Of<ContainerRegistryContentClient>();
            client
                .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Mock registry request failure."));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), new Uri("https://fake"), "fake"))
                .Returns(client.Object);

            var templateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>();

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, templateSpecRepositoryFactory.Object);
            var (output, error, result) = await Bicep(settings, "restore", compiledFilePath);
            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().Contain("main.bicep(1,12) : Error BCP192: Unable to restore the module with reference \"br:fake/fake:v1\": Mock registry request failure.");
            }
        }

        private static IEnumerable<object[]> GetAllDataSets() => DataSets.AllDataSets.ToDynamicTestData();

        private static IEnumerable<object[]> GetValidDataSetsWithExternalModules() => DataSets.AllDataSets.Where(ds => ds.IsValid && ds.HasExternalModules).ToDynamicTestData();
    }
}
