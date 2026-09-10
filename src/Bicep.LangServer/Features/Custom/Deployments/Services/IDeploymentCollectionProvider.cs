// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.LanguageServer.Features.Custom.Deployments.Services
{
    public interface IDeploymentCollectionProvider
    {
        ArmDeploymentCollection GetDeploymentCollection(ArmClient armClient, ResourceIdentifier resourceIdentifier, string scope);
    }
}
