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
using Microsoft.Win32;
using System.Threading.Tasks;
using Bicep.Core.Registry.Oci;
using System.Security.Cryptography.Xml;
using Bicep.Core.Registry.Catalog;

namespace Bicep.Core.Registry.Catalog; //asdfg split into PublicRegistry/PrivateRegistry


/// <summary>
/// Provider to get metadata for modules stored in a public or private registry.
/// </summary>
public abstract class BaseModuleMetadataProvider(
    string registry
) : IRegistryModuleMetadataProvider
{
    private readonly TimeSpan CacheValidFor = TimeSpan.FromHours(1);
    private readonly TimeSpan InitialThrottleDelay = TimeSpan.FromSeconds(5);
    private readonly TimeSpan MaxThrottleDelay = TimeSpan.FromMinutes(2);

    private readonly object queryingLiveSyncObject = new();
    private Task? queryLiveDataTask;
    private DateTime? lastSuccessfulQuery;
    private int consecutiveFailures = 0;

    public string Registry => registry;

    // Using array instead of dictionary to preserve sort order
    private ImmutableArray<IRegistryModuleMetadata> cachedModules = [];
    private string? lastDownloadError = null;

    public bool IsCached => cachedModules.Length > 0;

    public string? DownloadError => IsCached ? null : lastDownloadError;

    protected abstract Task<ImmutableArray<IRegistryModuleMetadata>> GetLiveDataCoreAsync();

    protected abstract Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath);

    public Task TryAwaitCache(bool forceUpdate)
    {
        return UpdateCacheIfNeededAsync(forceUpdate: forceUpdate, initialDelay: false);
    }

    public void StartCache()
    {
        _ = TryAwaitCache(false);
    }

    public async Task<bool> TryUpdateCacheAsync()//asdfg implement/test threading
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

    protected IRegistryModuleMetadata? GetCachedModule(string modulePath, bool throwIfNotFound)
    {
        var found = this.cachedModules.FirstOrDefault(x =>
            x.Registry.Equals(Registry, StringComparison.Ordinal)
                && x.ModuleName.Equals(modulePath, StringComparison.Ordinal));
        if (found != null && throwIfNotFound)
        {
            throw new InvalidOperationException($"Module '{modulePath}' not found in cache.");
        }

        return found;
    }

    public async Task<ImmutableArray<IRegistryModuleMetadata>> TryGetModulesAsync()
    {
        await TryAwaitCache(forceUpdate: false);
        return GetCachedModules();
    }

    // If cache has not yet successfully been updated, returns empty
    public ImmutableArray<IRegistryModuleMetadata> GetCachedModules()
    {
        return [.. cachedModules
            .Where(x => x.Registry.Equals(Registry, StringComparison.Ordinal))
        ];
    }

    private Task UpdateCacheIfNeededAsync(bool forceUpdate, bool initialDelay)
    {
        if (this.DownloadError is not null)
        {
            Trace.WriteLine($"{nameof(PublicModuleMetadataProvider)}: [{Registry}] Last cache load failed, trying again...");
        }
        else if (this.lastSuccessfulQuery is null)
        {
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicModuleMetadataProvider)}: [{Registry}] First data retrieval...");
        }
        else if (forceUpdate)
        {
            Trace.WriteLine($"{nameof(PublicModuleMetadataProvider)}: [{Registry}] Force updating cache...");
        }
        else if (IsCacheExpired())
        {
            Trace.WriteLineIf(IsCacheExpired(), $"{nameof(PublicModuleMetadataProvider)}: [{Registry}] Cache expired, updating...");
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
                        delay = int.Max(delay, GetExponentialDelay(InitialThrottleDelay, this.consecutiveFailures, MaxThrottleDelay).Milliseconds/*asdfg should be TotalMilliseconds, but don't want to delay here*/); // make second try fast
                    }

                    if (delay > 0)
                    {
                        Trace.WriteLine($"{nameof(PublicModuleMetadataProvider)}: [{Registry}] Delaying {delay} before retry...");
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
                        Trace.Assert(this.queryLiveDataTask is { }, $"{nameof(PublicModuleMetadataProvider)}: [{Registry}] Should be querying live data");
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
            Trace.TraceInformation($"{nameof(PublicModuleMetadataProvider)}: [{Registry}] Cache has expired.");
        }

        return expired;
    }

    private async Task<ImmutableArray<IRegistryModuleMetadata>?> TryGetLiveDataAsync()
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

    public static TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay)
    {
        var maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
        var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
        var delay = TimeSpan.FromSeconds(secondsDelay);

        return delay > maxDelay ? maxDelay : delay;
    }
}
