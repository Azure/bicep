// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Data;
using System.Reflection;
using System.Text.Json;
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
using static Bicep.Core.UnitTests.Utils.RegistryHelper;
using DataSet = Bicep.Core.Samples.DataSet;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class PublishCommandTests : TestBase
    {
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
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentation-uri");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentation-uri parameter expects an argument.");
        }

        [TestMethod]
        public async Task Publish_WithDocumentationUriSpecifiedMoreThanOnce_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'");
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentation-uri", "https://example.com", "--documentation-uri", "https://example.com");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentation-uri parameter cannot be specified more than once.");
        }

        [TestMethod]
        public async Task Publish_WithInvalidDocumentUri_ShouldProduceExpectedError()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'");
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentation-uri", "invalid_uri");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentation-uri should be a well formed uri string.");
        }

        // TODO: Enable this once Azure CLI is updated to support the new parameters.
        [TestMethod]
        public async Task Publish_WithDeprecatedParameter_PrintsDeprecationMessage()
        {
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'");
            var (output, error, result) = await Bicep(settings, "publish", bicepPath, "--target", "br:example.azurecr.io/hello/there:v1", "--documentationUri", "invalid_uri");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --documentationUri should be a well formed uri string.");
            error.Should().MatchRegex(@"DEPRECATED: The parameter --documentationUri is deprecated and will be removed in a future version of Bicep CLI. Use --documentation-uri instead.");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSetsWithDocUriAndPublishSource), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetTestDisplayName))]
        public async Task Publish_AllValidDataSets_ShouldSucceed(string testName, DataSet dataSet, string documentationUri, bool publishSource)
        {
            TestContext.WriteLine(testName);

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients(new RepoDescriptor(registryStr, repository, ["tag"]));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (FakeRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration.Cloud, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            List<string> requiredArgs = new() { "publish", bicepFilePath, "--target", $"br:{registryStr}/{repository}:v1" };

            if (!string.IsNullOrWhiteSpace(documentationUri))
            {
                requiredArgs.AddRange(new List<string> { "--documentation-uri", documentationUri });
            }
            if (publishSource)
            {
                requiredArgs.Add("--with-source");
            }

            string[] args = [.. requiredArgs];

            var (output, error, result) = await Bicep(settings, args);
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);
            var expectedCompiledPayload = BinaryData.FromStream(expectedCompiledStream);

            // verify the module was published
            testClient.Should().OnlyHaveModule("v1", expectedCompiledPayload);
            if (publishSource)
            {
                testClient.Should().HaveModuleWithSource("v1", expectedCompiledPayload);
            }
            else
            {
                testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledPayload);
            }

            // Modify the source
            File.AppendAllText(bicepFilePath, "\noutput newoutput string = 'hello'");

            // publish the same content again without --force
            var (output2, error2, result2) = await Bicep(settings, args);
            result2.Should().Be(1);
            output2.Should().BeEmpty();
            error2.Should().MatchRegex($"The module \"br:{registryStr}/{repository}:v1\" already exists in registry\\. Use --force to overwrite the existing module\\.");

            testClient.Should().OnlyHaveModule("v1", expectedCompiledPayload);
            if (publishSource)
            {
                testClient.Should().HaveModuleWithSource("v1", expectedCompiledPayload);
            }
            else
            {
                testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledPayload);
            }

            // publish the same content again with --force
            requiredArgs.Add("--force");
            var (output3, error3, result3) = await Bicep(settings, [.. requiredArgs]);
            result3.Should().Be(0);
            output3.Should().BeEmpty();
            AssertNoErrors(error3);

            // compile to get what the new expected main.json should be
            List<string> buildArgs = new() { "build", bicepFilePath, "--outfile", $"{compiledFilePath}.modified" };
            var (output4, error4, result4) = await Bicep(settings, [.. buildArgs]);
            result4.Should().Be(0);
            output4.Should().BeEmpty();
            AssertNoErrors(error4);
            using var expectedModifiedCompiledStream = new FileStream($"{compiledFilePath}.modified", FileMode.Open, FileAccess.Read);
            var expectedModifiedCompiledPayload = BinaryData.FromStream(expectedModifiedCompiledStream);

            // we should still only have 1 module
            testClient.Should().OnlyHaveReachableModule("v1", expectedModifiedCompiledPayload);
            if (publishSource)
            {
                testClient.Should().HaveModuleWithSource("v1", expectedModifiedCompiledPayload);
            }
            else
            {
                testClient.Should().HaveModuleWithNoSource("v1", expectedModifiedCompiledPayload);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Publish_ValidArmTemplateFile_AllValidDataSets_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients(new RepoDescriptor(registryStr, repository, ["tag"]));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (FakeRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration.Cloud, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1");
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);

            using var expectedCompiledStream = new FileStream(compiledFilePath, FileMode.Open, FileAccess.Read);
            var expectedCompiledPayload = BinaryData.FromStream(expectedCompiledStream);

            // verify the module was published
            testClient.Should().OnlyHaveModule("v1", expectedCompiledPayload);

            // There are no Bicep sources, it's only an ARM template being published, so even if published with sources, there should be no sources
            testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledPayload);

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
            testClient.Should().OnlyHaveReachableModule("v1", expectedCompiledPayload);

            // There are no Bicep sources, it's only an ARM template being published, so even if published with sources, there should be no sources
            testClient.Should().HaveModuleWithNoSource("v1", expectedCompiledPayload);
        }

        [TestMethod]
        public async Task Publish_ValidArmTemplateFile_WithSource_ShouldFail()
        {
            var dataSet = DataSets.AllDataSets.Where(ds => ds.IsValid).First();
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var registryStr = "example.com";
            var registryUri = new Uri($"https://{registryStr}");
            var repository = $"test/{dataSet.Name}".ToLowerInvariant();

            var clientFactory = dataSet.CreateMockRegistryClients(new RepoDescriptor(registryStr, repository, ["tag"]));
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            // mock client factory caches the clients
            var testClient = (FakeRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration.Cloud, registryUri, repository);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, templateSpecRepositoryFactory);

            var args = new List<string> { "publish", compiledFilePath, "--target", $"br:{registryStr}/{repository}:v1", "--with-source" };
            var (output, error, result) = await Bicep(settings, [.. args]);
            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex("Cannot publish with source when the target is an ARM template file.");
        }

        [TestMethod]
        public async Task Publish_RequestFailedException_ShouldFail()
        {
            var dataSet = DataSets.Empty;
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);

            var client = StrictMock.Of<ContainerRegistryContentClient>();
            client
                .Setup(m => m.UploadBlobAsync(It.IsAny<BinaryData>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Mock registry request failure."));
            client
                .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "Module not found."));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), new Uri("https://fake"), "fake"))
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

            var client = StrictMock.Of<ContainerRegistryContentClient>();
            client
                .Setup(m => m.UploadBlobAsync(It.IsAny<BinaryData>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateException(new RequestFailedException("Mock registry request failure 1."), new RequestFailedException("Mock registry request failure 2.")));
            client
                .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "Module not found."));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), new Uri("https://fake"), "fake"))
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

            var clientFactory = RegistryHelper.CreateMockRegistryClient(new RepoDescriptor(registryStr, repository, ["v1"]));
            var blobClient = (FakeRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration.Cloud, registryUri, repository);

            await RegistryHelper.PublishModuleToRegistryAsync(
                new ServiceBuilder(),
                clientFactory,
                BicepTestConstants.FileSystem,
                new($"br:example.com/test/{moduleName}:v1", bicepModuleContents, WithSource: false, documentationUri));

            var manifest = blobClient.Manifests.Single().Value.ToObjectFromJson<OciManifest>(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            manifest.Should().NotBeNull();

            if (expectedDescription is null)
            {
                manifest!.Annotations.Keys.Should().NotContain("org.opencontainers.image.description");
            }
            else
            {
                manifest!.Annotations.Should().ContainKey("org.opencontainers.image.description");
                manifest.Annotations["org.opencontainers.image.description"].Should().Be(expectedDescription);
            }

            if (documentationUri is null)
            {
                manifest.Annotations.Keys.Should().NotContain("org.opencontainers.image.documentation");
            }
            else
            {
                manifest.Annotations.Should().ContainKey("org.opencontainers.image.documentation");
                manifest.Annotations["org.opencontainers.image.documentation"].Should().Be(documentationUri);
            }
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == true)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetValidDataSetsWithDocUriAndPublishSource()
        {
            foreach (var ds in DataSets.AllDataSets.Where(ds => ds.IsValid))
            {
                yield return new object[] { $"{ds.Name}, without docUri, not publishing source", ds, "", false };
                yield return new object[] { $"{ds.Name}, with docUri, publishing source", ds, "https://example.com", true };
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
