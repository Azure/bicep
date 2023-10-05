// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Microsoft;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Provider to get modules metadata that we store at a public endpoint.
    /// This endpoint caches module names and versions of modules available in this github repository - https://github.com/Azure/bicep-registry-modules
    /// </summary>
    public class PublicRegistryModuleMetadataProvider : IPublicRegistryModuleMetadataProvider
    {
        private record ModuleTagPropertiesEntry(string description);
        private record ModuleMetadata(string moduleName, List<string> tags, Dictionary<string /*key: tag*/, ModuleTagPropertiesEntry>? properties);

        private const string LiveDataEndpoint = "https://aka.ms/BicepModulesMetadata";
        private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);
        private readonly TimeSpan InitialThrottleDelay = TimeSpan.FromSeconds(5);
        private readonly TimeSpan MaxThrottleDelay = TimeSpan.FromMinutes(2);

        private ImmutableArray<ModuleMetadata> cachedModules = ImmutableArray<ModuleMetadata>.Empty;
        private bool isQueryingLiveData = false;
        private object queryingLiveSyncObject = new();
        private DateTime? lastSuccessfulQuery;
        private int consecutiveFailures = 0;
        private Func<Task<Stream>> getDataStream;

        public PublicRegistryModuleMetadataProvider(bool initializeCacheInBackground = false)
            : this((Func<Task<Stream>>?)null, initializeCacheInBackground)
        {
        }

        public PublicRegistryModuleMetadataProvider(string testData, bool initializeCache = false)
            : this(() => Task.FromResult<Stream>(new MemoryStream(UTF8Encoding.UTF8.GetBytes(testData))), initializeCache)
        {
        }

        private PublicRegistryModuleMetadataProvider(Func<Task<Stream>>? getDataStream = null, bool initializeCacheInBackground = false)
        {
            getDataStream ??= async () =>
            {
                using var httpClient = new HttpClient();
                return await httpClient.GetStreamAsync(LiveDataEndpoint);
            };
            this.getDataStream = getDataStream;

            if (initializeCacheInBackground)
            {
                this.CheckUpdateCacheAsync(true);
            }
        }

        public async Task<bool> TryUpdateCacheAsync()
        {

            if (await TryGetModulesLive() is { } modules)
            {
                this.cachedModules = modules;
                this.lastSuccessfulQuery = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CheckUpdateCacheAsync(bool initialDelay = false)
        {
            if (!IsCacheExpired() && cachedModules.Any())
            {
                return;
            }
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicRegistryModuleMetadataProvider)}: Cache expired");

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
                        await Task.Delay(InitialThrottleDelay);
                    }

                    if (await TryUpdateCacheAsync())
                    {
                        this.consecutiveFailures = 0;
                    }
                    else
                    {
                        this.consecutiveFailures++;
                    }
                }
                finally
                {
                    // Throttle requests to avoid spamming the endpoint with unsuccessful requests
                    var delay = GetExponentialDelay(InitialThrottleDelay, this.consecutiveFailures, MaxThrottleDelay);
                    Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataProvider)}: Delaying {delay}...");
                    await Task.Delay(delay);

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
                var metadataStream = await this.getDataStream();
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

        // Modules paths are, e.g. "app/dapr-containerapp"
        public Task<IEnumerable<PublicRegistryModule>> GetModules()
        {
            CheckUpdateCacheAsync();
            var modules = cachedModules.ToArray();
            return Task.FromResult(
                modules.Select(metadata =>
                    new PublicRegistryModule(metadata.moduleName, GetDescription(metadata), GetDocumentationUri(metadata))));
        }

        public Task<IEnumerable<PublicRegistryModuleVersion>> GetVersions(string modulePath)
        {
            CheckUpdateCacheAsync();
            var modules = cachedModules.ToArray();
            ModuleMetadata? metadata = modules.FirstOrDefault(x => x.moduleName.Equals(modulePath, StringComparison.Ordinal));
            if (metadata == null)
            {
                return Task.FromResult(Enumerable.Empty<PublicRegistryModuleVersion>());
            }

            var versions = metadata.tags.OrderDescending().ToArray() ?? Enumerable.Empty<string>();
            return Task.FromResult(
                versions.Select(v =>
                    new PublicRegistryModuleVersion(v, GetDescription(metadata, v), GetDocumentationUri(metadata, v))));
        }

        private static string? GetDescription(ModuleMetadata moduleMetadata, string? version = null)
        {
            if (moduleMetadata.properties is null)
            {
                return null;
            }

            if (version is null)
            {
                // Get description for most recent version with a description
                return moduleMetadata.tags.Select(tag => moduleMetadata.properties.TryGetValue(tag, out var propertiesEntry) ? propertiesEntry.description : null)
                        .WhereNotNull().
                        LastOrDefault();
            }
            else
            {
                return moduleMetadata.properties.TryGetValue(version, out var propertiesEntry) ? propertiesEntry.description : null;
            }
        }

        private string? GetDocumentationUri(ModuleMetadata moduleMetadata, string? version = null)
        {
            version ??= moduleMetadata.tags.OrderDescending().FirstOrDefault();
            return version is null ? null : OciModuleRegistry.GetPublicBicepModuleDocumentationUri(moduleMetadata.moduleName, version);
        }

        public TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay)
        {
            int maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
            var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
            var delay = TimeSpan.FromSeconds(secondsDelay);

            return delay > maxDelay ? maxDelay : delay;
        }
    }
}
