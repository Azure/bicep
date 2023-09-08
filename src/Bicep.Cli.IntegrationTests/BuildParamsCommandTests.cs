// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Cli.Models;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;

namespace Bicep.Cli.IntegrationTests
{

    [TestClass]
    public class BuildParamsCommandTests : TestBase
    {
        
        [NotNull]
        public TestContext? TestContext { get; set; }

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

            var settings = new InvocationSettings(new(TestContext), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
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

            var settings = new InvocationSettings(new(TestContext), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
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
            var features = new FeatureProviderOverrides(TestContext);
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

            var settings = new InvocationSettings(new (), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "build-params", data.Parameters.OutputFilePath, "--bicep-file", data.Bicep.OutputFilePath, "--stdout");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            var parametersStdout = JsonSerializer.Deserialize<BuildParamsStdout>(output)!;
            data.Compiled!.WriteToOutputFolder(parametersStdout.parametersJson);
            data.Compiled.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.InvalidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Build_Invalid_Single_Params_File_ShouldFail_WithExpectedErrorMessage(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var settings = new InvocationSettings(new(TestContext), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);
            var diagnostics = await GetAllParamDiagnostics(data.Parameters.OutputFilePath);

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

            var clients = await MockRegistry.Build();
            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clients.ContainerRegistry, clients.TemplateSpec);

            var result = await Bicep(settings, "build-params", baselineFolder.EntryFile.OutputFilePath, "--stdout");
            result.Should().Succeed().And.NotHaveStderr();

            var parametersStdout = result.Stdout.FromJson<BuildParamsStdout>();
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

            var result = await Bicep(settings, "build-params", baselineFolder.EntryFile.OutputFilePath, "--bicep-file", bicepFile,  "--stdout");
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
            result.Stderr.Should().Contain("main.bicepparam(1,7) : Error BCP192: Unable to restore the module with reference \"br:mockregistry.io/parameters/basic:v1\": Mock registry request failure.");
        }

        [TestInitialize]
        public void testInit()
        {
            System.Environment.SetEnvironmentVariable("stringEnvVariableName", "test");
            System.Environment.SetEnvironmentVariable("intEnvVariableName", "100");
            System.Environment.SetEnvironmentVariable("boolEnvironmentVariable", "true");
        }
    }
}