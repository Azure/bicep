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

public static class PublicModuleMetadataProviderExtensions
{
    public static IServiceCollection AddPublicModuleMetadataProviderServices(this IServiceCollection services)
    {
        services.AddSingleton<IPublicModuleMetadataProvider, PublicModuleMetadataProvider>();

        // using type based registration for Http clients so dependencies can be injected automatically
        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        services
            .AddHttpClient<IPublicModuleIndexClient, PublicModuleMetadataClient>()
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
public class PublicModuleMetadataProvider : IPublicModuleMetadataProvider
{
    private readonly IServiceProvider serviceProvider;

    private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);
    private readonly TimeSpan InitialThrottleDelay = TimeSpan.FromSeconds(5);
    private readonly TimeSpan MaxThrottleDelay = TimeSpan.FromMinutes(2);

    private readonly object queryingLiveSyncObject = new();
    private Task? queryLiveDataTask;
    private DateTime? lastSuccessfulQuery;
    private int consecutiveFailures = 0;

    private ImmutableArray<PublicModuleIndexEntry> cachedIndex = [];
    private string? lastDownloadError = null;

    public bool IsCached => cachedIndex.Length > 0;

    public string? DownloadError => IsCached ? null : lastDownloadError;

    public PublicModuleMetadataProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task TryAwaitCache(bool forceUpdate)
    {
        return UpdateCacheIfNeeded(forceUpdate: forceUpdate, initialDelay: false);
    }

    public void StartUpdateCache(bool forceUpdate)
    {
        _ = TryAwaitCache(forceUpdate);
    }
    public async Task<bool> TryUpdateCacheAsync()
    {
        if (await TryGetLiveIndexAsync() is { } modules)
        {
            this.cachedIndex = modules;
            this.lastSuccessfulQuery = DateTime.Now;
            return true;
        }
        else
        {
            return false;
        }
    }

    // If cache has not yet successfully been updated, returns empty
    public ImmutableArray<PublicModuleMetadata> GetModulesMetadata()
    {
        StartCacheUpdateInBackgroundIfNeeded();

        return [.. this.cachedIndex.Select(x => new PublicModuleMetadata(x.ModulePath, x.GetDescription(), x.GetDocumentationUri()))];
    }

    public ImmutableArray<PublicModuleVersionMetadata> GetModuleVersionsMetadata(string modulePath)
    {
        StartCacheUpdateInBackgroundIfNeeded();

        PublicModuleIndexEntry? entry = this.cachedIndex.FirstOrDefault(x => x.ModulePath.Equals(modulePath, StringComparison.Ordinal));

        if (entry == null)
        {
            return [];
        }

        return [.. entry.Versions.Select(version => new PublicModuleVersionMetadata(version, entry.GetDescription(version), entry.GetDocumentationUri(version)))];
    }

    private void StartCacheUpdateInBackgroundIfNeeded(bool initialDelay = false)
    {
        _ = UpdateCacheIfNeeded(forceUpdate: false, initialDelay);
    }

    private Task UpdateCacheIfNeeded(bool forceUpdate, bool initialDelay)
    {
        if (!this.cachedIndex.Any())
        {
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicModuleMetadataProvider)}: First data retrieval...");
        }
        else if (forceUpdate)
        {
            Trace.WriteLine($"{nameof(PublicModuleMetadataProvider)}: Force updating cache...");
        }
        else if (IsCacheExpired())
        {
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicModuleMetadataProvider)}: Cache expired, updating...");
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
                        Trace.WriteLine($"{nameof(PublicModuleMetadataProvider)}: Delaying {delay} before retry...");
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
                        Trace.Assert(this.queryLiveDataTask is { }, $"{nameof(PublicModuleMetadataProvider)}: should be querying live data");
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
            Trace.TraceInformation($"{nameof(PublicModuleMetadataProvider)}: Public modules cache is expired.");
        }

        return expired;
    }

    private async Task<ImmutableArray<PublicModuleIndexEntry>?> TryGetLiveIndexAsync()
    {
        try
        {
            var client = serviceProvider.GetRequiredService<IPublicModuleIndexClient>();
            var modules = await client.GetModuleIndexAsync();

            return modules;
        }
        catch (Exception ex)
        {
            this.lastDownloadError = ex.Message;
            return null;
        }
    }

    public static TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay)
    {
        var maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
        var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
        var delay = TimeSpan.FromSeconds(secondsDelay);

        return delay > maxDelay ? maxDelay : delay;
    }
}
