// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using TestEnvironment = Bicep.Core.UnitTests.Utils.TestEnvironment;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class LintCommandTests : TestBase
{
    [TestMethod]
    public async Task Lint_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
    {
        var (output, error, result) = await Bicep("lint");

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Contain($"Either the input file path or the --pattern parameter must be specified");
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
            error.Should().Contain($@"The specified input ""{Path.GetFullPath("/dev/zero")}"" was not recognized as a Bicep or Bicep Parameters file. Valid files must either the .bicep or .bicepparam extension");
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(GetValidDataSetsWithoutWarnings), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
    public async Task Lint_Valid_SingleFile_WithTemplateSpecReference_ShouldSucceed(DataSet dataSet)
    {
        var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
        var features = new FeatureProviderOverrides(TestContext);
        FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();

        var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides(features));
        await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

        var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

        var settings = new InvocationSettings(features).WithArtifactManager(artifactManager, TestContext);
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

        var client = new FakeRegistryBlobClient();

        var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
        clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), registryUri, repository)).Returns(client);

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
        var diagnostics = await GetAllDiagnostics(bicepFilePath, InvocationSettings.Default.ClientFactory, InvocationSettings.Default.TemplateSpecRepositoryFactory, InvocationSettings.Default.ModuleMetadataClient);

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
        var settings = new InvocationSettings() { ModuleMetadataClient = PublicModuleIndexHttpClientMocks.Create([]).Object };

        var (output, error, result) = await Bicep(settings, "lint", inputFile);

        result.Should().Be(1);
        output.Should().BeEmpty();
        error.Should().StartWith($"{inputFile}(1,1) : Error BCP271: Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.");
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Lint_should_compile_files_matching_pattern(bool useRootPath)
    {
        var contents = """
output myOutput string = 'hello!'
""";

        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var fileResults = new[]
        {
            (input: "file1.bicep", expectOutput: true),
            (input: "file2.bicep", expectOutput: true),
            (input: "nofile.bicep", expectOutput: false)
        };

        foreach (var (input, _) in fileResults)
        {
            FileHelper.SaveResultFile(TestContext, input, contents, outputPath);
        }

        var (output, error, result) = await Bicep(
            services => services.WithEnvironment(useRootPath ? TestEnvironment.Default : TestEnvironment.Default with { CurrentDirectory = outputPath }),
            ["lint",
                "--pattern",
                useRootPath ? $"{outputPath}/file*.bicep" : "file*.bicep"]);

        result.Should().Be(0);
        error.Should().BeEmpty();
        output.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Lint_with_warnings_should_log_warnings_and_return_0_exit_code()
    {
        var inputFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", $@"
@minValue(1)
@maxValue(50)
param notUsedParam int = 3
");

        var (output, error, result) = await Bicep("lint", inputFile);

        result.Should().Be(0);
        output.Should().BeEmpty();
        error.Should().StartWith($"{inputFile}(4,7) : Warning no-unused-params: Parameter \"notUsedParam\" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-params]");
    }

    [TestMethod]
    public async Task Lint_bicepparam_with_warnings_should_log_warnings_and_return_0_exit_code()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", @"
@minValue(1)
@maxValue(50)
param notUsedParam int
", outputPath);
        var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", @"
using 'main.bicep'
param notUsedParam = 3
", outputPath);

        var (output, error, result) = await Bicep("lint", inputFile);

        result.Should().Be(0);
        output.Should().BeEmpty();
        error.Should().StartWith($"{bicepFile}(4,7) : Warning no-unused-params: Parameter \"notUsedParam\" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-params]");
    }

    [TestMethod]
    public async Task Lint_bicepparam_with_error_should_fail()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", @"
@minValue(1)
@maxValue(50)
param notUsedParam int
", outputPath);
        var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", @"
using 'main.bicep'
param notUsedParm = 'string'
", outputPath);

        var (output, error, result) = await Bicep("lint", inputFile);

        result.Should().Be(1);
        output.Should().BeEmpty();
        error.Should().Contain($"{bicepFile}(4,7) : Warning no-unused-params: Parameter \"notUsedParam\" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-params]");
        error.Should().Contain($"{inputFile}(2,7) : Error BCP258: The following parameters are declared in the Bicep file but are missing an assignment in the params file: \"notUsedParam\".");
        error.Should().Contain($"{inputFile}(3,1) : Error BCP259: The parameter \"notUsedParm\" is assigned in the params file without being declared in the Bicep file.");
    }

    [TestMethod]
    public async Task Lint_with_sarif_diagnostics_format_should_output_valid_sarif()
    {
        var inputFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", @"param storageAccountName string = 'test'");

        var (output, error, result) = await Bicep("lint", inputFile, "--diagnostics-format", "sarif");

        result.Should().Be(0);
        error.Should().BeEmpty();
        var sarifLog = output.FromJson<SarifLog>();
        sarifLog.Runs[0].Results[0].RuleId.Should().Be("no-unused-params");
        sarifLog.Runs[0].Results[0].Message.Text.Should().Contain("is declared but never used");
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
