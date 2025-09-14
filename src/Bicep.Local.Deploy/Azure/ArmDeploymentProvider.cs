// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Deployments.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Interfaces;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Tracing;
using Bicep.Local.Deploy.Engine;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using static Bicep.Local.Deploy.Extensibility.NestedDeploymentExtension;

namespace Bicep.Local.Deploy.Azure;

public class ArmDeploymentProvider(
    IArmClientProvider armClientProvider) : IArmDeploymentProvider
{
    private ArmDeploymentCollection GetDeploymentsClient(RootConfiguration configuration, DeploymentLocator deploymentLocator)
    {
        var armClient = armClientProvider.CreateArmClient(configuration, deploymentLocator.SubscriptionId);

        if (deploymentLocator.SubscriptionId == null)
        {
            throw new NotImplementedException("Above-subscription scope is not supported yet.");
        }

        return deploymentLocator switch
        {
            { SubscriptionId: { }, ResourceGroupName: null } => armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}")).GetArmDeployments(),
            { SubscriptionId: { }, ResourceGroupName: { } } => armClient.GetResourceGroupResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}/resourceGroups/{deploymentLocator.ResourceGroupName}")).GetArmDeployments(),
            _ => throw new NotImplementedException("Above-subscription scope is not supported yet."),
        };
    }
 
    private DeploymentStackCollection GetDeploymentStacksClient(RootConfiguration configuration, DeploymentLocator deploymentLocator)
    {
        var armClient = armClientProvider.CreateArmClient(configuration, deploymentLocator.SubscriptionId);

        if (deploymentLocator.SubscriptionId == null)
        {
            throw new NotImplementedException("Above-subscription scope is not supported yet.");
        }

        return deploymentLocator switch
        {
            { SubscriptionId: { }, ResourceGroupName: null } => armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}")).GetDeploymentStacks(),
            { SubscriptionId: { }, ResourceGroupName: { } } => armClient.GetResourceGroupResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}/resourceGroups/{deploymentLocator.ResourceGroupName}")).GetDeploymentStacks(),
            _ => throw new NotImplementedException("Above-subscription scope is not supported yet."),
        };
    }

    public async Task CreateResourceGroup(RootConfiguration configuration, DeploymentLocator deploymentLocator, string location, CancellationToken cancellationToken)
    {
        var armClient = armClientProvider.CreateArmClient(configuration, deploymentLocator.SubscriptionId);
        var resourceGroups = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}")).GetResourceGroups();

        await resourceGroups.CreateOrUpdateAsync(WaitUntil.Completed, deploymentLocator.ResourceGroupName, new ResourceGroupData(new(location)), cancellationToken);
    }

    public async Task StartDeployment(RootConfiguration configuration, DeploymentLocator deploymentLocator, string templateString, string parametersString, CancellationToken cancellationToken)
    {
        var deploymentsClient = GetDeploymentsClient(configuration, deploymentLocator);

        var paramsDefinition = parametersString.FromJson<DeploymentParametersDefinition>();
        var deploymentProperties = new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(templateString),
            Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
        };
        var armDeploymentContent = new ArmDeploymentContent(deploymentProperties);

        if (deploymentLocator.ResourceGroupName == null)
        {
            // TODO avoid hardcoding this...
            armDeploymentContent.Location = "West US 3";
        }

        await deploymentsClient.CreateOrUpdateAsync(WaitUntil.Started, deploymentLocator.DeploymentName, armDeploymentContent, cancellationToken);
    }

    public async Task<LocalDeploymentResult> CheckDeployment(RootConfiguration configuration, DeploymentLocator deploymentLocator, CancellationToken cancellationToken)
    {
        var deploymentsClient = GetDeploymentsClient(configuration, deploymentLocator);

        var response = await deploymentsClient.GetAsync(deploymentLocator.DeploymentName, cancellationToken);
        var content = response.GetRawResponse().Content.ToString().FromDeploymentsJson<DeploymentContent>();

        return new(content, []);
    }

    public async Task StartDeploymentStack(RootConfiguration configuration, DeploymentLocator deploymentLocator, string templateString, string parametersString, StacksConfig stacksConfig, CancellationToken cancellationToken)
    {
        var stacksClient = GetDeploymentStacksClient(configuration, deploymentLocator);

        var paramsDefinition = parametersString.FromJson<DeploymentParametersDefinition>();

        DeploymentStackData stacksData = new()
        {
            Description = stacksConfig.Description,
            ActionOnUnmanage = stacksConfig.ActionOnUnmanage,
            DenySettings = stacksConfig.DenySettings,
            Template = BinaryData.FromString(templateString),
        };

        foreach (var kvp in paramsDefinition.Parameters ?? [])
        {
            stacksData.Parameters[kvp.Key] = new DeploymentParameter()
            {
                // TODO handle expressions, external inputs
                Value = BinaryData.FromString(kvp.Value.Value.ToJson()),
            };
        }

        await stacksClient.CreateOrUpdateAsync(WaitUntil.Started, deploymentLocator.DeploymentName, stacksData, cancellationToken);
    }

    public async Task<LocalDeploymentResult> CheckDeploymentStack(RootConfiguration configuration, DeploymentLocator deploymentLocator, CancellationToken cancellationToken)
    {
        var stacksClient = GetDeploymentStacksClient(configuration, deploymentLocator);

        string? entrypointDeploymentId = null;
        while (entrypointDeploymentId is null)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            var stacksResponse = await stacksClient.GetAsync(deploymentLocator.DeploymentName, cancellationToken);
            entrypointDeploymentId = stacksResponse.Value.Data.DeploymentId;
        }

        return await CheckDeployment(configuration, deploymentLocator with { DeploymentName = entrypointDeploymentId.Split('/').Last(), }, cancellationToken);
    }
}
