// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Providers
{
    public record ModuleMetadata(string moduleName, List<string> tags);

    /// <summary>
    /// Provider to get modules metadata that we store at a public endpoint.
    /// This endpoint caches module names and versions of modules available in this github repository - https://github.com/Azure/bicep-registry-modules
    /// </summary>
    public class PublicRegistryModuleMetadataProvider : IPublicRegistryModuleMetadataProvider
    {
        private const string LiveDataEndpoint = "https://aka.ms/BicepModulesMetadata";
        private readonly TimeSpan cacheValidFor = TimeSpan.FromHours(1);

        private ModuleMetadata[] cachedModules = new ModuleMetadata[] { };
        private bool isQueryingLiveData = false;
        private object queryingLiveSyncObject = new();
        private DateTime? lastSuccessfulQuery;

        public PublicRegistryModuleMetadataProvider(bool initializeCache = false)
        {
            if (initializeCache)
            {
                this.UpdateCacheAsync(true);
            }
        }

        private void UpdateCacheAsync(bool initialDelay = false)
        {
            if (!IsCacheExpired() && cachedModules.Any())
            {
                return;
            }

            lock (this.queryingLiveSyncObject)
            {
                if (this.isQueryingLiveData)
                {
                    return;
                }

                this.isQueryingLiveData = true;
            }

            var _ = Task.Run(async () =>
            {
                try
                {
                    if (initialDelay)
                    {
                        await Task.Delay(5000);
                    }

                    var modules = await TryGetModulesLive();

                    this.cachedModules = modules;
                    this.lastSuccessfulQuery = DateTime.Now;
                }
                finally
                {
                    // Throttle unsuccessful requests to avoid spamming the endpoint
                    await Task.Delay(5000);

                    lock (this.queryingLiveSyncObject)
                    {
                        Trace.Assert(this.isQueryingLiveData, "this.isQueryingLiveData should be true");
                        this.isQueryingLiveData = false;
                    }
                }
            });
        }

        private bool IsCacheExpired()
        {
            var expired = this.lastSuccessfulQuery.HasValue && this.lastSuccessfulQuery.Value + this.cacheValidFor < DateTime.Now;
            if (expired)
            {
                Trace.TraceInformation("Public modules cache is expired.");
            }

            return expired;
        }

        private async Task<ModuleMetadata[]> TryGetModulesLive()
        {
            Trace.WriteLine($"Retrieving list of public registry modules...");

            try
            {
                HttpClient httpClient = new HttpClient();
                var moduleMetadata = await httpClient.GetStringAsync(LiveDataEndpoint);
                var metadata = JsonConvert.DeserializeObject<ModuleMetadata[]>(moduleMetadata);

                if (metadata is not null)
                {
                    return metadata;
                }
                else
                {
                    throw new Exception($"List of MCR modules at {LiveDataEndpoint} was empty");
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(string.Format("Error retrieving MCR modules metadata: {0}", e.Message));
                return new ModuleMetadata[] { };
            }
        }

        // e.g. "app/dapr-containerapp"
        public Task<IEnumerable<string>> GetModuleNames()
        {
            UpdateCacheAsync();
            var modules = cachedModules.ToArray();
            return Task.FromResult(modules.Select(m => m.moduleName));
        }

        public Task<IEnumerable<string>> GetVersions(string moduleName)
        {
            UpdateCacheAsync();
            var modules = cachedModules.ToArray();
            ModuleMetadata? metadata = modules.FirstOrDefault(x => x.moduleName.Equals(moduleName, StringComparison.Ordinal));
            var result = metadata?.tags.OrderDescending().ToArray() ?? Enumerable.Empty<string>();
            return Task.FromResult(result);
        }
    }
}
