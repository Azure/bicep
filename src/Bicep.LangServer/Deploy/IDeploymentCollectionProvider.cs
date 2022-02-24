// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.LanguageServer.Deploy
{
    public interface IDeploymentCollectionProvider
    {
        DeploymentCollection? GetDeployments(ArmClient armClient, ResourceIdentifier resourceIdentifier, string scope);
    }
}
