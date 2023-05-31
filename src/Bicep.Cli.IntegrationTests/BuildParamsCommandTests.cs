// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{

    [TestClass]
    public class BuildParamsCommandTests : TestBase
    {
        
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Build_Params_Without_Feature_Flag_Disabled_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            FileHelper.SaveResultFile(TestContext, "main.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var(output, error, result) = await Bicep("build-params", bicepparamsPath,"--outfile", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"{bicepparamsPath}(1,1) : Error BCP336: Using a Bicep Parameters file requires enabling EXPERIMENTAL feature \"{nameof(ExperimentalFeaturesEnabled.ParamsFiles)}\". [https://aka.ms/bicep/config]");
        }

        [TestMethod]
        public async Task Build_Params_With_Feature_Flag_Enabled_ShouldSucceed()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            FileHelper.SaveResultFile(TestContext, "main.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var settings = new InvocationSettings(new(TestContext, ParamsFilesEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var(output, error, result) = await Bicep(settings, "build-params", bicepparamsPath,"--outfile", outputFilePath);

            File.Exists(outputFilePath).Should().BeTrue();
            result.Should().Be(0);
            error.Should().BeEmpty();
            output.Should().BeEmpty();
        }


        [TestMethod]
        public async Task Build_Params_With_Incorrect_Bicep_File_Extension_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.wrongExt", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var(output, error, result) = await Bicep("build-params", bicepparamsPath,"--bicep-file", bicepPath, "--outfile", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"{bicepPath} is not a bicep file");
        }

        [TestMethod]
        public async Task Build_Params_With_CLI_Input_And_Using_Declaration_Bicep_File_Reference_Mismatch_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var otherBicepPath = FileHelper.SaveResultFile(TestContext, "otherMain.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var settings = new InvocationSettings(new(TestContext, ParamsFilesEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var(output, error, result) = await Bicep(settings, "build-params", bicepparamsPath,"--bicep-file", otherBicepPath, "--outfile", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"Bicep file {otherBicepPath} provided with --bicep-file option doesn't match the Bicep file {bicepPath} referenced by the using declaration in the parameters file");
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

            var settings = new InvocationSettings(new(TestContext, ParamsFilesEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            var(output, error, result) = await Bicep(settings, "build-params", bicepparamsPath,"--bicep-file", otherBicepPath, "--outfile", outputFilePath);

            var diagnostics = await GetAllParamDiagnostics(bicepparamsPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"Bicep file {otherBicepPath} provided with --bicep-file option doesn't match the Bicep file {bicepPath} referenced by the using declaration in the parameters file");
            error.Should().ContainAll(diagnostics);
            File.Exists(outputFilePath).Should().BeFalse();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Valid_Params_File_Should_Succeed(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            var features = new FeatureProviderOverrides(TestContext, ParamsFilesEnabled: true);
            var settings = new InvocationSettings(features, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var (output, error, result) = await Bicep(settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

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

            var features = new FeatureProviderOverrides(TestContext, ParamsFilesEnabled: true);
            var settings = new InvocationSettings(features, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath, "--stdout");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            var parametersStdout = JsonSerializer.Deserialize<ParametersStdout>(output)!;
            data.Compiled!.WriteToOutputFolder(parametersStdout.parametersJson);
            data.Compiled.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.InvalidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Invalid_Single_Params_File_ShouldFail_WithExpectedErrorMessage(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var settings = new InvocationSettings(new(TestContext, ParamsFilesEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var diagnostics = await GetAllParamDiagnostics(data.Parameters.OutputFilePath);

            var (output, error, result) = await Bicep(settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().ContainAll(diagnostics);
            }
        }
         [TestInitialize]
        public void testInit(){
            System.Environment.SetEnvironmentVariable("stringEnvVariableName", "test");
            System.Environment.SetEnvironmentVariable("intEnvVariableName", "100");
            System.Environment.SetEnvironmentVariable("boolEnvironmentVariable", "true");
        }

        [TestCleanup]
        public void TestCleanup(){
            System.Environment.SetEnvironmentVariable("stringEnvVariableName", null);
            System.Environment.SetEnvironmentVariable("intEnvVariableName", null);
            System.Environment.SetEnvironmentVariable("boolEnvironmentVariable", null);
        }
    }
}