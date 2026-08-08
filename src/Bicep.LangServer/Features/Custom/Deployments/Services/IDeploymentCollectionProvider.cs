// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.LanguageServer.Deploy
{
    public interface IDeploymentCollectionProvider
    {
        ArmDeploymentCollection GetDeploymentCollection(ArmClient armClient, ResourceIdentifier resourceIdentifier, string scope);
    }
}
