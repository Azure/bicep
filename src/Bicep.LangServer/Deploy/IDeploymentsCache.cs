// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using static Bicep.LanguageServer.Deploy.DeploymentsCache;

namespace Bicep.LanguageServer.Deploy
{
    public interface IDeploymentsCache
    {
        public LastUsedDefaults? GetDeploymentDefaults(string documentPath);

        public void UpdateDeploymentsCache(string documentPath, LastUsedDefaults lastUsedDefaults);
    }
}
