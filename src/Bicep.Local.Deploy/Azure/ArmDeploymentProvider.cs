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
}
