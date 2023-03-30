// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
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
        private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);
        private readonly TimeSpan ThrottleDelay = TimeSpan.FromSeconds(5);

        private ImmutableArray<ModuleMetadata> cachedModules = ImmutableArray<ModuleMetadata>.Empty;
        private bool isQueryingLiveData = false;
        private object queryingLiveSyncObject = new();
        private DateTime? lastSuccessfulQuery;

        public PublicRegistryModuleMetadataProvider(bool initializeCache = false)
        {
            if (initializeCache)
            {
                this.CheckUpdateCacheAsync(true);
            }
        }

        private void CheckUpdateCacheAsync(bool initialDelay = false)
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
                        // Allow language server to start up a bit before first hit
                        await Task.Delay(ThrottleDelay);
                    }

                    if (await TryGetModulesLive() is { } modules)
                    {
                        this.cachedModules = modules;
                    }

                    this.lastSuccessfulQuery = DateTime.Now;
                }
                finally
                {
                    // Throttle unsuccessful requests to avoid spamming the endpoint
                    await Task.Delay(ThrottleDelay);

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
            var expired = this.lastSuccessfulQuery.HasValue && this.lastSuccessfulQuery.Value + this.CacheValidFor < DateTime.Now;
            if (expired)
            {
                Trace.TraceInformation("Public modules cache is expired.");
            }

            return expired;
        }

        private async Task<ImmutableArray<ModuleMetadata>?> TryGetModulesLive()
        {
            Trace.WriteLine($"Retrieving list of public registry modules...");

            try
            {
                HttpClient httpClient = new HttpClient();
                using var metadataStream = await httpClient.GetStreamAsync(LiveDataEndpoint);
                var metadata = JsonSerializer.Deserialize<ModuleMetadata[]>(metadataStream);

                if (metadata is not null)
                {
                    return metadata.ToImmutableArray();
                }
                else
                {
                    throw new Exception($"List of MCR modules at {LiveDataEndpoint} was empty");
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(string.Format("Error retrieving MCR modules metadata: {0}", e.Message));
                return null;
            }
        }

        // e.g. "app/dapr-containerapp"
        public Task<IEnumerable<string>> GetModuleNames()
        {
            CheckUpdateCacheAsync();
            var modules = cachedModules.ToArray();
            return Task.FromResult(modules.Select(m => m.moduleName));
        }

        public Task<IEnumerable<string>> GetVersions(string moduleName)
        {
            CheckUpdateCacheAsync();
            var modules = cachedModules.ToArray();
            ModuleMetadata? metadata = modules.FirstOrDefault(x => x.moduleName.Equals(moduleName, StringComparison.Ordinal));
            var result = metadata?.tags.OrderDescending().ToArray() ?? Enumerable.Empty<string>();
            return Task.FromResult(result);
        }
    }
}
