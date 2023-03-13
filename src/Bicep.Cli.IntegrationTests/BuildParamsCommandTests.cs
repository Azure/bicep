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
    public class BuildParamsComa : TestBase
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
            var(output, error, result) = await Bicep("build-params", bicepparamsPath,"--outfile-params", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"The specified input \"{bicepparamsPath}\" could not be compiled. Compilation of files with extension .bicepparam is only supported if experimental feature \"{nameof(ExperimentalFeaturesEnabled.ParamsFiles)}\" is enabled.");
        }

        [TestMethod]
        public async Task Build_Params_With_Feature_Flag_Enabled_ShouldSucceed()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            FileHelper.SaveResultFile(TestContext, "main.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var settings = new InvocationSettings(new(TestContext, ParamsFilesEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var(output, error, result) = await Bicep(settings, "build-params", bicepparamsPath,"--outfile-params", outputFilePath);

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
            var(output, error, result) = await Bicep("build-params", bicepparamsPath,"--bicep-file", bicepPath, "--outfile-params", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"{bicepPath} is not a bicep file");
        }

        [TestMethod]
        public async Task Build_Params_With_Same_Params_And_Bicep_Output_Path_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepparamsPath = FileHelper.SaveResultFile(TestContext, "input.bicepparam", "using './main.bicep'");
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", "", Path.GetDirectoryName(bicepparamsPath));

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var(output, error, result) = await Bicep("build-params", bicepparamsPath,"--bicep-file", bicepPath, "--outfile-params", outputFilePath, "--outfile-bicep", outputFilePath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().Contain($"The path for --outfile-params and --outfile-bicep can not be the same");
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
            var(output, error, result) = await Bicep(settings, "build-params", bicepparamsPath,"--bicep-file", otherBicepPath, "--outfile-params", outputFilePath);

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

            var(output, error, result) = await Bicep(settings, "build-params", bicepparamsPath,"--bicep-file", otherBicepPath, "--outfile-params", outputFilePath);

            var diagnostics = await GetAllParamDiagnostics(bicepparamsPath, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

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

            var parametersStdout = JsonSerializer.Deserialize<ParametersStdout>(output);

            string compiledFilePath = data.Compiled!.OutputFilePath;
            File.Exists(compiledFilePath);

            // overwrite the output file
            File.WriteAllText(compiledFilePath, parametersStdout!.parametersJson);

            data.Compiled!.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.InvalidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Invalid_Single_Params_File_ShouldFail_WithExpectedErrorMessage(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var settings = new InvocationSettings(new(TestContext, ParamsFilesEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var diagnostics = await GetAllParamDiagnostics(data.Parameters.OutputFilePath, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().ContainAll(diagnostics);
            }
        }
    }
}