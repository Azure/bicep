// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.LanguageServer.Deploy
{
    public interface IDeploymentOperationsCache
    {
        public void AddToCache(string deploymentId, ArmOperation<ArmDeploymentResource> armOperation);

        public ArmOperation<ArmDeploymentResource>? GetDeploymentOperationFromCache(string deploymentId);
    }
}
