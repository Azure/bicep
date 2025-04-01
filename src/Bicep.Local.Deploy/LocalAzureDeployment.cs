// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Deployments.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Interfaces;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Azure.Deployments.Service.Shared.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Local.Deploy;

public static class LocalAzureDeployment
{
    public static async Task<LocalDeployment.Result> Deploy(RootConfiguration configuration, ITokenCredentialFactory credentialFactory, DeploymentLocator deploymentLocator, string templateString, string parametersString, CancellationToken cancellationToken)
    {
        var options = new ArmClientOptions();
        options.Diagnostics.ApplySharedResourceManagerSettings();
        options.Environment = new ArmEnvironment(configuration.Cloud.ResourceManagerEndpointUri, configuration.Cloud.AuthenticationScope);

        var credential = credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.CredentialOptions, configuration.Cloud.ActiveDirectoryAuthorityUri);
        var armClient = new ArmClient(credential, deploymentLocator.SubscriptionId, options);

        if (deploymentLocator.SubscriptionId == null)
        {
            throw new NotImplementedException("Above-subscription scope is not supported yet.");
        }

        var deploymentCollection = deploymentLocator switch {
            { SubscriptionId: {}, ResourceGroupName: null } => armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}")).GetArmDeployments(),
            { SubscriptionId: {}, ResourceGroupName: {} } => armClient.GetResourceGroupResource(new ResourceIdentifier($"/subscriptions/{deploymentLocator.SubscriptionId}/resourceGroups/{deploymentLocator.ResourceGroupName}")).GetArmDeployments(),
            _ => throw new NotImplementedException("Above-subscription scope is not supported yet."),
        };

        var deploymentProperties = new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(templateString),
            Parameters = BinaryData.FromString(parametersString),
        };
        var armDeploymentContent = new ArmDeploymentContent(deploymentProperties);

        if (deploymentLocator.ResourceGroupName == null)
        {
            // TODO avoid hardcoding this...
            armDeploymentContent.Location = "West US 3";
        }

        var deploymentOperation = await deploymentCollection.CreateOrUpdateAsync(WaitUntil.Completed, deploymentLocator.DeploymentName, armDeploymentContent, cancellationToken);

        var response = await deploymentOperation.WaitForCompletionAsync(cancellationToken);
        var content = response.GetRawResponse().Content.ToString().FromDeploymentsJson<DeploymentContent>();

        return new(content, []);
    }
}
