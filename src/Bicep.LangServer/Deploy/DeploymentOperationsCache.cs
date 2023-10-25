// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentOperationsCache : IDeploymentOperationsCache
    {
        private readonly ConcurrentDictionary<string, ArmOperation<ArmDeploymentResource>> deploymentOperationsCache = new();

        public void CacheDeploymentOperation(string deploymentId, ArmOperation<ArmDeploymentResource> armOperation)
        {
            deploymentOperationsCache.TryAdd(deploymentId, armOperation);
        }

        public ArmOperation<ArmDeploymentResource>? FindAndRemoveDeploymentOperation(string deploymentId)
        {
            if (deploymentOperationsCache.TryRemove(deploymentId, out ArmOperation<ArmDeploymentResource>? armDeploymentOperation) &&
                armDeploymentOperation is not null)
            {
                return armDeploymentOperation;
            }

            return null;
        }
    }
}
