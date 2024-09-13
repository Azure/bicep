// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;

namespace Bicep.DeployManager
{
    public class DeployManager : IDeployManager
    {
        public async Task StartDeploymentAsync(DeploymentContent deploymentContent, CancellationToken cancellationToken)
        {

            var armDeploymentMode = deploymentContent.Properties.Mode == DeploymentMode.Complete ? ArmDeploymentMode.Complete : ArmDeploymentMode.Complete;
            var armDeploymentContent = new ArmDeploymentContent(new(armDeploymentMode)
            {
                // TODO: Convert properties.
            });

            var armClient = new ArmClient(new DefaultAzureCredential());
            var armDeploymentResource = armClient.GetArmDeploymentResource(new(deploymentContent.Id));

            armDeploymentResource.GetDeploymentOperationsAsync(cancellationToken);

            var longRunningOperation = await armDeploymentResource.UpdateAsync(Azure.WaitUntil.Started, armDeploymentContent, cancellationToken);

            var deploymentStatus = await longRunningOperation.(cancellationToken);
        }
    }
}
