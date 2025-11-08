// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Cli.Models;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrictMock = Bicep.Core.UnitTests.Mock.StrictMock;
using TestEnvironment = Bicep.Core.UnitTests.Utils.TestEnvironment;

namespace Bicep.Cli.IntegrationTests
{

    [TestClass]
    public class BuildParamsCommandTests : TestBase
    {
        [TestMethod]
        public async Task Build_params_with_extends_and_base_merging_succeeds()
        {
            var baseParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "base.bicepparam",
                """
                using none

                param objParam = {
                    baseOnly: 'keep'
                    shared: {
                        fromBase: 'yes'
                        overrideMe: 'base'
                    }
                    arrParam: [1,2,3]
                }

                param strParam = 'strParamFromBase'
                param intParam = 10
                """);

            var mainParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "main.bicepparam",
                """
                using './main.bicep'
                extends './base.bicepparam'

                param objParam = {
                    ...base.objParam
                    shared: {
                        ...base.objParam.shared
                        overrideMe: 'main'
                        addedByMain: 'mainOnly'
                    }
                    arrParam: [...base.objParam.arrParam, 4]
                }

                param strParam = base.strParam
                param intParam = base.intParam + 5
                """,
                Path.GetDirectoryName(baseParamsFile));

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                """
                param objParam object
                param strParam string
                param intParam int
                """,
                Path.GetDirectoryName(baseParamsFile));

            FileHelper.SaveResultFile(
                    TestContext,
                    "bicepconfig.json",
                    """
                    {
                        "experimentalFeaturesEnabled": {
                            "extendableParamFiles": true
                        }
                    }
                    """,
                    Path.GetDirectoryName(baseParamsFile));

            var settings = CreateDefaultSettings();

            var result = await Bicep(settings, "build-params", mainParamsFile, "--stdout");

            result.Should().Succeed();
            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            var paramsObject = parametersStdout.parametersJson.FromJson<JToken>();

            paramsObject.Should().HaveValueAtPath("parameters.strParam.value", "strParamFromBase");
            paramsObject.Should().HaveValueAtPath("parameters.intParam.value", 15);
            paramsObject.Should().HaveValueAtPath("parameters.objParam.value.baseOnly", "keep");
            paramsObject.Should().HaveValueAtPath("parameters.objParam.value.shared.fromBase", "yes");
            paramsObject.Should().HaveValueAtPath("parameters.objParam.value.shared.overrideMe", "main");
            paramsObject.Should().HaveValueAtPath("parameters.objParam.value.shared.addedByMain", "mainOnly");
            paramsObject.Should().HaveValueAtPath("parameters.objParam.value.arrParam", JToken.Parse("[1,2,3,4]"));
        }

        [TestMethod]
        public async Task Build_params_with_base_merging_succeeds()
        {
            var baseParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "shared.bicepparam",
                """
                using none

                param p1 = 'shared1'
                param p2 = 'shared2'
                """);

            var mainParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "main.bicepparam",
                """
                using './main.bicep'
                extends './shared.bicepparam'

                param p2 = base.p1
                """,
                Path.GetDirectoryName(baseParamsFile));

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                """
                param p1 string
                param p2 string
                """,
                Path.GetDirectoryName(baseParamsFile));

            FileHelper.SaveResultFile(
                    TestContext,
                    "bicepconfig.json",
                    """
                    {
                        "experimentalFeaturesEnabled": {
                            "extendableParamFiles": true
                        }
                    }
                    """,
                    Path.GetDirectoryName(baseParamsFile));

            var settings = CreateDefaultSettings();

            var result = await Bicep(settings, "build-params", mainParamsFile, "--stdout");

            result.Should().Succeed();
            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            var paramsObject = parametersStdout.parametersJson.FromJson<JToken>();

            paramsObject.Should().HaveValueAtPath("parameters.p1.value", "shared1");
            paramsObject.Should().HaveValueAtPath("parameters.p2.value", "shared1");
        }

        [TestMethod]
        public async Task Build_params_with_base_without_extends_should_fail()
        {
            var paramsPath = FileHelper.SaveResultFile(
                TestContext,
                "main.bicepparam",
                """
                using './main.bicep'

                param testParam = base.someParam
                """);

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                "param testParam string",
                Path.GetDirectoryName(paramsPath));

            var result = await Bicep(CreateDefaultSettings(), "build-params", paramsPath, "--stdout");

            result.Should().Fail().And.HaveStderrMatch($"*Error BCP437: The identifier '{LanguageConstants.BaseIdentifier}' is only available in parameter files that declare an '{LanguageConstants.ExtendsKeyword}' clause.*");
        }

        [TestMethod]
        public async Task Build_params_with_base_redeclaration_should_fail()
        {
            var sharedParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "shared.bicepparam",
                """
                using './main.bicep'
                param sharedParam = 'shared'
                """);

            var mainParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "main.bicepparam",
                """
                using './main.bicep'
                extends './shared.bicepparam'

                param base = 'redeclared'
                """,
                Path.GetDirectoryName(sharedParamsFile));

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                """
                param parentParam string
                param base string
                """,
                Path.GetDirectoryName(sharedParamsFile));

            FileHelper.SaveResultFile(
                TestContext,
                "bicepconfig.json",
                """
                {
                  "experimentalFeaturesEnabled": {
                    "extendableParamFiles": true
                  }
                }
                """,
                Path.GetDirectoryName(sharedParamsFile));

            var result = await Bicep(CreateDefaultSettings(), "build-params", mainParamsFile, "--stdout");

            result.Should().Fail().And.HaveStderrMatch($"*Error BCP438: The identifier '{LanguageConstants.BaseIdentifier}' is reserved and cannot be declared.*");
        }

        [TestMethod]
        public async Task Build_Params_With_NonExisting_File_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep(CreateDefaultSettings(), "build-params", "/nonexisting/nonexisting.bicepparam");

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"An error occurred reading file. Could not find a part of the path '{Path.GetFullPath("/nonexisting/nonexisting.bicepparam")}'.");
        }

        [TestMethod]
        public async Task Build_Params_With_Incorrect_Bicep_File_Extension_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.wrongExt", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var (output, error, result) = await Bicep(CreateDefaultSettings(), "build-params", bicepparamsPath, "--bicep-file", bicepPath, "--outfile", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"\"{bicepPath}\" was not recognized as a Bicep file.");
        }

        [TestMethod]
        public async Task Build_Params_With_CLI_Input_And_Using_Declaration_Bicep_File_Reference_Mismatch_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var otherBicepPath = FileHelper.SaveResultFile(TestContext, "otherMain.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var result = await Bicep(CreateDefaultSettings(), "build-params", bicepparamsPath, "--bicep-file", otherBicepPath, "--outfile", outputFilePath);

            result.Should().Fail().And.HaveStderrMatch($"Bicep file {otherBicepPath} provided with --bicep-file option doesn't match the Bicep file {bicepPath} referenced by the \"using\" declaration in the parameters file.*");
        }

        [TestMethod]
        public async Task Build_Params_Bicep_File_Reference_Mismatch_And_Other_Diagnostics_ShouldFail_WithAllExpectedErrorMessages()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam",
            @"
            using './main.bicep'

            param foo = 'bar'
            ");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", "param foo object", Path.GetDirectoryName(bicepparamsPath));

            var otherBicepPath = FileHelper.SaveResultFile(TestContext, "otherMain.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            var result = await Bicep(CreateDefaultSettings(), "build-params", bicepparamsPath, "--bicep-file", otherBicepPath, "--outfile", outputFilePath);

            result.Should().Fail().And.HaveStderrMatch($"Bicep file {otherBicepPath} provided with --bicep-file option doesn't match the Bicep file {bicepPath} referenced by the \"using\" declaration in the parameters file.*");
            File.Exists(outputFilePath).Should().BeFalse();
        }

        [TestMethod]
        public async Task Build_params_with_correct_overrides_succeeds_with_values_overridden()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicepparam",
                """
                using './main.bicep'

                param strParam = 'foo'
                param intParam = 0
                param boolParam = false
                param arrParam = [1, 2]
                param objParam = {
                    someProp: 'someValue'
                }
                """);

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                """
                param strParam string
                param intParam int
                param boolParam bool
                param arrParam array
                param objParam object
                """,
                Path.GetDirectoryName(bicepparamsPath));

            var paramsOverrides = """
                {
                    "strParam" : "bar",
                    "intParam" : 1,
                    "boolParam" : true,
                    "arrParam" : [3, 4],
                    "objParam" : {
                        otherProp: "otherValue"
                    }
                }
                """;

            var settings = CreateDefaultSettings() with
            {
                Environment = TestEnvironment.Default.WithVariables(
                    ("BICEP_PARAMETERS_OVERRIDES", paramsOverrides)
                )
            };

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var result = await Bicep(settings, "build-params", bicepparamsPath, "--stdout");

            result.Should().Succeed();
            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();

            var paramsObject = parametersStdout.parametersJson.FromJson<JToken>();

            paramsObject.Should().HaveValueAtPath("parameters.strParam.value", "bar");
            paramsObject.Should().HaveValueAtPath("parameters.intParam.value", 1);
            paramsObject.Should().HaveValueAtPath("parameters.boolParam.value", true);
            paramsObject.Should().HaveValueAtPath("parameters.arrParam.value", JToken.Parse("[3, 4]"));
            paramsObject.Should().HaveValueAtPath("parameters.objParam.value", JToken.Parse(
            """
            {
                otherProp: "otherValue"
            }
            """));
        }

        [TestMethod]
        public async Task Build_params_with_default_value_override_succeeds()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicepparam", """
