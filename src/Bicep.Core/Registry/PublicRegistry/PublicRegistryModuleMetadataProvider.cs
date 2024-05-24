// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Provider to get modules metadata that we store at a public endpoint.
    /// </summary>
    public class PublicRegistryModuleMetadataProvider : IPublicRegistryModuleMetadataProvider
    {
        private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);

        private readonly TimeSpan InitialThrottleDelay = TimeSpan.FromSeconds(5);

        private readonly TimeSpan MaxThrottleDelay = TimeSpan.FromMinutes(2);

        private readonly object queryingLiveSyncObject = new();

        private ImmutableArray<ModuleMetadata> cachedModules = [];

        private bool isQueryingLiveData = false;

        private DateTime? lastSuccessfulQuery;

        private int consecutiveFailures = 0;

        private readonly IServiceProvider serviceProvider;

        public PublicRegistryModuleMetadataProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            this.TryUpdateCacheInBackground(true);
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

        private void TryUpdateCacheInBackground(bool initialDelay = false)
        {
            if (!IsCacheExpired() && this.cachedModules.Any())
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

            _ = Task.Run(async () =>
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
                Trace.TraceInformation($"{nameof(PublicRegistryModuleMetadataProvider)}: Public modules cache is expired.");
            }

            return expired;
        }

        private async Task<ImmutableArray<ModuleMetadata>?> TryGetModulesLive()
        {
            try
            {
                var client = serviceProvider.GetRequiredService<IPublicRegistryModuleMetadataClient>();
                return await client.GetModuleMetadata();
            }
            catch
            {
                return null;
            }
        }

        // Modules paths are, e.g. "app/dapr-containerapp"
        public Task<IEnumerable<RegistryModule>> GetModules()
        {
            TryUpdateCacheInBackground();
            var modules = this.cachedModules.ToArray();
            return Task.FromResult(
                modules.Select(metadata =>
                    new RegistryModule(metadata.ModuleName, GetDescription(metadata), GetDocumentationUri(metadata))));
        }

        public Task<IEnumerable<RegistryModuleVersion>> GetVersions(string modulePath)
        {
            TryUpdateCacheInBackground();
            var modules = this.cachedModules.ToArray();
            ModuleMetadata? metadata = modules.FirstOrDefault(x => x.ModuleName.Equals(modulePath, StringComparison.Ordinal));
            if (metadata == null)
            {
                return Task.FromResult(Enumerable.Empty<RegistryModuleVersion>());
            }

            var versions = metadata.Tags.OrderDescending().ToArray() ?? Enumerable.Empty<string>();
            return Task.FromResult(
                versions.Select(v =>
                    new RegistryModuleVersion(v, GetDescription(metadata, v), GetDocumentationUri(metadata, v))));
        }

        private static string? GetDescription(ModuleMetadata moduleMetadata, string? version = null)
        {
            if (moduleMetadata.Properties is null)
            {
                return null;
            }

            if (version is null)
            {
                // Get description for most recent version with a description
                return moduleMetadata.Tags.Select(tag => moduleMetadata.Properties.TryGetValue(tag, out var propertiesEntry) ? propertiesEntry.Description : null)
                        .WhereNotNull().
                        LastOrDefault();
            }
            else
            {
                return moduleMetadata.Properties.TryGetValue(version, out var propertiesEntry) ? propertiesEntry.Description : null;
            }
        }

        private static string? GetDocumentationUri(ModuleMetadata moduleMetadata, string? version = null)
        {
            version ??= moduleMetadata.Tags.OrderDescending().FirstOrDefault();
            return version is null ? null : moduleMetadata.Properties[version].DocumentationUri;
        }

        public static TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay)
        {
            var maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
            var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
            var delay = TimeSpan.FromSeconds(secondsDelay);

            return delay > maxDelay ? maxDelay : delay;
        }
    }
}
