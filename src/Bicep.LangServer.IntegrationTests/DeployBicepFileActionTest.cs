// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.ResourceManager;
using Bicep.Core;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using TextRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    public class MockArmClientProvider : IArmClientProvider
    {
        public ArmClient createArmClient(TokenCredential credential, string? defaultSubscriptionId, ArmClientOptions options)
        {
            var clientMock = StrictMock.Of<ArmClient>();

            return clientMock.Object;
        }
    }

    [TestClass]
    public class DeployBicepFileActionTest
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task IncorrectUpdateOption_WithBicepparamFile_ReturnsError()
        {   
            var bicepFileUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var bicepparamFileUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");

            var fileTextsByUri = new Dictionary<Uri, string>() 
            {
                [bicepFileUri] = "",
                [bicepparamFileUri] = ""
            };

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext, 
                fileTextsByUri,
                bicepFileUri, 
                services => services
                .WithFeatureOverrides(new(TestContext, ParamsFilesEnabled: true))
                .WithArmClientProvider(new MockArmClientProvider()));

            var client = helper.Client;
            var bicepDeploymentStartParams = new BicepDeploymentStartParams(
                bicepFileUri.AbsolutePath, 
                bicepparamFileUri.AbsolutePath, 
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                "fake-token-123",
                "123456",
                string.Empty,
                string.Empty,
                string.Empty,
                true,
                string.Empty,
                LanguageServer.Deploy.ParametersFileUpdateOption.Create,
                new List<BicepUpdatedDeploymentParameter>(),
                "https://management.azure.com/",
                "https://management.core.windows.net/");

            var deploymentStartResponse = await client.Workspace.ExecuteCommandWithResponse<BicepDeploymentStartResponse>(
                new Command {
                    Name = "deploy/start",
                    Arguments = new JArray(
                        bicepDeploymentStartParams.ToJToken()
                    )
                }
            );

            deploymentStartResponse.isSuccess.Should().BeFalse();
            deploymentStartResponse.outputMessage.Should().Be("Cannot create/overwrite/update parameter files when using a bicep parameters file");
        }

        [TestMethod]
        public async Task ProvidingUpdateParameterValues_WithBicepparamFile_ReturnsError()
        {   
            var bicepFileUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var bicepparamFileUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");

            var fileTextsByUri = new Dictionary<Uri, string>() 
            {
                [bicepFileUri] = "",
                [bicepparamFileUri] = ""
            };

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext, 
                fileTextsByUri,
                bicepFileUri, 
                services => services
                .WithFeatureOverrides(new(TestContext, ParamsFilesEnabled: true))
                .WithArmClientProvider(new MockArmClientProvider()));

            var client = helper.Client;
            var bicepDeploymentStartParams = new BicepDeploymentStartParams(
                bicepFileUri.AbsolutePath, 
                bicepparamFileUri.AbsolutePath, 
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                "fake-token-123",
                "123456",
                string.Empty,
                string.Empty,
                string.Empty,
                true,
                string.Empty,
                LanguageServer.Deploy.ParametersFileUpdateOption.None,
                new List<BicepUpdatedDeploymentParameter>{
                    new BicepUpdatedDeploymentParameter("foo", "bar", false, null)
                },
                "https://management.azure.com/",
                "https://management.core.windows.net/");

            var deploymentStartResponse = await client.Workspace.ExecuteCommandWithResponse<BicepDeploymentStartResponse>(
                new Command {
                    Name = "deploy/start",
                    Arguments = new JArray(
                        bicepDeploymentStartParams.ToJToken()
                    )
                }
            );

            deploymentStartResponse.isSuccess.Should().BeFalse();
            deploymentStartResponse.outputMessage.Should().Be("Cannot update parameters for bicep parameter file");
        }
    }
}
