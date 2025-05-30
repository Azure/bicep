// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions;
using Bicep.Core.Configuration;

namespace Bicep.Local.Deploy.Azure;

public interface IArmDeploymentProvider
{
    Task<LocalDeploymentResult> CheckDeployment(RootConfiguration configuration, DeploymentLocator deploymentLocator, CancellationToken cancellationToken);

    Task StartDeployment(RootConfiguration configuration, DeploymentLocator deploymentLocator, string templateString, string parametersString, CancellationToken cancellationToken);
}
