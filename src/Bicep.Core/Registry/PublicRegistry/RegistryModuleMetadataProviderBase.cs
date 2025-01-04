// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// </summary>
public abstract class RegistryModuleMetadataProviderBase : IRegistryModuleMetadataProvider //asdfg rename     asdfg refactor cache portion?
{
    private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);
    private readonly TimeSpan InitialThrottleDelay = TimeSpan.FromSeconds(5);
    private readonly TimeSpan MaxThrottleDelay = TimeSpan.FromMinutes(2);

    private readonly object queryingLiveSyncObject = new();
    private Task? queryLiveDataTask;
    private DateTime? lastSuccessfulQuery;
    private int consecutiveFailures = 0;

    protected record CachedModule(
        RegistryModuleMetadata RegistryModuleMetadata,
        // Using array instead of dictionary to preserve sort order of versions
        ImmutableArray<RegistryModuleVersionMetadata>RegistryModuleVersionMetadata);

    private ImmutableArray<CachedModule> cachedModules = []; //asdfg dicvtionary?
    private string? lastDownloadError = null;

    public bool IsCached => cachedModules.Length > 0;

    public string? DownloadError => IsCached ? null : lastDownloadError;

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
        if (await TryGetLiveDataAsync() is { } modules)
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

    private CachedModule? TryGetCachedModule(string registry, string modulePath)
    {
        return this.cachedModules.FirstOrDefault(x =>
            x.RegistryModuleMetadata.Registry.Equals(registry, StringComparison.Ordinal)
                && x.RegistryModuleMetadata.ModuleName.Equals(modulePath, StringComparison.Ordinal));
    }

    // If cache has not yet successfully been updated, returns empty
    public ImmutableArray<RegistryModuleMetadata> GetModules()
    {
        StartCacheUpdateInBackgroundIfNeeded();

        return [.. cachedModules.Select(x => x.RegistryModuleMetadata)];
    }

    public ImmutableArray<RegistryModuleVersionMetadata> GetModuleVersions(string registry, string modulePath)
    {
        StartCacheUpdateInBackgroundIfNeeded();

        return TryGetCachedModule(registry, modulePath)?.RegistryModuleVersionMetadata.ToImmutableArray() ?? [];
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
            Trace.TraceInformation($"{nameof(PublicRegistryModuleMetadataProvider)}: Public asdfg modules cache is expired.");
        }

        return expired;
    }

    protected abstract Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync();

    private async Task<ImmutableArray<CachedModule>?> TryGetLiveDataAsync()
    {
        try
        {
            return await GetLiveDataCoreAsync();
        }
        catch (Exception ex)
        {
            this.lastDownloadError = ex.Message;
            return null;
        }
    }

    public static TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay) //asdfg move
    {
        var maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
        var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
        var delay = TimeSpan.FromSeconds(secondsDelay);

        return delay > maxDelay ? maxDelay : delay;
    }
}
