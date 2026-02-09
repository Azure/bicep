// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class BicepDeploymentToolsTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    private static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services
            .AddBicepMcpServer();

        return services.BuildServiceProvider();
    }

    private readonly BicepDeploymentTools tools = GetServiceProvider().GetRequiredService<BicepDeploymentTools>();

    [TestMethod]
    public async Task GetDeploymentSnapshot_returns_a_valid_snapshot()
    {
        var outputFolder = FileHelper.SaveResultFiles(TestContext, [
            new("main.bicep", """
                @description('Storage Account type')
                param storageAccountType string = 'Standard_LRS'
                
                @description('The storage account location.')
                param location string = resourceGroup().location
                
                @description('The name of the storage account')
                param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'
                
                resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                  name: storageAccountName
                  location: location
                  sku: {
                    name: storageAccountType
                  }
                  kind: 'StorageV2'
                  properties: {}
                }
                """),
            new("main.bicepparam", """
                using './main.bicep'

                param location = 'eastus'
                """),
        ]);

        var response = await tools.GetDeploymentSnapshot(
            filePath: Path.Combine(outputFolder, "main.bicepparam"),
            tenantId: null,
            subscriptionId: "1ec1dd71-d88e-465d-95e2-4996c828833a",
            resourceGroup: "myRg",
            location: "antartica",
            deploymentName: null,
            cancellationToken: default);
        JsonSerializer.Serialize(response).Should().DeepEqualJson("""
        {
          "PredictedResources": [
            {
              "id": "/subscriptions/1ec1dd71-d88e-465d-95e2-4996c828833a/resourceGroups/myRg/providers/Microsoft.Storage/storageAccounts/storejztimcr36svny",
              "type": "Microsoft.Storage/storageAccounts",
              "name": "storejztimcr36svny",
              "apiVersion": "2022-09-01",
              "location": "eastus",
              "sku": {
                "name": "Standard_LRS"
              },
              "kind": "StorageV2",
              "properties": {}
            }
          ],
          "Diagnostics": []
        }
        """);
    }
}
