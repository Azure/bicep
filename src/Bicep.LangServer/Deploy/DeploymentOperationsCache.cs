// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using System.Collections.Concurrent;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentOperationsCache : IDeploymentOperationsCache
    {
        private readonly ConcurrentDictionary<string, ArmOperation<ArmDeploymentResource>> deploymentOperationsCache = new ConcurrentDictionary<string, ArmOperation<ArmDeploymentResource>>();

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
