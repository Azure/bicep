// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

public static class PublicRegistryModuleMetadataProviderExtensions
{
    public static IServiceCollection AddPublicRegistryModuleMetadataProviderServices(this IServiceCollection services)
    {
        services.AddSingleton<IPublicRegistryModuleMetadataProvider, PublicRegistryModuleMetadataProvider>();

        // using type based registration for Http clients so dependencies can be injected automatically
        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        services
            .AddHttpClient<IPublicRegistryModuleMetadataClient, PublicRegistryModuleMetadataClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

        return services;
    }
}


/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// </summary>
public class PublicRegistryModuleMetadataProvider : IPublicRegistryModuleMetadataProvider
{
    private readonly IServiceProvider serviceProvider;

    private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);
    private readonly TimeSpan InitialThrottleDelay = TimeSpan.FromSeconds(5);
    private readonly TimeSpan MaxThrottleDelay = TimeSpan.FromMinutes(2);

    private readonly object queryingLiveSyncObject = new();
    private Task? queryLiveDataTask;
    private DateTime? lastSuccessfulQuery;
    private int consecutiveFailures = 0;

    private ImmutableArray<BicepModuleMetadata> cachedModules = [];
    private string? lastDownloadError = null;

    public bool IsCached => cachedModules.Length > 0;

    public string? DownloadError => IsCached ? null : lastDownloadError;

    public PublicRegistryModuleMetadataProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task TryAwaitCache(bool forceUpdate)
    {
        return UpdateCacheIfNeeded(forceUpdate: forceUpdate, initialDelay: false);
    }

    public void StartUpdateCache(bool forceUpdate) {
        _ = TryAwaitCache(forceUpdate);
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

    // If cache has not yet successfully been updated, returns empty
    public RegistryModule[] GetCachedModules()
    {
        StartCacheUpdateInBackgroundIfNeeded();

        var modules = this.cachedModules.ToArray();
        return modules.Select(metadata =>
            new RegistryModule(metadata.ModuleName, GetDescription(metadata), GetDocumentationUri(metadata)))
            .ToArray();
    }

    public RegistryModuleVersion[] GetCachedModuleVersions(string modulePath)
    {
        StartCacheUpdateInBackgroundIfNeeded();

        var modules = this.cachedModules.ToArray();
        BicepModuleMetadata? metadata = modules.FirstOrDefault(x => x.ModuleName.Equals(modulePath, StringComparison.Ordinal));
        if (metadata == null)
        {
            return [];
        }

        var versions = metadata.Tags.OrderDescending().ToArray() ?? Enumerable.Empty<string>();
        return versions.Select(v =>
            new RegistryModuleVersion(v, GetDescription(metadata, v), GetDocumentationUri(metadata, v)))
            .ToArray();
    }

    private void StartCacheUpdateInBackgroundIfNeeded(bool initialDelay = false)
    {
        _ = UpdateCacheIfNeeded(forceUpdate: false, initialDelay);
    }

    private Task UpdateCacheIfNeeded(bool forceUpdate, bool initialDelay)
    {
        if (!this.cachedModules.Any())
        {
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicRegistryModuleMetadataProvider)}: First data retrieval...");
        }
        else if (forceUpdate)
        {
            Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataProvider)}: Force updating cache...");
        }
        else if (IsCacheExpired())
        {
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicRegistryModuleMetadataProvider)}: Cache expired, updating...");
        }
        else
        {
            return Task.CompletedTask;
        }

        lock (this.queryingLiveSyncObject)
        {
            if (this.queryLiveDataTask is { })
            {
                return this.queryLiveDataTask;
            }

            return this.queryLiveDataTask = QueryData(initialDelay);
        }

        Task QueryData(bool initialDelay)
        {
            return Task.Run(async () =>
            {
                try
                {
                    int delay = 0;
                    if (initialDelay)
                    {
                        // Allow language server to start up a bit before first hit
                        delay = InitialThrottleDelay.Milliseconds;
                    }
                    if (consecutiveFailures > 0)
                    {
                        // Throttle requests to avoid spamming the endpoint with unsuccessful requests
                        delay = int.Max(delay, GetExponentialDelay(InitialThrottleDelay, this.consecutiveFailures, MaxThrottleDelay).Milliseconds); // make second try fast
                    }

                    if (delay > 0)
                    {
                        Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataProvider)}: Delaying {delay} before retry...");
                        await Task.Delay(delay);
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
                    lock (this.queryingLiveSyncObject)
                    {
                        Trace.Assert(this.queryLiveDataTask is { }, $"{nameof(PublicRegistryModuleMetadataProvider)}: should be querying live data");
                        this.queryLiveDataTask = null;
                    }
                }
            });
        }
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

    private async Task<ImmutableArray<BicepModuleMetadata>?> TryGetModulesLive()
    {
        try
        {
            var client = serviceProvider.GetRequiredService<IPublicRegistryModuleMetadataClient>();
            var modules = await client.GetModuleMetadata();

            return modules;
        }
        catch (Exception ex)
        {
            this.lastDownloadError = ex.Message;
            return null;
        }
    }

    // Modules paths are, e.g. "app/dapr-containerapp"
    public Task<IEnumerable<RegistryModule>> GetModules()
    {
        StartCacheUpdateInBackgroundIfNeeded();
        var modules = this.cachedModules.ToArray();
        return Task.FromResult(
            modules.Select(metadata =>
                new RegistryModule(metadata.ModuleName, GetDescription(metadata), GetDocumentationUri(metadata))));
    }

    public Task<IEnumerable<RegistryModuleVersion>> GetVersions(string modulePath)
    {
        StartCacheUpdateInBackgroundIfNeeded();
        var modules = this.cachedModules.ToArray();
        BicepModuleMetadata? metadata = modules.FirstOrDefault(x => x.ModuleName.Equals(modulePath, StringComparison.Ordinal));
        if (metadata == null)
        {
            return Task.FromResult(Enumerable.Empty<RegistryModuleVersion>());
        }

        var versions = metadata.Tags.OrderDescending().ToArray() ?? Enumerable.Empty<string>();
        return Task.FromResult(
            versions.Select(v =>
                new RegistryModuleVersion(v, GetDescription(metadata, v), GetDocumentationUri(metadata, v))));
    }

    private static string? GetDescription(BicepModuleMetadata moduleMetadata, string? version = null)
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

    private static string? GetDocumentationUri(BicepModuleMetadata moduleMetadata, string? version = null)
    {
        version ??= moduleMetadata.Tags.OrderDescending().FirstOrDefault();
        return version is null ? null : moduleMetadata.Properties.TryGetValue(version)?.DocumentationUri;
    }

    public static TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay)
    {
        var maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
        var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
        var delay = TimeSpan.FromSeconds(secondsDelay);

        return delay > maxDelay ? maxDelay : delay;
    }
}
