// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Azure.ResourceManager;
using Bicep.Cli.Services;
using Bicep.Cli;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using Bicep.LanguageServer;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDeploymentStartCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataRow("AzureCloud", "https://management.azure.com/", "https://management.azure.com")]
        [DataRow("AzureUSGovernment", "https://management.usgovcloudapi.net/", "https://management.usgovcloudapi.net")]
        [DataTestMethod]
        public void GetArmClientOptions(string environmentName, string expectedEndpointPath, string expectedAudience)
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepFileContents = @"param test string = 'abc'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);

            var bicepConfigFileContents = @"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""VisualStudioCode""
    ]
  }
}";
            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);

            var bicepDeploymentStartCommandHandler = GetBicepDeploymentStartCommandHandler();

            ArmClientOptions result = bicepDeploymentStartCommandHandler.GetArmClientOptions(bicepFilePath, environmentName);
            var environment = result.Environment;

            environment.Should().NotBeNull();
            environment!.Value.Endpoint.AbsoluteUri.Should().Be(expectedEndpointPath);
            environment!.Value.Audience.Should().Be(expectedAudience);
        }

        [TestMethod]
        public void GetArmClientOptions_WithInvalidEnvironmentName_ShouldThrow()
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepFileContents = @"param test string = 'abc'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);

            var bicepConfigFileContents = @"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      }""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""VisualStudioCode""
    ]
  }
}";
            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);
            var bicepDeploymentStartCommandHandler = GetBicepDeploymentStartCommandHandler();
            Action action = () => bicepDeploymentStartCommandHandler.GetArmClientOptions(bicepFilePath, "Invalid_Environment_Name");

            action.Should().Throw<Exception>().WithMessage(string.Format(LangServerResources.InvalidResourceManagerEndpoint, bicepFilePath));
        }

        private BicepDeploymentStartCommandHandler GetBicepDeploymentStartCommandHandler()
        {
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>().Object;
            var deploymentOperationsCache = StrictMock.Of<IDeploymentOperationsCache>().Object;
            var serializer = StrictMock.Of<ISerializer>().Object;
            var telemetryProvider = StrictMock.Of<ITelemetryProvider>().Object;

            return new BicepDeploymentStartCommandHandler(BicepTestConstants.ConfigurationManager, deploymentCollectionProvider, deploymentOperationsCache, serializer, telemetryProvider);
        }
    }
}
