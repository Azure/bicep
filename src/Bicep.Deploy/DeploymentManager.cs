// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Models;

namespace Bicep.Deploy
{
    public class DeploymentManager : IDeploymentManager
    {
        public async Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
        {
            var armClient = new ArmClient(new DefaultAzureCredential());

            if (deploymentDefinition.SubscriptionId is { } subscriptionId &&
                subscriptionId.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var subscription = await armClient.GetDefaultSubscriptionAsync(cancellationToken);
                deploymentDefinition = deploymentDefinition with { SubscriptionId = subscription.Id };
            }

            var deploymentResource = armClient.GetArmDeploymentResource(deploymentDefinition.Id);
            var deploymentContent = new ArmDeploymentContent(deploymentDefinition.Properties);

            var operation = await deploymentResource.UpdateAsync(WaitUntil.Completed, deploymentContent, cancellationToken);

            return operation.Value;
        }

        public Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, Action<ImmutableSortedSet<ArmDeploymentOperation>>? onOperationsUpdated, CancellationToken cancellationToken)
        {
            // TODO: Custom polling logic. Take a look at:
            // - https://github.com/Azure/azure-powershell/blob/b819adaeec70e735e3b46d51c72bc2fc17c4fef6/src/Resources/ResourceManager/SdkClient/NewResourceManagerSdkClient.cs#L201
            // - Azure.Core.OperationPoller
            throw new NotImplementedException();
        }

        public Task<ArmDeploymentValidateResult> ValidateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WhatIfOperationResult> WhatIfAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
