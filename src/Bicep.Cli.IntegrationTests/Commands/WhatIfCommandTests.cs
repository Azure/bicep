// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Azure;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Mocking;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.UnitTests;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Cli.UnitTests.Helpers.Deploy;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;

namespace Bicep.Cli.IntegrationTests.Commands;

[TestClass]
public class WhatIfCommandTests : TestBase
{
    private async Task<CliResult> WhatIf(IDeploymentProcessor deploymentProcessor)
    {
        var bicepparamsPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
            var subscriptionId = readEnvVar('AZURE_SUBSCRIPTION_ID')
            var resourceGroup = readEnvVar('AZURE_RESOURCE_GROUP')

            using './main.bicep' with {
              mode: 'deployment'
              scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
            }

            param stgName = 'asiudfndaisud'
            """);

        FileHelper.SaveResultFile(
            TestContext,
            "main.bicep",
            """
            param stgName string

            resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
              name: stgName
              location: 'West US'
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }

            output blobUri string = stg.properties.primaryEndpoints.blob
            """,
            Path.GetDirectoryName(bicepparamsPath));

        var subscriptionId = Guid.NewGuid().ToString();
        var resourceGroup = "testRg";
        var settings = CreateDefaultSettings() with
        {
            Environment = TestEnvironment.Default.WithVariables(
                ("AZURE_SUBSCRIPTION_ID", subscriptionId),
                ("AZURE_RESOURCE_GROUP", resourceGroup)),
            FeatureOverrides = CreateDefaultFeatureProviderOverrides() with { DeployCommandsEnabled = true }
        };

        return await Bicep(
            settings,
            services => services.AddSingleton(deploymentProcessor),
            TestContext.CancellationTokenSource.Token,
            ["what-if", bicepparamsPath]);
    }

    [TestMethod]
    public async Task WhatIf_should_succeed()
    {
        var deploymentProcessor = StrictMock.Of<IDeploymentProcessor>();
        deploymentProcessor.Setup(x => x.WhatIf(It.IsAny<RootConfiguration>(), It.IsAny<DeployCommandsConfig>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("""
            {
              "status": "Succeeded",
              "properties": {
                "changes": [
                  {
                    "resourceId": "/subscriptions/32f15881-632d-43b8-bedc-5785244dab01/resourceGroups/testRg/providers/Microsoft.ApiManagement/service/foo",
                    "changeType": "Modify",
                    "delta": [
                      {
                        "path": "sku.capacity",
                        "propertyChangeType": "Modify",
                        "before": 1,
                        "after": 3
                      },
                      {
                        "path": "zones",
                        "propertyChangeType": "Create",
                        "after": [
                          "1",
                          "2",
                          "3"
                        ]
                      }
                    ]
                  }
                ]
              }
            }
            """.FromJson<DeploymentWhatIfResponseDefinition>());

        var result = await WhatIf(deploymentProcessor.Object);

        result.WithVisibleAnsi().Stdout.Should().BeEquivalentToIgnoringNewlines("""

            Scope: /subscriptions/32f15881-632d-43b8-bedc-5785244dab01/resourceGroups/testRg
            [Purple]
              ~ Microsoft.ApiManagement/service/foo[Reset]
                [Green]+[Reset] zones[Reset]:[Reset] [Green][Reset][[Green]
                    0[Reset]:[Green] "1"
                    1[Reset]:[Green] "2"
                    2[Reset]:[Green] "3"
                  [Reset]][Green][Reset]
                [Purple]~[Reset] sku.capacity[Reset]:[Reset] [Orange]1[Reset] => [Green]3[Reset]
            [Purple][Reset]
            """);
    }
}
