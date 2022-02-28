// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Bicep.Core;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentCollectionProvider : IDeploymentCollectionProvider
    {
        public DeploymentCollection? GetDeployments(ArmClient armClient, ResourceIdentifier resourceIdentifier, string scope)
        {
            if (scope == LanguageConstants.TargetScopeTypeResourceGroup)
            {
                var resourceGroup = armClient.GetResourceGroup(resourceIdentifier);
                return resourceGroup.GetDeployments();
            }
            else if (scope == LanguageConstants.TargetScopeTypeSubscription)
            {
                var subscription = armClient.GetSubscription(resourceIdentifier);
                return subscription.GetDeployments();
            }
            else if (scope == LanguageConstants.TargetScopeTypeManagementGroup)
            {
                var managementGroup = armClient.GetManagementGroup(resourceIdentifier);
                return managementGroup.GetDeployments();
            }

            return null;
        }
    }
}
