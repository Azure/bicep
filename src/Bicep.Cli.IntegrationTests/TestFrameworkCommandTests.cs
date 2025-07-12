// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class TestFrameworkCommandTests : TestBase
    {
        [TestMethod]
        public async Task Test_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("test");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [TestMethod]
        public async Task Test_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var (output, error, result) = await Bicep(settings, "test", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($@"The specified input ""{Path.GetFullPath("/dev/zero")}"" was not recognized as a Bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
            }
        }

        [TestMethod]
        public async Task Test_commandNoParams_ShouldSucceed()
        {
            // Test should succeed when there are no required params
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true, AssertsEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);


            var testPath = FileHelper.SaveResultFile(TestContext, "test.bicep", @"// test content here", outputFileDir);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"test foo 'test.bicep' = {}", outputFileDir);

            var (output, error, result) = await Bicep(settings, "test", bicepPath);

            result.Should().Be(0);
            error.Should().NotBeEmpty();
            error.Should().NotContain("Skipped");
            error.Should().NotContain("Failed");

            output.Should().Contain("All 1 evaluations passed!");
        }

        [TestMethod]
        public async Task Test_commandWithRequiredParam_ShouldSucceed()
        {
            // Test should succeed when passing a required parameter
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true, AssertsEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);

            var testPath = FileHelper.SaveResultFile(TestContext, "test.bicep", @"param foo string
                                                                              output foo string = foo", outputFileDir);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"test foo 'test.bicep' = {params:{foo:'ShouldSucceed'}}", outputFileDir);

            var (output, error, result) = await Bicep(settings, "test", bicepPath);

            result.Should().Be(0);
            error.Should().NotBeEmpty();
            error.Should().NotContain("Skipped");
            error.Should().NotContain("Failed");
            output.Should().Contain("All 1 evaluations passed!");
        }
        [TestMethod]
        public async Task Test_commandAllAssertionsPassed_ShouldSucceed()
        {
            // Test should succeed when all Assertions pass
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true, AssertsEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);


            var testPath = FileHelper.SaveResultFile(TestContext, "test.bicep", @"param foo string
                                                                              assert isEqual = foo == 'ShouldSucceed'", outputFileDir);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"test foo 'test.bicep' = {params:{foo:'ShouldSucceed'}}", outputFileDir);

            var (output, error, result) = await Bicep(settings, "test", bicepPath);

            result.Should().Be(0);
            error.Should().NotBeEmpty();
            error.Should().NotContain("Skipped");
            error.Should().NotContain("Failed");

            output.Should().NotBeEmpty();
            output.Should().Contain("All 1 evaluations passed!");
        }


        [TestMethod]
        public async Task Test_command_ShouldFail()
        {
            // Test should fail when not passing a required parameter
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true, AssertsEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);

            var testPath = FileHelper.SaveResultFile(TestContext, "test.bicep", @"param foo string
                                                                                  output foo string = foo", outputFileDir);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"test foo 'test.bicep' = {foo: 1}", outputFileDir);

            var (output, error, result) = await Bicep(settings, "test", bicepPath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain("Evaluation foo Skipped!");
            }

        }
        [TestMethod]
        public async Task Test_commandNotPassingAllRequiredParams_ShouldFail()
        {
            // Test should fail when not passing a required parameter
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true, AssertsEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);

            var testPath = FileHelper.SaveResultFile(TestContext, "test.bicep", @"param foo string
                                                                              output foo string = foo", outputFileDir);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"test foo 'test.bicep' = {params:{}}", outputFileDir);

            var (output, error, result) = await Bicep(settings, "test", bicepPath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain("Evaluation foo Skipped!");
            }
        }

        [TestMethod]
        public async Task Test_commandAssertionFails_ShouldFail()
        {
            // Test should fail when at least one assertion fails
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true, AssertsEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);

            var testPath = FileHelper.SaveResultFile(TestContext, "test.bicep", @"param foo string
                                                                              assert isEqual = foo == 'ShouldNotSucceed'", outputFileDir);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"test foo 'test.bicep' = {params:{foo:'ShouldSucceed'}}", outputFileDir);

            var (output, error, result) = await Bicep(settings, "test", bicepPath);

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain("Evaluation foo Failed");
            }
        }

        [TestMethod]
        public async Task Test_WithoutTestFrameworkEnabled_ShouldFail()
        {
            var (output, error, result) = await Bicep(
                services => services.WithFeatureOverrides(new(TestFrameworkEnabled: false)),
                "test", "/dev/zero.bicep");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain("TestFrameWork not enabled");
            }
        }
    }
}
