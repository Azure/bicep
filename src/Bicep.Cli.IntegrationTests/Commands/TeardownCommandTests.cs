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
public class TeardownCommandTests : TestBase
{
    private async Task<CliResult> Teardown(IDeploymentProcessor deploymentProcessor)
    {
        var bicepparamsPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
            var subscriptionId = readEnvVar('AZURE_SUBSCRIPTION_ID')
            var resourceGroup = readEnvVar('AZURE_RESOURCE_GROUP')

            using 'main.bicep' with {
              mode: 'stack'
              scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
              actionOnUnmanage: {
                resources: 'delete'
              }
              denySettings: {
                mode: 'denyDelete'
              }
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
            ["teardown", bicepparamsPath]);
    }

    [TestMethod]
    public async Task Teardown_should_succeed()
    {
        var deploymentProcessor = StrictMock.Of<IDeploymentProcessor>();
        deploymentProcessor.Setup(x => x.Teardown(It.IsAny<RootConfiguration>(), It.IsAny<DeployCommandsConfig>(), It.IsAny<Action<GeneralOperationView>>(), It.IsAny<CancellationToken>()))
            .Returns<RootConfiguration, DeployCommandsConfig, Action<GeneralOperationView>, CancellationToken>((_, config, onUpdate, _) =>
            {
                onUpdate(new("Teardown", "Succeeded", null));

                return Task.CompletedTask;
            });

        var result = await Teardown(deploymentProcessor.Object);

        result.WithoutAnsi().WithoutDurations().Stdout.Should().BeEquivalentToIgnoringNewlines("""
        ╭───────────┬──────────┬───────────╮
        │ Operation │ Duration │ Status    │
        ├───────────┼──────────┼───────────┤
        │ Teardown  │          │ Succeeded │
        ╰───────────┴──────────┴───────────╯

        """);
    }

    [TestMethod]
    public async Task Errors_are_displayed()
    {
        var deploymentProcessor = StrictMock.Of<IDeploymentProcessor>();
        deploymentProcessor.Setup(x => x.Teardown(It.IsAny<RootConfiguration>(), It.IsAny<DeployCommandsConfig>(), It.IsAny<Action<GeneralOperationView>>(), It.IsAny<CancellationToken>()))
            .Returns<RootConfiguration, DeployCommandsConfig, Action<GeneralOperationView>, CancellationToken>((_, config, onUpdate, _) =>
            {
                onUpdate(new("Teardown", "Failed", "Teardown error!"));

                return Task.CompletedTask;
            });

        var result = await Teardown(deploymentProcessor.Object);

        result.WithoutAnsi().WithoutDurations().Stdout.Should().BeEquivalentToIgnoringNewlines("""
        ╭───────────┬──────────┬─────────────────╮
        │ Operation │ Duration │ Status          │
        ├───────────┼──────────┼─────────────────┤
        │ Teardown  │          │ Teardown error! │
        ╰───────────┴──────────┴─────────────────╯

        """);
    }
}
