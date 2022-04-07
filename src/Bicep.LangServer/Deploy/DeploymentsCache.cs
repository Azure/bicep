// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentsCache : IDeploymentsCache
    {
        private LastUsedDefaults? InputsUsedInPreviousDeployment { get; set; }

        private readonly ConcurrentDictionary<string, LastUsedDefaults> lastUsedDeploymentDefaultsPerFileCache = new ConcurrentDictionary<string, LastUsedDefaults>();

        public record LastUsedDefaults(string subscriptionId, string id, string parameterFilePath, string location);

        public void UpdateDeploymentsCache(string documentPath, LastUsedDefaults lastUsedDefaults)
        {
            InputsUsedInPreviousDeployment = lastUsedDefaults;
            lastUsedDeploymentDefaultsPerFileCache.AddOrUpdate(documentPath, documentPath => lastUsedDefaults, (documentPath, prevDefaults) => lastUsedDefaults);
        }

        public LastUsedDefaults? GetDeploymentDefaults(string documentPath)
        {
            if (lastUsedDeploymentDefaultsPerFileCache.TryGetValue(documentPath, out LastUsedDefaults? lastUsedDefaults) &&
                lastUsedDefaults is not null)
            {
                return lastUsedDefaults;
            }

            return InputsUsedInPreviousDeployment;
        }
    }
}
