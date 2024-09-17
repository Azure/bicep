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
using Bicep.Core.Extensions;
using Bicep.Core.Models;

namespace Bicep.Deploy
{
    public class DeploymentManager : IDeploymentManager
    {
        public async Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
        {
            var armClient = new ArmClient(new DefaultAzureCredential());

            try
            {
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
            catch (RequestFailedException ex)
            {
                throw new DeploymentException(ex.Message, ex);
            }
        }

        public async Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, Action<ImmutableSortedSet<ArmDeploymentOperation>>? onOperationsUpdated, CancellationToken cancellationToken)
        {
            // TODO: Custom polling logic. Take a look at:
            // - https://github.com/Azure/azure-powershell/blob/b819adaeec70e735e3b46d51c72bc2fc17c4fef6/src/Resources/ResourceManager/SdkClient/NewResourceManagerSdkClient.cs#L201
            // - Azure.Core.OperationPoller
            //throw new NotImplementedException();
            var armClient = new ArmClient(new DefaultAzureCredential());

            try
            {
                if (deploymentDefinition.SubscriptionId is { } subscriptionId &&
                    subscriptionId.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var subscription = await armClient.GetDefaultSubscriptionAsync(cancellationToken);
                    deploymentDefinition = deploymentDefinition with { SubscriptionId = subscription.Id };
                }

                var deploymentResource = armClient.GetArmDeploymentResource(deploymentDefinition.Id);
                var deploymentContent = new ArmDeploymentContent(deploymentDefinition.Properties);

                var operation = await deploymentResource.UpdateAsync(WaitUntil.Started, deploymentContent, cancellationToken);

                var currentOperations = new List<ArmDeploymentOperation>();
                while (!operation.HasCompleted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                    await operation.UpdateStatusAsync(cancellationToken);

                    var nextOperations = await GetDeploymentOperationsAsync(deploymentResource, cancellationToken);
                    var newOperations = GetNewOperations(currentOperations, nextOperations);
                    currentOperations.AddRange(newOperations);

                    onOperationsUpdated?.Invoke(newOperations.ToImmutableSortedSet(new ArmDeploymentOperationComparer()));
                }
                
                return operation.Value;
            }
            catch (RequestFailedException ex)
            {
                throw new DeploymentException(ex.Message, ex);
            }
        }

        public Task<ArmDeploymentValidateResult> ValidateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WhatIfOperationResult> WhatIfAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }   

        private async Task<List<ArmDeploymentOperation>> GetDeploymentOperationsAsync(ArmDeploymentResource deploymentResource, CancellationToken cancellationToken)
        {
            var results = new List<ArmDeploymentOperation>();
            await foreach (var operation in deploymentResource.GetDeploymentOperationsAsync().WithCancellation(cancellationToken))
            {
                results.Add(operation);
            }

            return results;
        }

        private List<ArmDeploymentOperation> GetNewOperations(List<ArmDeploymentOperation> currentOperations, List<ArmDeploymentOperation> nextOperations)
        {
            var newOperations = new List<ArmDeploymentOperation>();
            foreach (var operation in nextOperations)
            {
                var operationWithSameIdAndStatus = currentOperations.Find(o => o.OperationId.Equals(operation.OperationId) && o.Properties.ProvisioningState.Equals(operation.Properties.ProvisioningState));
                if (operationWithSameIdAndStatus is null)
                {
                    newOperations.Add(operation);
                }

                // TODO: Handle nested deployments
            }

            return newOperations;
        }

        private class ArmDeploymentOperationComparer : IComparer<ArmDeploymentOperation>
        {
            public int Compare(ArmDeploymentOperation? x, ArmDeploymentOperation? y)
            {
                if (x is null || y is null)
                {
                    return x is null ? (y is null ? 0 : -1) : 1;
                }

                return string.Compare(x.OperationId, y.OperationId, StringComparison.Ordinal);
            }
        }
    }
}
