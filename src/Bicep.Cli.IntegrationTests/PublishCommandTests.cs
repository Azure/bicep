// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry.Specialized;
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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSet = Bicep.Core.Samples.DataSet;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class PublishCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Publish_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("publish");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [TestMethod]
        public async Task Publish_MissingTarget_ShouldProduceExpectedError()
        {
            var (output, error, result) = await Bicep("publish", "WrongFile.bicep");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain("The target module was not specified.");
        }

        [TestMethod]
        public async Task Publish_InvalidTarget_ShouldProduceExpectedError()
        {
            var (output, error, result) = await Bicep("publish", "WrongFile.bicep", "--target", "fake:");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain("The specified module reference scheme \"fake\" is not recognized.");
        }

        [TestMethod]
        public async Task Publish_InvalidInputFile_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var (output, error, result) = await Bicep(settings, "publish", "WrongFile.bicep", "--target", "br:example.azurecr.io/hello/there:v1");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"An error occurred reading file\. Could not find file '.+WrongFile.bicep'\.");
        }

        [TestMethod]
        public async Task Publish_LocalTarget_ShouldProduceExpectedError()
        {
            var (output, error, result) = await Bicep("publish", "WrongFile.bicep", "--target", "./test.bicep");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain("The specified module target \"./test.bicep\" is not supported.");
        }

        [TestMethod]
        public async Task Publish_OciDigestTarget_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var (output, error, result) = await Bicep(settings, "publish", "WrongFile.bicep", "--target", "br:example.com/test@sha256:80f63ced0b80b63874c808a321f472755a3c9e93987d1fa0a51e13c65e4a52b9");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain("The specified module target \"br:example.com/test@sha256:80f63ced0b80b63874c808a321f472755a3c9e93987d1fa0a51e13c65e4a52b9\" is not supported.");
        }

        [TestMethod]
        public async Task Publish_WithMissingDocumentationUri_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'");
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentationUri");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentationUri parameter expects an argument.");
        }

        [TestMethod]
        public async Task Publish_WithDocumentationUriSpecifiedMoreThanOnce_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'");
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentationUri", "https://example.com", "--documentationUri", "https://example.com");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentationUri parameter cannot be specified more than once.");
        }

        [TestMethod]
        public async Task Publish_WithInvalidDocumentUri_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'");
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentationUri", "invalid_uri");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentationUri should be a well formed uri string.");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSetsWithDocumentationUri), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_ValidFile_ShouldSucceed(DataSet dataSet, string documentationUri)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients((registryUri, repository));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (MockRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            List<string> requiredArgs = new List<string> { "publish", bicepFilePath, "--target", $"br:{registryStr}/{repository}:v1" };

            if (!string.IsNullOrWhiteSpace(documentationUri))
            {
                requiredArgs.AddRange(new List<string> { "--documentationUri", documentationUri });
            }

            string[] args = requiredArgs.ToArray();

            var (output, error, result) = await Bicep(settings, args);
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);

            // verify the module was published
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);

            // publish the same content again
            var (output2, error2, result2) = await Bicep(settings, args);
            result2.Should().Be(0);
            output2.Should().BeEmpty();
            AssertNoErrors(error2);

            // we should still only have 1 module
            expectedCompiledStream.Position = 0;
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_ValidArmTemplateFile_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients((registryUri, repository));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (MockRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);

            // verify the module was published
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);

            // publish the same content again
            var (output2, error2, result2) = await Bicep(settings, "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result2.Should().Be(0);
            output2.Should().BeEmpty();
            AssertNoErrors(error2);

            // we should still only have 1 module
            expectedCompiledStream.Position = 0;
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);
        }

        [TestMethod]
        public async Task Publish_RequestFailedException_ShouldFail()
        {
            var dataSet = DataSets.Empty;
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            var client = StrictMock.Of<ContainerRegistryBlobClient>();
            client
                .Setup(m => m.UploadBlobAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Mock registry request failure."));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), new Uri("https://fake"), "fake"))
                .Returns(client.Object);

            var templateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>();

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, templateSpecRepositoryFactory.Object);
            var (output, error, result) = await Bicep(settings, "publish", compiledFilePath, "--target", "br:fake/fake:v1");
            using (new AssertionScope())
            {
                error.Should().StartWith("Unable to publish module \"br:fake/fake:v1\": Mock registry request failure.");
                output.Should().BeEmpty();
                result.Should().Be(1);
            }
        }

        [TestMethod]
        public async Task Publish_AggregateExceptionWithInnerRequestFailedExceptions_ShouldFail()
        {
            var dataSet = DataSets.Empty;
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            var client = StrictMock.Of<ContainerRegistryBlobClient>();
            client
                .Setup(m => m.UploadBlobAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateException(new RequestFailedException("Mock registry request failure 1."), new RequestFailedException("Mock registry request failure 2.")));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), new Uri("https://fake"), "fake"))
                .Returns(client.Object);

            var templateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>();

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, templateSpecRepositoryFactory.Object);
            var (output, error, result) = await Bicep(settings, "publish", compiledFilePath, "--target", "br:fake/fake:v1");
            using (new AssertionScope())
            {
                error.Should().StartWith("Unable to publish module \"br:fake/fake:v1\": One or more errors occurred. (Mock registry request failure 1.) (Mock registry request failure 2.)");
                output.Should().BeEmpty();
                result.Should().Be(1);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_InvalidFile_ShouldFail_WithExpectedErrorMessage(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            // publish won't actually happen, so broken client factory is fine
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var diagnostics = await GetAllDiagnostics(bicepFilePath, settings.ClientFactory, settings.TemplateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "publish", bicepFilePath, "--target", $"br:example.com/fail/{dataSet.Name.ToLowerInvariant()}:v1");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().ContainAll(diagnostics);
            }
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetValidDataSetsWithDocumentationUri()
        {
            var validDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid);

            foreach (DataSet dataSet in validDataSets)
            {
                yield return new object[] { dataSet, "" };
                yield return new object[] { dataSet, "https://example.com" };
            }
        }

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();
    }
}
