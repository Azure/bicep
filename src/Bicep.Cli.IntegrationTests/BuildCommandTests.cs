// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        // TODO: handle variant linter messaging for each data test
        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_ShouldSucceed(DataSet dataSet)
        {
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);

            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);
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
                Directory.Exists(settings.Features.CacheRootDirectory).Should().BeTrue();
                Directory.EnumerateFiles(settings.Features.CacheRootDirectory, "*.json", SearchOption.AllDirectories).Should().NotBeEmpty();
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_ToStdOut_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "build", "--stdout", bicepFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            if (dataSet.HasExternalModules)
            {
                // ensure something got restored
                Directory.Exists(settings.Features.CacheRootDirectory).Should().BeTrue();
                Directory.EnumerateFiles(settings.Features.CacheRootDirectory, "*.json", SearchOption.AllDirectories).Should().NotBeEmpty();
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(output);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSetsWithExternalModules), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Valid_SingleFile_After_Restore_Should_Succeed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var settings = new InvocationSettings(BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: dataSet.HasExternalModules), clientFactory, templateSpecRepositoryFactory);

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
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Build_Invalid_SingleFile_ShouldFail_WithExpectedErrorMessage(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var defaultSettings = CreateDefaultSettings();
            var diagnostics = GetAllDiagnostics(bicepFilePath, defaultSettings.ClientFactory, defaultSettings.TemplateSpecRepositoryFactory);

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
            var diagnostics = GetAllDiagnostics(bicepFilePath, defaultSettings.ClientFactory, defaultSettings.TemplateSpecRepositoryFactory);
            error.Should().ContainAll(diagnostics);
        }

        [TestMethod]
        public async Task Build_WithOutFile_ShouldSucceed()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

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
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            var (output, error, result) = await Bicep("build", "--outdir", outputFileDir, bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The specified output directory "".*outputdir"" does not exist");
        }

        [TestMethod]
        public async Task Build_WithOutDir_ShouldSucceed()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);
            var expectedOutputFile = Path.Combine(outputFileDir, "input.json");

            File.Exists(expectedOutputFile).Should().BeFalse();
            var (output, error, result) = await Bicep("build", "--outdir", outputFileDir, bicepPath);

            File.Exists(expectedOutputFile).Should().BeTrue();
            output.Should().BeEmpty();
            error.Should().BeEmpty();
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

