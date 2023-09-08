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
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
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
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class BuildCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Build_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("build");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
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
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules, PublishSourceEnabled: true), clientFactory, templateSpecRepositoryFactory);
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

                Directory.Exists(settings.FeatureOverrides.CacheRootDirectory).Should().BeTrue();
                Directory.EnumerateFiles(settings.FeatureOverrides.CacheRootDirectory!, "*.json", SearchOption.AllDirectories).Should().NotBeEmpty();
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [TestMethod]
        public async Task Provider_Artifacts_Restore_From_Registry_ShouldSucceed()
        {
            // SETUP
            // 1. create a mock registry client
            var registryUri = new Uri($"https://{LanguageConstants.BicepPublicMcrRegistry}");
            var repository = $"bicep/providers/az";
            var (clientFactory, blobClients) = DataSetsExtensions.CreateMockRegistryClients((registryUri, repository));
            var myClient = blobClients[(registryUri, repository)];

            // 2. upload a manifest and its blob layer
            var manifestStr = $$"""
                {
                  "schemaVersion": 2,
                  "mediaType": "application/vnd.oci.image.manifest.v1+json",
                  "artifactType": "{{BicepMediaTypes.BicepProviderArtifactType}}",
                  "config": {
                    "mediaType": "{{BicepMediaTypes.BicepProviderConfigV1}}",
                    "digest": "sha256:44136fa355b3678a1146ad16f7e8649e94fb4fc21fe77e8310c060f61caaff8a",
                    "size": 2
                  },
                  "layers": [
                    {
                      "mediaType": "{{BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip}}",
                      "digest": "sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                      "size": 0
                    }
                  ],
                  "annotations": {
                    "bicep.serialization.format": "v1",
                    "org.opencontainers.image.created": "2023-05-04T16:40:05Z"
                  }
                }
                """;

            await myClient.SetManifestAsync(BinaryData.FromString(manifestStr), "2.0.0");
            await myClient.UploadBlobAsync(new MemoryStream());

            // 3. create a main.bicep and save it to a output directory
            var bicepFile = """
import 'az@2.0.0'
""";
            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);
            var bicepFilePath = Path.Combine(tempDirectory, "main.bicep");
            File.WriteAllText(bicepFilePath, bicepFile);

            // 4. create a settings object with the mock registry client and relevant features enabled
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true, ExtensibilityEnabled: true, DynamicTypeLoading: true), clientFactory.Object, Repository.Create<ITemplateSpecRepositoryFactory>().Object);

            // TEST
            // 5. run bicep build
            var (output, error, result) = await Bicep(settings, "build", bicepFilePath);

            // ASSERT
            // 6. assert 'bicep build' completed successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }
            // 7. assert the provider files were restored to the cache directory
            Directory.Exists(settings.FeatureOverrides.CacheRootDirectory).Should().BeTrue();
            var providerDir = Path.Combine(settings.FeatureOverrides.CacheRootDirectory!, ModuleReferenceSchemes.Oci, LanguageConstants.BicepPublicMcrRegistry, "bicep$providers$az", "2.0.0$");
            Directory.EnumerateFiles(providerDir).ToList().Select(Path.GetFileName).Should().BeEquivalentTo(new List<string> { "types.tgz", "lock", "manifest", "metadata" });
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_WithTemplateSpecReference_ToStdOut_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules, PublishSourceEnabled: true), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "build", "--stdout", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            if (dataSet.HasExternalModules)
            {
                settings.FeatureOverrides.Should().HaveValidCachedModulesWithSources();
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
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules, PublishSourceEnabled: true), clientFactory, templateSpecRepositoryFactory);

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

            var client = new MockRegistryBlobClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

            var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true, PublishSourceEnabled: true), clientFactory.Object, BicepTestConstants.TemplateSpecRepositoryFactory);

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

            client.Blobs.Should().HaveCount(4);
            client.Manifests.Should().HaveCount(2);
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
            var defaultSettings = CreateDefaultSettings();
            var diagnostics = await GetAllDiagnostics(bicepFilePath, defaultSettings.ClientFactory, defaultSettings.TemplateSpecRepositoryFactory);

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

            var defaultSettings = CreateDefaultSettings();
            var diagnostics = await GetAllDiagnostics(bicepFilePath, defaultSettings.ClientFactory, defaultSettings.TemplateSpecRepositoryFactory);
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
        public async Task Build_WithNonExistantOutDir_ShouldFail_WithExpectedErrorMessage()
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

        [DataRow(new string[] { })]
        [DataRow(new[] { "--diagnostics-format", "defAULt" })]
        [DataRow(new[] { "--diagnostics-format", "sArif" })]
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
            var (output, error, result) = await Bicep(new[] { "build", "--outdir", outputFileDir, bicepPath }.Concat(args).ToArray());

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
            var (output, error, result) = await Bicep(new[] { "build" }.Concat(args).Append(badPath).ToArray());

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
            error.Should().StartWith($"{inputFile}(1,1) : Error BCP271: Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: \"The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.\".");
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
            error.Should().StartWith($"{inputFile}(1,1) : Error BCP271: Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: \"Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 0.\".");
        }

        [DataRow(new string[] { })]
        [DataRow(new[] { "--diagnostics-format", "defAULt" })]
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
            var (output, error, result) = await Bicep(new[] { "build", "--outdir", testOutputPath, inputFile }.Concat(args).ToArray());

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
