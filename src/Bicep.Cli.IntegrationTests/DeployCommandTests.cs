// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Models;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Deploy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class DeployCommandTests : TestBase
{
    [TestMethod]
    public async Task Deploy_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
    {
        var (output, error, result) = await Bicep("publish");

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Contain($"The input file path was not specified");
        }
    }

    [TestMethod]
    public async Task Deploy_With_Incorrect_Bicep_Deployment_File_Extension_ShouldFail_WithExpectedErrorMessage()
    {
        var bicepDeployPath = FileHelper.SaveResultFile(TestContext, "main.wrongExt", "");
        var (output, error, result) = await Bicep("deploy", bicepDeployPath);

        result.Should().Be(1);
        output.Should().BeEmpty();
        error.Should().Contain($"\"{bicepDeployPath}\" was not recognized as a Bicep deployment file.");
    }

    [TestMethod]
    public async Task Deploy_RequestFailedException_ShouldFail_WithExpectedErrorMessage()
    {
        var bicepDeployPath = FileHelper.SaveResultFile(TestContext, "main.bicepdeploy", "");
        var deploymentManager = StrictMock.Of<IDeploymentManager>();
        deploymentManager
            .Setup(x => x.CreateOrUpdateAsync(It.IsAny<ArmDeploymentDefinition>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Mock deployment request failed"));

        deploymentManager
            .Setup(x => x.CreateOrUpdateAsync(It.IsAny<ArmDeploymentDefinition>(), It.IsAny<Action<ImmutableSortedSet<ArmDeploymentOperation>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Mock deployment request failed"));

        var settings = new InvocationSettings(new(DeploymentFileEnabled: true));
        var (output, error, result) = await Bicep(
                settings, 
                services => services.AddSingleton(deploymentManager.Object), CancellationToken.None, "deploy", bicepDeployPath);
        using (new AssertionScope())
        {
            error.Should().StartWith("Unable to deploy: Mock deployment request failed");
            output.Should().BeEmpty();
            result.Should().Be(1);
        }
    }

    // TODO: Implement this test when baseline data is available
    // [DataTestMethod]
    // [BaselineData_BicepDeploy.TestData(Filter = BaselineData_BicepDeploy.TestDataFilterType.ValidOnly)]
    // [TestCategory(BaselineHelper.BaselineTestCategory)]
    // public async Task Deploy_Valid_Deployment_File_Should_Succeed(BaselineData_BicepDeploy baselineData)
    // {
    //     var data = baselineData.GetData(TestContext);
    //     var (output, error, result) = await Bicep("deploy", data.DeployFile.OutputFilePath);
    //     var deploymentManager = StrictMock.Of<IDeploymentManager>();
    //     deploymentManager
    //         .Setup(x => x.CreateOrUpdateAsync(It.IsAny<ArmDeploymentDefinition>(), It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(new RequestFailedException("Mock deployment request failed"));

    //     deploymentManager
    //         .Setup(x => x.CreateOrUpdateAsync(It.IsAny<ArmDeploymentDefinition>(), It.IsAny<Action<ImmutableSortedSet<ArmDeploymentOperation>>>(), It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(new RequestFailedException("Mock deployment request failed"));
    //     using (new AssertionScope())
    //     {
    //         result.Should().Be(0);
    //         output.Should().BeEmpty();
    //         AssertNoErrors(error);
    //     }

    //     data.Compiled!.ShouldHaveExpectedJsonValue();
    // }
}
