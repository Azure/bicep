// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Bicep.Core;
using System;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentCollectionProvider : IDeploymentCollectionProvider
    {
        public ArmDeploymentCollection GetDeploymentCollection(ArmClient armClient, ResourceIdentifier resourceIdentifier, string scope)
        {
            switch (scope)
            {
                case LanguageConstants.TargetScopeTypeResourceGroup:
                    var resourceGroup = armClient.GetResourceGroupResource(resourceIdentifier);
                    return resourceGroup.GetArmDeployments();
                case LanguageConstants.TargetScopeTypeSubscription:
                    var subscription = armClient.GetSubscriptionResource(resourceIdentifier);
                    return subscription.GetArmDeployments();
                case LanguageConstants.TargetScopeTypeManagementGroup:
                    var managementGroup = armClient.GetManagementGroupResource(resourceIdentifier);
                    return managementGroup.GetArmDeployments();
                default:
                    throw new Exception(string.Format(LangServerResources.UnsupportedTargetScopeMessage, scope));
            }
        }
    }
}
