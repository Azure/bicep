// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Bicep.Cli.UnitTests;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class BuildCommandTests : TestBase
    {
        [TestMethod]
        public async Task Build_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("build");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"Either the input file path or the --pattern parameter must be specified");
            }
        }

        [TestMethod]
        public async Task Build_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("build", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($@"The specified input ""/dev/zero"" was not recognized as a Bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_WithTemplateSpecReference_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);
            var (output, error, result) = await Bicep(settings, "build", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            if (dataSet.HasExternalModules)
            {
                // ensure something got restored

                settings.FeatureOverrides!.CacheRootDirectory!.Exists().Should().BeTrue();
                Directory.EnumerateFiles(settings.FeatureOverrides.CacheRootDirectory!.Uri.GetLocalFilePath(), "*.json", SearchOption.AllDirectories).Should().NotBeEmpty();
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var compiledFileContent = File.ReadAllText(compiledFilePath);
            compiledFileContent.Should().OnlyContainLFNewline();

            var actual = JToken.Parse(compiledFileContent);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_WithTemplateSpecReference_ToStdOut_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "build", "--stdout", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                output.Should().OnlyContainLFNewline();
                AssertNoErrors(error);
            }

            if (dataSet.HasExternalModules)
            {
                CachedModules.GetCachedModules(BicepTestConstants.FileSystem, settings.FeatureOverrides!.CacheRootDirectory!).Should().HaveCountGreaterThan(0)
                    .And.AllSatisfy(m => m.Should().HaveSource());
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(output);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSetsWithExternalModules), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_After_Restore_Should_Succeed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);

            var (restoreOutput, restoreError, restoreResult) = await Bicep(settings, "restore", bicepFilePath);
            using (new AssertionScope())
            {
                restoreResult.Should().Be(0);
                restoreOutput.Should().BeEmpty();
                restoreError.Should().BeEmpty();
            }

            // run restore with the same feature settings, so it will use the mock local module cache
            // but break the client to ensure no outgoing calls are made
            var settingsWithBrokenClient = settings with { ClientFactory = Repository.Create<IContainerRegistryClientFactory>().Object };
            var (output, error, result) = await Bicep(settingsWithBrokenClient, "build", "--stdout", "--no-restore", bicepFilePath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(output);
            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [TestMethod]
        public async Task Build_Valid_SingleFile_WithDigestReference_ShouldSucceed()
        {
            var registry = "example.com";
            var registryUri = new Uri("https://" + registry);
            var repository = "hello/there";

            var client = new FakeRegistryBlobClient();

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

            string digest = client.ModuleManifestObjects.Single().Key;

            var bicep = $$"""
module empty 'br:{{registry}}/{{repository}}@{{digest}}' = {
    name: 'empty'
}
""";

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep(settings, "build", bicepFilePath);
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Invalid_SingleFile_ShouldFail_WithExpectedErrorMessage(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var diagnostics = await GetAllDiagnostics(bicepFilePath, InvocationSettings.Default.ClientFactory, InvocationSettings.Default.TemplateSpecRepositoryFactory);

            var (output, error, result) = await Bicep("build", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().ContainAll(diagnostics);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Invalid_SingleFile_ToStdOut_ShouldFail_WithExpectedErrorMessage(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var (output, error, result) = await Bicep("build", "--stdout", bicepFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();

            var diagnostics = await GetAllDiagnostics(bicepFilePath, InvocationSettings.Default.ClientFactory, InvocationSettings.Default.TemplateSpecRepositoryFactory);
            error.Should().ContainAll(diagnostics);
        }

        [TestMethod]
        public async Task Build_WithOutFile_ShouldSucceed()
        {
            var bicepPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicep",
                """
                output myOutput string = 'hello!'
                """);

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var (output, error, result) = await Bicep("build", "--outfile", outputFilePath, bicepPath);

            File.Exists(outputFilePath).Should().BeTrue();
            result.Should().Be(0);
            error.Should().BeEmpty();
            output.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Build_should_compile_files_matching_pattern(bool useRootPath)
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
                ["build",
                ..useRootPath ? new[] {"--pattern-root", outputPath} : [],
                "--pattern", $"file*.bicep"]);

            result.Should().Be(0);
            error.Should().BeEmpty();
            output.Should().BeEmpty();

            foreach (var (input, expectOutput) in fileResults)
            {
                var outputFile = Path.ChangeExtension(input, ".json");
                File.Exists(Path.Combine(outputPath, outputFile)).Should().Be(expectOutput);
            }
        }

        [TestMethod]
        public async Task Build_WithNonExistentOutDir_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicep",
                """
                output myOutput string = 'hello!'
                """);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            var (output, error, result) = await Bicep("build", "--outdir", outputFileDir, bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The specified output directory "".*outputdir"" does not exist");
        }

        [DataRow([])]
        [DataRow(["--diagnostics-format", "defAULt"])]
        [DataRow(["--diagnostics-format", "sArif"])]
        [DataTestMethod]
        public async Task Build_WithOutDir_ShouldSucceed(string[] args)
        {
            var bicepPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicep",
                """
                output myOutput string = 'hello!'
                """);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);
            var expectedOutputFile = Path.Combine(outputFileDir, "input.json");

            File.Exists(expectedOutputFile).Should().BeFalse();
            var (output, error, result) = await Bicep(["build", "--outdir", outputFileDir, bicepPath, .. args]);

            File.Exists(expectedOutputFile).Should().BeTrue();
            output.Should().BeEmpty();
            if (Array.Exists(args, x => x.Equals("sarif", StringComparison.OrdinalIgnoreCase)))
            {
                var errorJToken = JToken.Parse(error);
                var expectedErrorJToken = JToken.Parse("""
                    {
                      "$schema": "https://schemastore.azurewebsites.net/schemas/json/sarif-2.1.0-rtm.6.json",
                      "version": "2.1.0",
                      "runs": [
                        {
                          "tool": {
                            "driver": {
                              "name": "bicep"
                            }
                          },
                          "results": [],
                          "columnKind": "utf16CodeUnits"
                        }
                      ]
                    }
                    """);
                errorJToken.Should().EqualWithJsonDiffOutput(
                    TestContext,
                    expectedErrorJToken,
                    "",
                    "",
                    validateLocation: false);
            }
            else
            {
                error.Should().BeEmpty();
            }

            result.Should().Be(0);
        }

        [DataRow("DoesNotExist.bicep", new[] { "--stdout" }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("DoesNotExist.bicep", new[] { "--outdir", "." }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("DoesNotExist.bicep", new[] { "--outfile", "file1" }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--stdout" }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--outdir", "." }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--outfile", "file1" }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataTestMethod]
        public async Task Build_InvalidInputPaths_ShouldProduceExpectedError(string badPath, string[] args, string expectedErrorRegex)
        {
            var (output, error, result) = await Bicep(["build", .. args, badPath]);

            result.Should().Be(1);
            output.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Build_LockedOutputFile_ShouldProduceExpectedError()
        {
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "Empty.bicep", DataSets.Empty.Bicep);
            var outputFile = PathHelper.GetDefaultBuildOutputPath(inputFile);

            // ReSharper disable once ConvertToUsingDeclaration
            using (new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                // keep the output stream open while we attempt to write to it
                // this should force an access denied error
                var (output, error, result) = await Bicep("build", inputFile);

                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().Contain("Empty.json");
            }
        }

        [TestMethod]
        public async Task Build_WithEmptyBicepConfig_ShouldProduceConfigurationError()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", DataSets.Empty.Bicep, testOutputPath);
            var configurationPath = FileHelper.SaveResultFile(this.TestContext, "bicepconfig.json", string.Empty, testOutputPath);

            var (output, error, result) = await Bicep("build", inputFile);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().StartWith($"{inputFile}(1,1) : Error BCP271: Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.");
        }

        [TestMethod]
        public async Task Build_WithInvalidBicepConfig_ShouldProduceConfigurationError()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", DataSets.Empty.Bicep, testOutputPath);
            var configurationPath = FileHelper.SaveResultFile(
                this.TestContext,
                "bicepconfig.json",
                """
                {
                  "analyzers": {
                    "core": {
                      "verbose": false,
                      "enabled": true,
                      "rules": {
                        "no-unused-params": {
                          "level": "info"

                """,
                testOutputPath);

            var (output, error, result) = await Bicep("build", inputFile);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().StartWith($"{inputFile}(1,1) : Error BCP271: Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 0.");
        }

        [DataRow([])]
        [DataRow(["--diagnostics-format", "defAULt"])]
        [DataTestMethod]
        public async Task Build_WithValidBicepConfig_ShouldProduceOutputFileAndExpectedError(string[] args)
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", @"param storageAccountName string = 'test'", testOutputPath);
            FileHelper.SaveResultFile(
                this.TestContext,
                "bicepconfig.json",
                """
                {
                  "analyzers": {
                    "core": {
                      "verbose": false,
                      "enabled": true,
                      "rules": {
                        "no-unused-params": {
                          "level": "warning"
                        }
                      }
                    }
                  }
                }
                """,
                testOutputPath);

            var expectedOutputFile = Path.Combine(testOutputPath, "main.json");

            File.Exists(expectedOutputFile).Should().BeFalse();
            var (output, error, result) = await Bicep(["build", "--outdir", testOutputPath, inputFile, .. args]);

            File.Exists(expectedOutputFile).Should().BeTrue();
            result.Should().Be(0);
            output.Should().BeEmpty();
            error.Should().Contain(@"main.bicep(1,7) : Warning no-unused-params: Parameter ""storageAccountName"" is declared but never used. [https://aka.ms/bicep/linter/no-unused-params]");
        }

        [TestMethod]
        public async Task Build_WithValidBicepConfig_ShouldProduceOutputFileAndExpectedErrorInSarifFormat()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", @"param storageAccountName string = 'test'", testOutputPath);
            FileHelper.SaveResultFile(
                this.TestContext,
                "bicepconfig.json",
                """
                {
                   "analyzers":{
                      "core":{
                         "verbose":false,
                         "enabled":true,
                         "rules":{
                            "no-unused-params":{
                               "level":"warning"
                            }
                         }
                      }
                   }
                }
                """,
                testOutputPath);

            var expectedOutputFile = Path.Combine(testOutputPath, "main.json");

            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep("build", "--outdir", testOutputPath, inputFile, "--diagnostics-format", "saRif");

            File.Exists(expectedOutputFile).Should().BeTrue();
            result.Should().Be(0);
            output.Should().BeEmpty();
            var errorJToken = JToken.Parse(error);
            var expectedErrorJToken = JToken.Parse("""
{
   "$schema":"https://schemastore.azurewebsites.net/schemas/json/sarif-2.1.0-rtm.6.json",
   "version":"2.1.0",
   "runs":[
      {
         "tool":{
            "driver":{
               "name":"bicep"
            }
         },
         "results":[
            {
               "ruleId":"no-unused-params",
               "message":{
                  "text":"Parameter \"storageAccountName\" is declared but never used. [https://aka.ms/bicep/linter/no-unused-params]"
               },
               "locations":[
                  {
                     "physicalLocation":{
                        "artifactLocation":{
                           "uri":"main.bicep"
                        },
                        "region":{
                           "startLine":1,
                           "charOffset":7
                        }
                     }
                  }
               ]
            }
         ],
         "columnKind":"utf16CodeUnits"
      }
   ]
}
""");
            var selectedPath = errorJToken.SelectToken("$.runs[0].results[0].locations[0].physicalLocation.artifactLocation.uri");
            selectedPath.Should().NotBeNull();
            selectedPath?.Value<string>().Should().Contain("file://");
            selectedPath?.Value<string>().Should().Contain("main.bicep");
            selectedPath?.Replace("main.bicep");
            errorJToken.Should().EqualWithJsonDiffOutput(
                    TestContext,
                    expectedErrorJToken,
                    "",
                    "",
                    validateLocation: false);
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetValidDataSetsWithExternalModules() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid && ds.HasExternalModules)
            .ToDynamicTestData();
    }
}
