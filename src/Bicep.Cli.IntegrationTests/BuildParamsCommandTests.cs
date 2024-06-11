// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Cli.Models;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests
{

    [TestClass]
    public class BuildParamsCommandTests : TestBase
    {
        private InvocationSettings Settings
            => CreateDefaultSettings() with
            {
                Environment = TestEnvironment.Create(
                    ("stringEnvVariableName", "test"),
                    ("intEnvVariableName", "100"),
                    ("boolEnvironmentVariable", "true")
                )
            };

        [TestMethod]
        public async Task Build_Params_With_Incorrect_Bicep_File_Extension_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.wrongExt", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var (output, error, result) = await Bicep(Settings, "build-params", bicepparamsPath, "--bicep-file", bicepPath, "--outfile", outputFilePath);

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
            var result = await Bicep(Settings, "build-params", bicepparamsPath, "--bicep-file", otherBicepPath, "--outfile", outputFilePath);

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

            var result = await Bicep(Settings, "build-params", bicepparamsPath, "--bicep-file", otherBicepPath, "--outfile", outputFilePath);

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

            var settings = Settings with
            {
                Environment = TestEnvironment.Create(
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

            var environment = TestEnvironment.Create(("BICEP_PARAMETERS_OVERRIDES", new
            {
                foo = "bar"
            }.ToJson()));

            var settings = Settings with { Environment = environment };
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

            var environment = TestEnvironment.Create(("BICEP_PARAMETERS_OVERRIDES", new
            {
                wrongName = "bar"
            }.ToJson()));

            var settings = Settings with { Environment = environment };
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

            var settings = Settings with
            {
                Environment = TestEnvironment.Create(
                ("BICEP_PARAMETERS_OVERRIDES", paramsOverrides)
            )
            };

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var result = await Bicep(settings, "build-params", bicepparamsPath, "--stdout");
            result.Should().Fail().And.NotHaveStdout();
            result.Stderr.Should().Contain("Error BCP033: Expected a value of type \"int\" but the provided value is of type \"'bar'\".");
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            var (output, error, result) = await Bicep(Settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            data.Compiled!.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_To_Outdir_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            var (output, error, result) = await Bicep(Settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath, "--outdir", Path.GetDirectoryName(data.Compiled!.OutputFilePath));

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            data.Compiled!.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_ToStdOut_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var (output, error, result) = await Bicep(Settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath, "--stdout");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            var parametersStdout = output.FromJson<BuildParamsStdout>();
            data.Compiled!.WriteToOutputFolder(parametersStdout.parametersJson);
            data.Compiled.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.InvalidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Invalid_Single_Params_File_ShouldFail_WithExpectedErrorMessage(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var diagnostics = await GetAllParamDiagnostics(data.Parameters.OutputFilePath);

            var (output, error, result) = await Bicep(Settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

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

            var clients = await MockRegistry.Build();
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clients.ContainerRegistry, clients.TemplateSpec);

            var result = await Bicep(settings, "build-params", baselineFolder.EntryFile.OutputFilePath, "--stdout");
            result.Should().Succeed().And.NotHaveStderr();

            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
            // Force consistency for escaped newlines.
            parametersStdout = parametersStdout with { templateJson = parametersStdout?.templateJson?.ReplaceLineEndings("\n") };
            outputFile.WriteJsonToOutputFolder(parametersStdout);
            outputFile.ShouldHaveExpectedJsonValue();
        }

        [TestMethod]
        [EmbeddedFilesTestData(@"Files/BuildParamsCommandTests/.*/main\.bicepparam")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_returns_intuitive_error_if_invoked_with_bicep_file_param(EmbeddedFile paramFile)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
            var bicepFile = Path.Combine(baselineFolder.OutputFolderPath, "main.bicep");
            File.WriteAllText(bicepFile, "");

            var clients = await MockRegistry.Build();
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clients.ContainerRegistry, clients.TemplateSpec);

            var result = await Bicep(settings, "build-params", baselineFolder.EntryFile.OutputFilePath, "--bicep-file", bicepFile, "--stdout");
            result.Should().Fail().And.HaveStderrMatch($"Bicep file * provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.*");
        }

        [TestMethod]
        [EmbeddedFilesTestData(@"Files/BuildParamsCommandTests/Registry/main\.bicepparam")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_params_to_stdout_with_registry_should_succeed_after_restore(EmbeddedFile paramFile)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
            var outputFile = baselineFolder.GetFileOrEnsureCheckedIn("output.json");

            var clients = await MockRegistry.Build();
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clients.ContainerRegistry, clients.TemplateSpec);

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
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), new Uri("https://mockregistry.io"), "parameters/basic"))
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
                sarifLog.Runs[0].Results[0].Message.Text.Should().Be("Parameter \"unusedParam\" is declared but never used. [https://aka.ms/bicep/linter/no-unused-params]");
            }
            else
            {
                error.Should().Contain($"{bicepFile}(4,11) : Warning no-unused-params: Parameter \"unusedParam\" is declared but never used. [https://aka.ms/bicep/linter/no-unused-params]");
            }

            result.Should().Be(0);
        }
    }
}
