// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
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
using DataSet = Bicep.Core.Samples.DataSet;
using Bicep.Core.UnitTests.TypeSystem.Az;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class PublishTypeCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        [DynamicData(nameof(OnlyPublishTypeDataSets), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetTestDisplayName))]
        public async Task Publish_AllValidDataSets_ShouldSucceed(string testName, DataSet dataSet)
        {
            TestContext.WriteLine(testName);

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClientsForTypes((registryUri, repository));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            //Why are we publishing something?
/*            await dataSet.PublishTypesToRegistryAsync(clientFactory);*/

            var indexPath = Path.Combine(outputDirectory, DataSet.TestIndex);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestIndex);

            // mock client factory caches the clients
            var testClient = (MockRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            List<string> requiredArgs = new() { "publish-type", indexPath, "--target", $"br:{registryStr}/{repository}:v1" };

            string[] args = requiredArgs.ToArray();

            var (output, error, result) = await Bicep(settings, args);
            result.Should().Be(0);
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);

            // verify the module was published
            testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledStream);

            // Modify the source
            File.AppendAllText(indexPath, "\noutput newoutput string = 'hello'");

            // publish the same content again without --force
            var (output2, error2, result2) = await Bicep(settings, args);
            result2.Should().Be(1);
            error2.Should().MatchRegex($"The module \"br:{registryStr}/{repository}:v1\" already exists in registry\\. Use --force to overwrite the existing module\\.");

            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);
            testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledStream);

            // publish the same content again with --force
            requiredArgs.Add("--force");
            var (output3, error3, result3) = await Bicep(settings, requiredArgs.ToArray());
            result3.Should().Be(0);
            AssertNoErrors(error3);

            // compile to get what the new expected main.json should be
            List<string> buildArgs = new() { "build", indexPath, "--outfile", $"{compiledFilePath}.modified" };
            var (output4, error4, result4) = await Bicep(settings, buildArgs.ToArray());
            result4.Should().Be(0);
            output4.Should().BeEmpty();
            AssertNoErrors(error4);
            using var expectedModifiedCompiledStream = new FileStream($"{compiledFilePath}.modified", FileMode.Open, FileAccess.Read);

            // we should still only have 1 module
            testClient.Should().OnlyHaveReachableModule("v1", expectedModifiedCompiledStream);
            testClient.Should().HaveModuleWithNoSource("v1", expectedModifiedCompiledStream);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_ValidArmTemplateFile_AllValidDataSets_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients(enablePublishSource: false, (registryUri, repository));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (MockRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true, PublishSourceEnabled: true), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);

            // verify the module was published
            testClient.Should().OnlyHaveModule("v1", expectedCompiledStream);

            // There are no Bicep sources, it's only an ARM template being published, so even if published with sources, there should be no sources
            testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledStream);

            // publish the same content again without --force
            var (output2, error2, result2) = await Bicep(settings, "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result2.Should().Be(1);
            output2.Should().BeEmpty();
            error2.Should().MatchRegex($"The module \"br:{registryStr}/{repository}:v1\" already exists in registry\\. Use --force to overwrite the existing module\\.");

            // publish the same content again with --force
            var (output3, error3, result3) = await Bicep(settings, "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1", "--force");
            result3.Should().Be(0);
            output3.Should().BeEmpty();
            AssertNoErrors(error3);

            // we should still only have 1 reachable module. (The old module will still exist because it has a timestamp and therefore a
            // different digest, but is not in the tags list. It could still be reached via digest until cleaned up.)
            expectedCompiledStream.Position = 0;
            testClient.Should().OnlyHaveReachableModule("v1", expectedCompiledStream);

            // There are no Bicep sources, it's only an ARM template being published, so even if published with sources, there should be no sources
            testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledStream);
        }

        [DataTestMethod]
        [DataRow(
            null,
            "param description string",
            null,
            DisplayName = "manifest: neither description nor uri"
            )]
        [DataRow(
            "mydocs.org/abc",
            "metadata description = 'my description'",
            "my description",
            DisplayName = "manifest: description and uri"
            )]
        [DataRow(
            null,
            "metadata description = 'my description'",
            "my description",
            DisplayName = "manifest: just description"
            )]
        [DataRow(
            "mydocs.org/abc",
            "",
            null,
            DisplayName = "manifest: just uri"
            )]
        [DataRow(
            "mydocs.org/abc",
            "metadata description2 = 'my description'",
            null,
            DisplayName = "manifest: just uri 2"
            )]
        public async Task Publish_BicepModule_WithDescriptionAndDocUri_ShouldPlaceDescriptionInManifest(string? documentationUri, string bicepModuleContents, string? expectedDescription)
        {
            var moduleName = "module1";
            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{moduleName}".ToLowerInvariant();

            var (clientFactory, blobClients) = DataSetsExtensions.CreateMockRegistryClients(false, (registryUri, repository));

            var blobClient = blobClients[(registryUri, repository)];

            await DataSetsExtensions.PublishModuleToRegistryAsync(clientFactory, "modulename", $"br:example.com/test/{moduleName}:v1", bicepModuleContents, publishSource: false, documentationUri);

            var manifest = blobClient.Manifests.Single().Value.Text;

            if (expectedDescription is null)
            {
                manifest.Should().NotMatchRegex($@"org.opencontainers.image.description");
            }
            else
            {
                manifest.Should().MatchRegex($@"""org.opencontainers.image.description"": ""{expectedDescription}""");
            }

            if (documentationUri is null)
            {
                manifest.Should().NotMatchRegex($@"org.opencontainers.image.documentation");
            }
            else
            {
                manifest.Should().MatchRegex($@"""org.opencontainers.image.documentation"": ""{documentationUri}""");
            }
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == true)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetValidDataSetsWithPublishSources()
        {
            foreach (var ds in DataSets.AllDataSets.Where(ds => ds.IsValid))
            {
                yield return new object[] { $"{ds.Name}, not publishing source", ds, false };
                yield return new object[] { $"{ds.Name}, publishing source", ds, true };
            }
        }

        private static IEnumerable<object[]> GetValidDataSetsWithDocUriAndPublishSource()
        {
            foreach (var ds in DataSets.AllDataSets.Where(ds => ds.IsValid))
            {
                yield return new object[] { $"{ds.Name}, without docUri, not publishing source", ds, "", false };
                yield return new object[] { $"{ds.Name}, with docUri, publishing source", ds, "https://example.com", true };
            }
        }

        private static IEnumerable<object[]> OnlyPublishTypeDataSets()
        {
            foreach (var ds in DataSets.AllDataSets.Where(ds => ds.Name == "Publish_Types"))
            {
                yield return new object[] { $"{ds.Name}, publishing types", ds };
                break;
            }
        }

        public static string GetTestDisplayName(MethodInfo _, object[] objects)
        {
            return (string)objects[0];
        }

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();
    }
}
