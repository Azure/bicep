// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Bicep.Core;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentCollectionProvider : IDeploymentCollectionProvider
    {
        public DeploymentCollection? GetDeploymentCollection(ArmClient armClient, ResourceIdentifier resourceIdentifier, string scope)
        {
            switch (scope)
            {
                case LanguageConstants.TargetScopeTypeResourceGroup:
                    var resourceGroup = armClient.GetResourceGroup(resourceIdentifier);
                    return resourceGroup.GetDeployments();
                case LanguageConstants.TargetScopeTypeSubscription:
                    var subscription = armClient.GetSubscription(resourceIdentifier);
                    return subscription.GetDeployments();
                case LanguageConstants.TargetScopeTypeManagementGroup:
                    var managementGroup = armClient.GetManagementGroup(resourceIdentifier);
                    return managementGroup.GetDeployments();
                default:
                    throw new Exception(string.Format(LangServerResources.UnsupportedTargetScopeMessage, scope));
            }
        }
    }
}
