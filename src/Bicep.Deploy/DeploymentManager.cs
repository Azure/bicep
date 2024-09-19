// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Identifiers;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Models;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;
using Bicep.Deploy.Exceptions;
using ValidationException = Bicep.Deploy.Exceptions.ValidationException;

namespace Bicep.Deploy;

public class DeploymentManager : IDeploymentManager
{
    private readonly ArmClient armClient;

    public DeploymentManager(ArmClient armClient)
    {
        this.armClient = armClient;
    }

    public async Task<ArmDeploymentResource> CreateOrUpdateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
    {
        try
        {
            deploymentDefinition = await HandleEmptySubscriptionIdAsync(deploymentDefinition, cancellationToken);

            var deploymentResource = this.armClient.GetArmDeploymentResource(deploymentDefinition.Id);
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
        try
        {
            deploymentDefinition = await HandleEmptySubscriptionIdAsync(deploymentDefinition, cancellationToken);

            var deploymentResource = this.armClient.GetArmDeploymentResource(deploymentDefinition.Id);
            var deploymentContent = new ArmDeploymentContent(deploymentDefinition.Properties);

            var deploymentOperation = await deploymentResource.UpdateAsync(WaitUntil.Started, deploymentContent, cancellationToken);
            var currentOperations = ImmutableSortedSet.Create<ArmDeploymentOperation>(Comparer<ArmDeploymentOperation>.Create((x, y) => 
            {
                return string.Compare(x.Properties.TargetResource?.Id, y.Properties.TargetResource?.Id, StringComparison.Ordinal);
            }));

            while (!deploymentOperation.HasCompleted)
            {
                // TODO: Handle nested deployments
                await deploymentOperation.UpdateStatusAsync(cancellationToken);
                await foreach (var operation in deploymentResource.GetDeploymentOperationsAsync().WithCancellation(cancellationToken))
                {
                    currentOperations = currentOperations.Add(operation);
                }

                onOperationsUpdated?.Invoke(currentOperations);
                
                // TODO: Retry-After header doesn't seem to be present in the response
                // even tho it does show in the portal. Need to investigate further.
                // Potentially implement more sophisticated retry logic here i.e.
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }

            return deploymentOperation.Value;
        }
        catch (RequestFailedException ex)
        {
            throw new DeploymentException(ex.Message, ex);
        }
    }

    public async Task<ArmDeploymentValidateResult> ValidateAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
    {
        try
        {
            deploymentDefinition = await HandleEmptySubscriptionIdAsync(deploymentDefinition, cancellationToken);

            var deploymentResource = this.armClient.GetArmDeploymentResource(deploymentDefinition.Id);
            var deploymentContent = new ArmDeploymentContent(deploymentDefinition.Properties);

            var validationResult = await deploymentResource.ValidateAsync(WaitUntil.Completed, deploymentContent, cancellationToken);

            return validationResult.Value;
        }
        catch (RequestFailedException ex)
        {
            throw new ValidationException(ex.Message, ex);
        }
    }

    public async Task<WhatIfOperationResult> WhatIfAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
    {
        try
        {
            deploymentDefinition = await HandleEmptySubscriptionIdAsync(deploymentDefinition, cancellationToken);

            var deploymentResource = this.armClient.GetArmDeploymentResource(deploymentDefinition.Id);
            if (deploymentDefinition.Properties is not ArmDeploymentWhatIfProperties whatIfProperties)
            {
                throw new WhatIfException("What-if properties are required for a what-if operation.");
            }

            var deploymentContent = new ArmDeploymentWhatIfContent(whatIfProperties);

            var whatIfResult = await deploymentResource.WhatIfAsync(WaitUntil.Completed, deploymentContent, cancellationToken);

            return whatIfResult.Value;
        }
        catch (RequestFailedException ex)
        {
            throw new WhatIfException(ex.Message, ex);
        }
    }

    private async Task<ArmDeploymentDefinition> HandleEmptySubscriptionIdAsync(ArmDeploymentDefinition deploymentDefinition, CancellationToken cancellationToken)
    {
        if (deploymentDefinition.SubscriptionId is { } subscriptionId &&
                subscriptionId.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            var subscription = await this.armClient.GetDefaultSubscriptionAsync(cancellationToken);
            deploymentDefinition = deploymentDefinition with { SubscriptionId = subscription.Data.SubscriptionId };
        }

        return deploymentDefinition;
    }
}
