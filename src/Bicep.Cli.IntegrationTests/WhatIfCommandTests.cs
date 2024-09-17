// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Bicep.Core.Configuration;
using Bicep.Core.Models;
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
public class WhatIfCommandTests : TestBase
{
    [TestMethod]
    public async Task WhatIf_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
    {
        var (output, error, result) = await Bicep("what-if");

        using (new AssertionScope())
        {
            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Contain($"The input file path was not specified");
        }
    }

    [TestMethod]
    public async Task WhatIf_With_Incorrect_Bicep_Deployment_File_Extension_ShouldFail_WithExpectedErrorMessage()
    {
        var bicepDeployPath = FileHelper.SaveResultFile(TestContext, "main.wrongExt", "");
        var (output, error, result) = await Bicep("what-if", bicepDeployPath);

        result.Should().Be(1);
        output.Should().BeEmpty();
        error.Should().Contain($"\"{bicepDeployPath}\" was not recognized as a Bicep deployment file.");
    }

    [TestMethod]
    public async Task WhatIf_RequestFailedException_ShouldFail_WithExpectedErrorMessage()
    {
        var bicepDeployPath = FileHelper.SaveResultFile(TestContext, "main.bicepdeploy", "");
        
        var deploymentManagerFactory = StrictMock.Of<IDeploymentManagerFactory>();
        var deploymentManager = StrictMock.Of<IDeploymentManager>()
            .Setup(x => x.ValidateAsync(It.IsAny<ArmDeploymentDefinition>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Mock what-if request failed"));

        deploymentManagerFactory
            .Setup(x => x.CreateDeploymentManager(It.IsAny<RootConfiguration>()))
            .Returns(StrictMock.Of<IDeploymentManager>().Object);

        var settings = new InvocationSettings(new(DeploymentFileEnabled: true));
        var (output, error, result) = await Bicep(
                settings, 
                services => services.AddSingleton(deploymentManagerFactory.Object), CancellationToken.None, "what-if", bicepDeployPath);
        using (new AssertionScope())
        {
            error.Should().StartWith("Unable to run what-if: Mock what-if request failed");
            output.Should().BeEmpty();
            result.Should().Be(1);
        }
    }
}