using 'main.bicep'
""");

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep", """
param foo string = 'default'

output foo string = foo
""",
                Path.GetDirectoryName(bicepparamsPath));

            var environment = TestEnvironment.Default.WithVariables(("BICEP_PARAMETERS_OVERRIDES", new
            {
                foo = "bar"
            }.ToJson()));

            var settings = CreateDefaultSettings() with { Environment = environment };
            var result = await Bicep(settings, "build-params", bicepparamsPath, "--stdout");

            result.Should().NotHaveStderr();
            result.Should().Succeed();
            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            var paramsObject = parametersStdout.parametersJson.FromJson<JToken>();

            paramsObject.Should().DeepEqual(JToken.Parse("""
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "foo": {
      "value": "bar"
    }
  }
}
"""));
        }

        [TestMethod]
        public async Task Build_params_with_incorrect_default_value_override_fails()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicepparam", """
using 'main.bicep'
""");

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep", """
param foo string = 'default'

output foo string = foo
""",
                Path.GetDirectoryName(bicepparamsPath));

            var environment = TestEnvironment.Default.WithVariables(("BICEP_PARAMETERS_OVERRIDES", new
            {
                wrongName = "bar"
            }.ToJson()));

            var settings = CreateDefaultSettings() with { Environment = environment };
            var result = await Bicep(settings, "build-params", bicepparamsPath, "--stdout");

            result.Should().Fail().And.HaveStderrMatch($"*Error BCP259: The parameter \"wrongName\" is assigned in the params file without being declared in the Bicep file.*");
        }

        [TestMethod]
        public async Task Build_params_with_overrides_with_mismatch_type_fails_with_error()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(
                TestContext,
                "input.bicepparam",
                """
                using './main.bicep'
                param intParam = 0
                """);

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                """
                param intParam int
                """,
                Path.GetDirectoryName(bicepparamsPath));

            var paramsOverrides = """
                {
                    "intParam" : "bar"
                }
                """;

            var settings = CreateDefaultSettings() with
            {
                Environment = TestEnvironment.Default.WithVariables(
                    ("BICEP_PARAMETERS_OVERRIDES", paramsOverrides)
                )
            };

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var result = await Bicep(settings, "build-params", bicepparamsPath, "--stdout");
            result.Should().Fail().And.NotHaveStdout();
            result.Stderr.Should().Contain("Error BCP033: Expected a value of type \"int\" but the provided value is of type \"'bar'\".");
        }

        [TestMethod]
        public async Task Build_params_incompatible_type_override_should_fail()
        {
            var rootDir = FileHelper.GetUniqueTestOutputPath(TestContext);
            var basePath = FileHelper.SaveResultFile(TestContext, "base.bicepparam", """
