// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Registry;
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
            var settings = new InvocationSettings(BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
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

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_ValidFile_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients(TestContext, (registryUri, repository));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (MockRegistryBlobClient)clientFactory.CreateBlobClient(registryUri, repository, Repository.Create<TokenCredential>().Object);

            var settings = new InvocationSettings(BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "publish", bicepFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);

            // verify the module was published
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);

            // publish the same content again
            var (output2, error2, result2) = await Bicep(settings, "publish", bicepFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result2.Should().Be(0);
            output2.Should().BeEmpty();
            AssertNoErrors(error2);

            // we should still only have 1 module
            expectedCompiledStream.Position = 0;
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_InvalidFile_ShouldFail_WithExpectedErrorMessage(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            // publish won't actually happen, so broken client factory is fine
            var settings = new InvocationSettings(BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var diagnostics = GetAllDiagnostics(bicepFilePath, settings.ClientFactory, settings.TemplateSpecRepositoryFactory);

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

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();
    }
}
