// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class LintCommandTests : TestBase
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task Lint_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
    {
        var (output, error, result) = await Bicep("lint");

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Contain($"The input file path was not specified");
        }
    }

    [TestMethod]
    public async Task Lint_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
    {
        var (output, error, result) = await Bicep("lint", "/dev/zero");

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Contain($@"The specified input ""/dev/zero"" was not recognized as a Bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(GetValidDataSetsWithoutWarnings), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
    public async Task Lint_Valid_SingleFile_WithTemplateSpecReference_ShouldSucceed(DataSet dataSet)
    {
        var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
        var clientFactory = dataSet.CreateMockRegistryClients().Object;
        var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
        await dataSet.PublishModulesToRegistryAsync(clientFactory);
        var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

        var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);
        var (output, error, result) = await Bicep(settings, "lint", bicepFilePath);

        using (new AssertionScope())
        {
            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);
        }
    }

    [TestMethod]
    public async Task Lint_Valid_SingleFile_WithDigestReference_ShouldSucceed()
    {
        var registry = "example.com";
        var registryUri = new Uri("https://" + registry);
        var repository = "hello/there";

        var client = new MockRegistryBlobClient();

        var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
        clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

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

        var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
        File.WriteAllText(bicepFilePath, bicep);

        var (output, error, result) = await Bicep(settings, "lint", bicepFilePath);
        using (new AssertionScope())
        {
            result.Should().Be(0);
            output.Should().BeEmpty();
            error.Should().BeEmpty();
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
    public async Task Lint_Invalid_SingleFile_ShouldFail_WithExpectedErrorMessage(DataSet dataSet)
    {
        var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
        var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
        var defaultSettings = CreateDefaultSettings();
        var diagnostics = await GetAllDiagnostics(bicepFilePath, defaultSettings.ClientFactory, defaultSettings.TemplateSpecRepositoryFactory);

        var (output, error, result) = await Bicep("lint", bicepFilePath);

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().ContainAll(diagnostics);
        }
    }

    [TestMethod]
    public async Task Lint_WithEmptyBicepConfig_ShouldProduceConfigurationError()
    {
        string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicep", DataSets.Empty.Bicep, testOutputPath);
        var configurationPath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", string.Empty, testOutputPath);

        var (output, error, result) = await Bicep("lint", inputFile);

        result.Should().Be(1);
        output.Should().BeEmpty();
        error.Should().StartWith($"{inputFile}(1,1) : Error BCP271: Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: \"The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.\".");
    }

    [TestMethod]
    public async Task Lint_Invalid_WithIgnoreWarningsFlag_ShouldSucceed()
    {
        var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        Directory.CreateDirectory(tempDirectory);

        var bicep = $@"
@minValue(1)
@maxValue(50)
param notUsedParam int = 3
";

        var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
        File.WriteAllText(bicepFilePath, bicep);

        var (_, _, result) = await Bicep("lint", "--ignore-warnings", bicepFilePath);
        using (new AssertionScope())
        {
            result.Should().Be(0);
        }
    }

    private static IEnumerable<object[]> GetValidDataSetsWithoutWarnings() => DataSets
        .AllDataSets
        .Where(ds => ds.IsValid)
        .Where(ds => ds.Name is "Functions_LF" or "ModulesWithScopes_LF")
        .ToDynamicTestData();

    private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
        .AllDataSets
        .Where(ds => ds.IsValid == false)
        .ToDynamicTestData();
}