using none

param objParam = { a: 1 }
""", rootDir);

            var childPath = FileHelper.SaveResultFile(TestContext, "child.bicepparam", """
using './main.bicep'
extends './base.bicepparam'

param objParam = 'notAnObject'
""", rootDir);

            FileHelper.SaveResultFile(TestContext, "main.bicep", """
param objParam object
""", rootDir);

            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", """
{ "experimentalFeaturesEnabled": {"extendableParamFiles": true}}
""", rootDir);

            var result = await Bicep(CreateDefaultSettings(), "build-params", childPath, "--stdout");
            result.Should().Fail();
            result.Stderr.Should().Contain("Error BCP033: Expected a value of type \"object\" but the provided value is of type \"'notAnObject'\".");
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var (output, error, result) = await Bicep(await CreateDefaultSettingsWithDefaultMockRegistry(), "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            data.Compiled.Should().NotBeNull();
            data.Compiled!.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_To_Outdir_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var (output, error, result) = await Bicep(await CreateDefaultSettingsWithDefaultMockRegistry(), "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath, "--outdir", Path.GetDirectoryName(data.Compiled!.OutputFilePath));

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            data.Compiled.Should().NotBeNull();
            data.Compiled!.ReadFromOutputFolder().Should().OnlyContainLFNewline();
            data.Compiled!.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_ToStdOut_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var (output, error, result) = await Bicep(await CreateDefaultSettingsWithDefaultMockRegistry(), "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath, "--stdout");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            var parametersStdout = output.FromJson<BuildParamsStdout>();
            parametersStdout.parametersJson.Should().OnlyContainLFNewline();

            data.Compiled.Should().NotBeNull();
            data.Compiled!.WriteToOutputFolder(parametersStdout.parametersJson);
            data.Compiled.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.InvalidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Invalid_Single_Params_File_ShouldFail_WithExpectedErrorMessage(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var artifactManager = await CreateDefaultExternalArtifactManager();

            var serviceBuilder = new ServiceBuilder()
                .WithFeatureOverrides(CreateDefaultFeatureProviderOverrides())
                .WithTestArtifactManager(artifactManager);

            var diagnostics = await GetAllParamDiagnostics(serviceBuilder, data.Parameters.OutputFilePath);

            var settings = CreateDefaultSettings().WithArtifactManager(artifactManager, TestContext);

            var (output, error, result) = await Bicep(settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().ContainAll(diagnostics);
            }
        }

        [TestMethod]
        [EmbeddedFilesTestData(@"Files/BuildParamsCommandTests/.*/main\.bicepparam")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_to_stdout_with_non_bicep_references_should_succeed(EmbeddedFile paramFile)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
            var outputFile = baselineFolder.GetFileOrEnsureCheckedIn("output.json");

            var result = await Bicep(await CreateDefaultSettingsWithDefaultMockRegistry(), "build-params", baselineFolder.EntryFile.OutputFilePath, "--stdout");
            result.Should().Succeed();

            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            // Force consistency for escaped newlines.
            parametersStdout = parametersStdout with { templateJson = parametersStdout?.templateJson?.ReplaceLineEndings("\n") };
            outputFile.WriteJsonToOutputFolder(parametersStdout);
            outputFile.ShouldHaveExpectedJsonValue();
        }

        [TestMethod]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_to_stdout_with_experimentalfeaturenotenabled_should_fail()
        {
            var mainBicepParamPath = FileHelper.SaveResultFile(
                TestContext,
                "main.bicepparam",
                """
                using 'br:mockregistry.io/parameters/basic:v1'
                extends 'shared.bicepparam'
                param intParam = 123
                param boolParam = false
                param arrayParam = []
                param objectParam = {}
                """);

            var sharedBicepParamPath = FileHelper.SaveResultFile(
                TestContext,
                "shared.bicepparam", """
                using none
                param stringParam = 'foo'
                """,
                Path.GetDirectoryName(mainBicepParamPath));

            var bicepConfigPath = FileHelper.SaveResultFile(
                TestContext,
                "bicepconfig.json", "{}",
                Path.GetDirectoryName(mainBicepParamPath));

            var result = await Bicep(await CreateDefaultSettingsWithDefaultMockRegistry(), "build-params", mainBicepParamPath, "--stdout");

            result.Should().Fail().And.HaveStderrMatch($"*Error BCP406: Using \"extends\" keyword requires enabling EXPERIMENTAL feature \"ExtendableParamFiles\".*");
        }

        [TestMethod]
        [EmbeddedFilesTestData(@"Files/BuildParamsCommandTests/.*/main\.bicepparam")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_returns_intuitive_error_if_invoked_with_bicep_file_param(EmbeddedFile paramFile)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
            var bicepFile = Path.Combine(baselineFolder.OutputFolderPath, "main.bicep");
            File.WriteAllText(bicepFile, "");

            var result = await Bicep(await CreateDefaultSettingsWithDefaultMockRegistry(), "build-params", baselineFolder.EntryFile.OutputFilePath, "--bicep-file", bicepFile, "--stdout");
            result.Should().Fail().And.HaveStderrMatch($"Bicep file * provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.*");
        }

        [TestMethod]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_works_with_using_none()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", @"
                param unusedParam int
                ", outputPath);

            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", @"
                using none

                param unusedParam = 3
                ", outputPath);

            var (output, error, result) = await Bicep(["build-params", inputFile, "--bicep-file", bicepFile]);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);

            File.Exists(expectedOutputFile).Should().BeTrue();
            output.Should().BeEmpty();
            error.Should().BeEmpty();
            result.Should().Be(0);
        }

        [TestMethod]
        [EmbeddedFilesTestData(@"Files/BuildParamsCommandTests/Registry/main\.bicepparam")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_to_stdout_with_registry_should_succeed_after_restore(EmbeddedFile paramFile)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
            var outputFile = baselineFolder.GetFileOrEnsureCheckedIn("output.json");

            var settings = await CreateDefaultSettingsWithDefaultMockRegistry();

            var result = await Bicep(settings, "restore", baselineFolder.EntryFile.OutputFilePath);
            result.Should().Succeed().And.NotHaveStdout().And.NotHaveStderr();

            result = await Bicep(settings, "build-params", baselineFolder.EntryFile.OutputFilePath, "--no-restore", "--stdout");
            result.Should().Succeed().And.NotHaveStderr();

            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            // Force consistency for escaped newlines.
            parametersStdout = parametersStdout with { templateJson = parametersStdout?.templateJson?.ReplaceLineEndings("\n") };
            outputFile.WriteJsonToOutputFolder(parametersStdout);
            outputFile.ShouldHaveExpectedJsonValue();
        }

        [TestMethod]
        [EmbeddedFilesTestData(@"Files/BuildParamsCommandTests/Registry/main\.bicepparam")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_bicepparam_should_fail_with_error_diagnostics_for_registry_failure(EmbeddedFile paramFile)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);

            var client = StrictMock.Of<ContainerRegistryContentClient>();
            client
                .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Mock registry request failure."));

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), new Uri("https://mockregistry.io"), "parameters/basic"))
                .Returns(client.Object);

            var templateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>();

            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, templateSpecRepositoryFactory.Object);
            var result = await Bicep(settings, "build-params", baselineFolder.EntryFile.OutputFilePath, "--stdout");

            result.Should().Fail().And.NotHaveStdout();
            result.Stderr.Should().Contain("main.bicepparam(1,7) : Error BCP192: Unable to restore the artifact with reference \"br:mockregistry.io/parameters/basic:v1\": Mock registry request failure.");
        }

        [DataRow([])]
        [DataRow(["--diagnostics-format", "defAULt"])]
        [DataRow(["--diagnostics-format", "sArif"])]
        [TestMethod]
        public async Task BuildParams_supports_sarif_diagnostics_format(string[] args)
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", @"
    @minValue(1)
    @maxValue(50)
    param unusedParam int
    ", outputPath);
            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", @"
    using 'main.bicep'

    param unusedParam = 3
    ", outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);

            File.Exists(expectedOutputFile).Should().BeFalse();
            var (output, error, result) = await Bicep(["build-params", inputFile, .. args]);

            File.Exists(expectedOutputFile).Should().BeTrue();
            output.Should().BeEmpty();
            if (Array.Exists(args, x => x.Equals("sarif", StringComparison.OrdinalIgnoreCase)))
            {
                var sarifLog = JsonConvert.DeserializeObject<SarifLog>(error)!;
                sarifLog.Runs[0].Results[0].RuleId.Should().Be("no-unused-params");
                sarifLog.Runs[0].Results[0].Message.Text.Should().Be("Parameter \"unusedParam\" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-params]");
            }
            else
            {
                error.Should().Contain($"{bicepFile}(4,11) : Warning no-unused-params: Parameter \"unusedParam\" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-params]");
            }

            result.Should().Be(0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task BuildParams_should_compile_files_matching_pattern(bool useRootPath)
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var fileSystem = new MockFileSystem();

            var mainContents = """
                param intParam int

                output intOutput int = intParam
                """;

            var paramsContents = """
                using 'main.bicep'

                param intParam = 42
                """;

            fileSystem.AddFile($"{outputPath}/main.bicep", mainContents);
            FileHelper.SaveResultFile(TestContext, "main.bicep", mainContents, outputPath);


            var fileResults = new[]
            {
                (input: "file1.bicepparam", expectOutput: true),
                (input: "file2.bicepparam", expectOutput: true),
                (input: "nofile.bicepparam", expectOutput: false)
            };

            foreach (var (input, _) in fileResults)
            {
                fileSystem.AddFile($"{outputPath}/{input}", paramsContents);
                // Since Matcher uses the real file system, we need to save the files to the
                // real file system as well so it can find the files.
                FileHelper.SaveResultFile(TestContext, input, paramsContents, outputPath);
            }

            if (!useRootPath)
            {
                fileSystem.Directory.SetCurrentDirectory(outputPath);
            }

            var (output, error, result) = await Bicep(
                services => services.WithFileSystem(fileSystem),
                ["build-params",
                    "--pattern",
                    useRootPath ? $"{outputPath}/file*.bicepparam" : "file*.bicepparam"]);

            result.Should().Be(0);
            error.Should().BeEmpty();
            output.Should().BeEmpty();

            foreach (var (input, expectOutput) in fileResults)
            {
                var outputFile = fileSystem.Path.ChangeExtension(input, ".json");
                fileSystem.File.Exists(fileSystem.Path.Combine(outputPath, outputFile)).Should().Be(expectOutput);
            }
        }

        [TestMethod]
        public async Task Build_WithPatternAndOutDir_ShouldReplicateDirStructure()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepInputPath = Path.Combine(testOutputPath, "input");
            var bicepOutputPath = Path.Combine(testOutputPath, "output");
            Directory.CreateDirectory(bicepOutputPath);
            var fileResults = new[]
            {
                (Path: "foo.bicepparam", Contents: "using 'main.bicep'\n\nparam intParam = 42\n"),
                (Path: "dir/bar.bicepparam", Contents: "using '../main.bicep'\n\nparam intParam = 42\n"),
            };
            FileHelper.SaveResultFile(TestContext, Path.Combine(bicepInputPath, "main.bicep"),
                """
                param intParam int
                output intOutput int = intParam
                """);

            // Create input structure
            foreach (var (f, contents) in fileResults)
            {
                FileHelper.SaveResultFile(TestContext, Path.Combine(bicepInputPath, f), contents, testOutputPath);
            }

            var (output, error, result) = await Bicep(["build-params", "--pattern", $"{bicepInputPath}/**/*.bicepparam", "--outdir", bicepOutputPath]);

            error.Should().BeEmpty();
            output.Should().BeEmpty();
            result.Should().Be(0);

            foreach (var (f, _) in fileResults)
            {
                var outputFile = Path.ChangeExtension(f, ".json");
                File.Exists(Path.Combine(bicepOutputPath, outputFile)).Should().Be(true, f);
            }
        }

        [TestMethod]
        public async Task Build_params_with_multiple_object_spreads_succeeds()
        {
            var rootDir = FileHelper.GetUniqueTestOutputPath(TestContext);

            var basePath = FileHelper.SaveResultFile(TestContext, "base.bicepparam", """
                using none

                param obj = {
                    a: 1
                    arr: [1,2]
                }
                """, rootDir);

            var mainPath = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
                using './main.bicep'
                extends './base.bicepparam'

                param obj = {
                    pre: 'pre'
                    ...base.obj
                    mid: 'mid'
                    ...base.obj
                    post: 'post'
                }
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "main.bicep", """
                param obj object
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", """
                { "experimentalFeaturesEnabled": {"extendableParamFiles": true}}
                """, rootDir);

            var result = await Bicep(CreateDefaultSettings(), "build-params", mainPath, "--stdout");
            result.Should().Succeed();
            var json = result.Stdout.FromJson<BuildParamsStdout>().parametersJson.FromJson<JToken>();
            json.Should().HaveValueAtPath("parameters.obj.value.a", 1);
            json.Should().HaveValueAtPath("parameters.obj.value.pre", "pre");
            json.Should().HaveValueAtPath("parameters.obj.value.mid", "mid");
            json.Should().HaveValueAtPath("parameters.obj.value.post", "post");
            json.Should().HaveValueAtPath("parameters.obj.value.arr", JToken.Parse("[1,2]"));
        }

        [TestMethod]
        public async Task Build_params_with_array_spread_positions_succeeds()
        {
            var rootDir = FileHelper.GetUniqueTestOutputPath(TestContext);
            var basePath = FileHelper.SaveResultFile(TestContext, "base.bicepparam", """
                using none

                param arr = [1,2,3]
                """, rootDir);

            var mainPath = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
                using './main.bicep'
                extends './base.bicepparam'

                param arrStart = [0, ...base.arr]
                param arrMiddle = [0, ...base.arr, 4]
                param arrEnd = [...base.arr, 4]
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "main.bicep", """
                param arrStart array
                param arrMiddle array
                param arrEnd array
                param arr array
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", """
                { "experimentalFeaturesEnabled": {"extendableParamFiles": true}}
                """, rootDir);

            var result = await Bicep(CreateDefaultSettings(), "build-params", mainPath, "--stdout");
            result.Should().Succeed();
            var json = result.Stdout.FromJson<BuildParamsStdout>().parametersJson.FromJson<JToken>();
            json.Should().HaveValueAtPath("parameters.arrStart.value", JToken.Parse("[0,1,2,3]"));
            json.Should().HaveValueAtPath("parameters.arrMiddle.value", JToken.Parse("[0,1,2,3,4]"));
            json.Should().HaveValueAtPath("parameters.arrEnd.value", JToken.Parse("[1,2,3,4]"));
        }

        [TestMethod]
        public async Task Build_params_child_variable_referencing_base_param_succeeds()
        {
            var rootDir = FileHelper.GetUniqueTestOutputPath(TestContext);
            var basePath = FileHelper.SaveResultFile(TestContext, "base.bicepparam", """
                using none

                param greeting = 'hello'
                """, rootDir);

            var mainPath = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
                using './main.bicep'
                extends './base.bicepparam'

                var full = '${base.greeting}-world'
                param final = full
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "main.bicep", """
                param greeting string
                param final string
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", """
                { "experimentalFeaturesEnabled": {"extendableParamFiles": true}}
                """, rootDir);

            var result = await Bicep(CreateDefaultSettings(), "build-params", mainPath, "--stdout");
            result.Should().Succeed();
            var json = result.Stdout.FromJson<BuildParamsStdout>().parametersJson.FromJson<JToken>();
            json.Should().HaveValueAtPath("parameters.final.value", "hello-world");
        }

        [TestMethod]
        public async Task Build_params_spread_non_object_should_fail()
        {
            var rootDir = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "base.bicepparam", """
                using none

                param strParam = 'text'
                """, rootDir);

            var childPath = FileHelper.SaveResultFile(TestContext, "child.bicepparam", """
                using './main.bicep'
                extends './base.bicepparam'

                param objParam = { ...base.strParam }
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "main.bicep", """
                param strParam string
                param objParam object
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", """
                { "experimentalFeaturesEnabled": {"extendableParamFiles": true}}
                """, rootDir);

            var result = await Bicep(CreateDefaultSettings(), "build-params", childPath, "--stdout");
            result.Should().Fail();
            result.Stderr.Should().Contain("Error BCP338: Failed to evaluate parameter \"objParam\"");
        }

        [TestMethod]
        public async Task Build_params_self_extends_should_fail()
        {
            var rootDir = FileHelper.GetUniqueTestOutputPath(TestContext);
            var path = FileHelper.SaveResultFile(TestContext, "self.bicepparam", """
                using './main.bicep'
                extends './self.bicepparam'

                param p = 1
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "main.bicep", """
                param p int
                """, rootDir);

            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", """
                { "experimentalFeaturesEnabled": {"extendableParamFiles": true}}
                """, rootDir);

            var result = await Bicep(CreateDefaultSettings(), "build-params", path, "--stdout");
            result.Should().Fail();
            result.Stderr.Should().Contain("Error BCP278: This parameters file references itself, which is not allowed.");
        }

        public async Task BuildParams_Extends_InvalidType_ThrowsError()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "main.bicep", @"
            param tag string
            ", outputPath);
            FileHelper.SaveResultFile(TestContext, "base.bicepparam", @"
            using none
            param tag = 42
            ", outputPath);
            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", @"
            using 'main.bicep'
            extends 'base.bicepparam'
            ", outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);
            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep(["build-params", inputFile]);

            File.Exists(expectedOutputFile).Should().BeFalse();

            output.Should().BeEmpty();
            error.Should().Contain("Error BCP033: Expected a value of type \"string\" but the provided value is of type \"42\".");
            result.Should().Be(1);
        }

        [TestMethod]
        public async Task BuildParams_Extends_Multiple_InvalidType_ThrowsMultipleErrors()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "main.bicep", @"
            param myString string
            param myInt int
            param myBool bool
            ", outputPath);
            FileHelper.SaveResultFile(TestContext, "base.bicepparam", @"
            using none
            param myInt = '42'
            param myString = {}
            param myBool = []
            ", outputPath);
            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", @"
            using './main.bicep'
            extends 'base.bicepparam'
            ", outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);
            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep(["build-params", inputFile]);

            File.Exists(expectedOutputFile).Should().BeFalse();

            output.Should().BeEmpty();
            error.Should().Contain("Error BCP033: Expected a value of type \"int\" but the provided value is of type \"'42'\".");
            error.Should().Contain("Error BCP033: Expected a value of type \"string\" but the provided value is of type \"object\".");
            error.Should().Contain("Error BCP033: Expected a value of type \"bool\" but the provided value is of type \"<empty array>\".");
            result.Should().Be(1);
        }

        [TestMethod]
        public async Task BuildParams_ResourceInputType_WithValidObject_Succeeds()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "main.bicep", """
            @description('Parameter with resourceInput type')
            param storageConfig resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties.encryption

            output test string = 'success'
            """, outputPath);

            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
            using './main.bicep'

            param storageConfig = {
              services: {
                blob: {
                  enabled: true
                }
                file: {
                  enabled: true
                }
              }
              keySource: 'Microsoft.Storage'
            }
            """, outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);
            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep(["build-params", inputFile]);

            result.Should().Be(0);
            error.Should().NotContain("Error");
            File.Exists(expectedOutputFile).Should().BeTrue();

            var parametersFile = File.ReadAllText(expectedOutputFile);
            var parametersObject = JObject.Parse(parametersFile);
            ((JToken)parametersObject).Should().NotBeNull();
            var storageConfigValue = parametersObject["parameters"]?["storageConfig"]?["value"];
            storageConfigValue.Should().NotBeNull();
        }

        [TestMethod]
        public async Task BuildParams_ResourceInputType_NestedProperty_Succeeds()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "main.bicep", """
            @description('Parameter with nested resourceInput type')
            param encryptionServices resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties.encryption.services

            output test string = 'success'
            """, outputPath);

            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
            using './main.bicep'

            param encryptionServices = {
              blob: {
                enabled: true
                keyType: 'Account'
              }
              file: {
                enabled: false
              }
            }
            """, outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);
            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep(["build-params", inputFile]);

            result.Should().Be(0);
            error.Should().NotContain("Error");
            File.Exists(expectedOutputFile).Should().BeTrue();
        }

        [TestMethod]
        public async Task BuildParams_ResourceInputType_ArrayOfResources_Succeeds()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "main.bicep", """
            @description('Parameter with array of resourceInput type')
            param subnets resourceInput<'Microsoft.Network/virtualNetworks/subnets@2023-09-01'>.properties[]

            output test string = 'success'
            """, outputPath);

            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
            using './main.bicep'

            param subnets = [
              {
                addressPrefix: '10.0.1.0/24'
                privateEndpointNetworkPolicies: 'Disabled'
              }
              {
                addressPrefix: '10.0.2.0/24'
                delegations: []
              }
            ]
            """, outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);
            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep(["build-params", inputFile]);

            result.Should().Be(0);
            error.Should().NotContain("Error");
            File.Exists(expectedOutputFile).Should().BeTrue();

            var parametersFile = File.ReadAllText(expectedOutputFile);
            var parametersObject = JObject.Parse(parametersFile);
            var subnetsArray = parametersObject["parameters"]?["subnets"]?["value"] as JArray;
            subnetsArray.Should().NotBeNull();
            subnetsArray!.Count.Should().Be(2);
        }

        [TestMethod]
        public async Task BuildParams_ResourceInputType_ComplexNestedObject_Succeeds()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            FileHelper.SaveResultFile(TestContext, "main.bicep", """
            @description('Parameter with complex resourceInput type')
            param organizationProfile resourceInput<'Microsoft.DevOpsInfrastructure/pools@2024-10-19'>.properties.organizationProfile

            output test string = 'success'
            """, outputPath);

            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
            using './main.bicep'

            param organizationProfile = {
              kind: 'AzureDevOps'
              organizations: [
                {
                  url: 'https://dev.azure.com/my-org'
                  projects: []
                  parallelism: 1
                }
              ]
              permissionProfile: {
                kind: 'CreatorOnly'
              }
            }
            """, outputPath);

            var expectedOutputFile = FileHelper.GetResultFilePath(TestContext, "main.json", outputPath);
            File.Exists(expectedOutputFile).Should().BeFalse();

            var (output, error, result) = await Bicep(["build-params", inputFile]);

            result.Should().Be(0);
            error.Should().NotContain("Error");
            File.Exists(expectedOutputFile).Should().BeTrue();

            var parametersFile = File.ReadAllText(expectedOutputFile);
            var parametersObject = JObject.Parse(parametersFile);
            var kindValue = parametersObject["parameters"]?["organizationProfile"]?["value"]?["kind"]?.ToString();
            kindValue.Should().Be("AzureDevOps");
        }

        [TestMethod]
        public async Task Build_params_with_extends_and_loadTextContent_in_base_succeeds()
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            var exampleTxtFile = FileHelper.SaveResultFile(
                TestContext,
                "example.txt",
                "This is an example txt file.",
                outputPath);

            var baseParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "base.bicepparam",
                """
                using none

                param foo = {
                 bar: loadTextContent('./example.txt')
                }
                """,
                outputPath);

            var mainParamsFile = FileHelper.SaveResultFile(
                TestContext,
                "main.bicepparam",
                """
                using './main.bicep'
                extends './base.bicepparam'
                """,
                outputPath);

            FileHelper.SaveResultFile(
                TestContext,
                "main.bicep",
                """
                #disable-next-line no-unused-params
                param foo object
                """,
                outputPath);

            FileHelper.SaveResultFile(
                TestContext,
                "bicepconfig.json",
                """
                {
                    "experimentalFeaturesEnabled": {
                        "extendableParamFiles": true
                    }
                }
                """,
                outputPath);

            var settings = CreateDefaultSettings();
            var result = await Bicep(settings, "build-params", mainParamsFile, "--stdout");

            result.Should().Succeed();

            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            parametersStdout.Should().NotBeNull();

            var parametersObject = JObject.Parse(parametersStdout!.parametersJson);
            var bar = parametersObject["parameters"]?["foo"]?["value"]?["bar"]?.ToString();
            bar.Should().Be("This is an example txt file.");
        }
    }
}
