// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions;
using Bicep.Core.Configuration;
using static Bicep.Local.Deploy.Extensibility.NestedDeploymentExtension;

namespace Bicep.Local.Deploy.Azure;

public interface IArmDeploymentProvider
{
    Task CreateResourceGroup(IBicepConfiguration configuration, DeploymentLocator deploymentLocator, string location, CancellationToken cancellationToken);

    Task<LocalDeploymentResult> CheckDeployment(IBicepConfiguration configuration, DeploymentLocator deploymentLocator, CancellationToken cancellationToken);

    Task StartDeployment(IBicepConfiguration configuration, DeploymentLocator deploymentLocator, string templateString, string parametersString, CancellationToken cancellationToken);

    Task<LocalDeploymentResult> CheckDeploymentStack(IBicepConfiguration configuration, DeploymentLocator deploymentLocator, CancellationToken cancellationToken);

    Task StartDeploymentStack(IBicepConfiguration configuration, DeploymentLocator deploymentLocator, string templateString, string parametersString, StacksConfig stacksConfig, CancellationToken cancellationToken);
}
