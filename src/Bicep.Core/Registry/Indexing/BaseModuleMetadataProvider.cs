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

namespace Bicep.Core.Registry.Indexing;

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

    private static readonly object CachedModuleSyncObject = new();

    //asdfg test all this
    protected class CachableModule
    {
        public RegistryModuleMetadata RegistryModuleMetadata;

        // Using array instead of dictionary to preserve sort order
        public Task<ImmutableArray<CachableVersionMetadata>>? Versions { get; private set; }

        public Task<ImmutableArray<CachableVersionMetadata>>? GetVersionsTask { get; private set; }

        public CachableModule(RegistryModuleMetadata registryModuleMetadata, ImmutableArray<RegistryModuleVersionMetadata>? versionsMetadata)
        {
            this.RegistryModuleMetadata = registryModuleMetadata;
            this.GetVersionsTask = versionsMetadata is null ? null :
                Task.FromResult(versionsMetadata.Value.Select(v =>
                    new CachableVersionMetadata(v.Version, v)).ToImmutableArray());
        }

        //asdfg extract - use lazy?
        public Task<ImmutableArray<CachableVersionMetadata>> GetOrSetVersionsTask(Func<Task<ImmutableArray<CachableVersionMetadata>>> createTask)
        {
            if (this.GetVersionsTask is null)
            {
                lock (CachedModuleSyncObject)
                {
                    if (this.GetVersionsTask is null)
                    {
                        this.GetVersionsTask = createTask();
                    }
                }
            }

            return this.GetVersionsTask;
        }
    }

    protected class CachableVersionMetadata
    {
        public string Version { get; init; }
        public RegistryModuleVersionMetadata? Metadata { get; private set; }
        public Task<RegistryModuleVersionMetadata?>? GetMetadataTask { get; private set; }

        public CachableVersionMetadata(string version, RegistryModuleVersionMetadata? metadata)
        {
            this.Version = version;
            this.Metadata = metadata;
            this.GetMetadataTask = metadata is null ? null : Task.FromResult(metadata);
        }

        public Task<RegistryModuleVersionMetadata?> GetOrSetMetadataTask(Func<Task<RegistryModuleVersionMetadata?>> createTask)
        {
            if (this.GetMetadataTask is null)
            {
                lock (CachedModuleSyncObject)
                {
                    if (this.GetMetadataTask is null)
                    {
                        this.GetMetadataTask = createTask();
                    }
                }
            }

            return this.GetMetadataTask;
        }
    }

    // Using array instead of dictionary to preserve sort order
    private ImmutableArray<CachableModule> cachedModules = [];
    private string? lastDownloadError = null;

    public bool IsCached => cachedModules.Length > 0;

    public string? DownloadError => IsCached ? null : lastDownloadError;

    protected abstract Task<ImmutableArray<CachableModule>> GetLiveDataCoreAsync();

    protected abstract Task<ImmutableArray<string>> GetLiveModuleVersionsAsync(string modulePath);

    protected abstract Task<RegistryModuleVersionMetadata?> GetLiveModuleVersionMetadataAsync(string modulePath, string version);

    public Task TryAwaitCache(bool forceUpdate)
    {
        return UpdateCacheIfNeededAsync(forceUpdate: forceUpdate, initialDelay: false);
    }

    public void StartUpdateCache(bool forceUpdate)
    {
        _ = TryAwaitCache(forceUpdate);
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

    protected CachableModule? GetCachedModule(string modulePath, bool throwIfNotFound)
    {
        var found = this.cachedModules.FirstOrDefault(x =>
            x.RegistryModuleMetadata.Registry.Equals(Registry, StringComparison.Ordinal)
                && x.RegistryModuleMetadata.ModuleName.Equals(modulePath, StringComparison.Ordinal));
        if (found != null && throwIfNotFound)
        {
            throw new InvalidOperationException($"Module '{modulePath}' not found in cache.");
        }

        return found;
    }

    public async Task<ImmutableArray<RegistryModuleMetadata>> TryGetModulesAsync()
    {
        await TryAwaitCache(forceUpdate: false);
        return GetCachedModules();
    }

    // If cache has not yet successfully been updated, returns empty
    public ImmutableArray<RegistryModuleMetadata> GetCachedModules()
    {
        return [.. cachedModules.Where(x => x.RegistryModuleMetadata.Registry.Equals(Registry, StringComparison.Ordinal)).Select(x => x.RegistryModuleMetadata)];
    }

    private async Task<ImmutableArray<CachableVersionMetadata>> TryGetCachableModuleVersionsAsync(string modulePath)
    {
        await TryAwaitCache(forceUpdate: false);
        var module = GetCachedModule(modulePath, false);
        if (module is null)
        {
            return [];
        }

        return await module.GetOrSetVersionsTask(async () =>
        {
            var versions = await GetLiveModuleVersionsAsync(modulePath);
            return [.. versions.Select(v => new CachableVersionMetadata(v, null))];
        });
    }

    public async Task<ImmutableArray<string>> TryGetModuleVersionsAsync(string modulePath) //asdfg should start getting metadata
    {
        var versions = await TryGetCachableModuleVersionsAsync(modulePath);
        return [.. versions.Select(v => v.Version)];
    }

    public async Task<RegistryModuleVersionMetadata?> TryGetModuleVersionMetadataAsync(string modulePath, string version)
    {
        var versions = await TryGetCachableModuleVersionsAsync(modulePath);
        if (versions.FirstOrDefault(v => v.Version.Equals(version, StringComparison.Ordinal)) is not { } versionMetadata)
        {
            return new RegistryModuleVersionMetadata(version, null, null);
        }

        return await versionMetadata.GetOrSetMetadataTask(async () => await GetLiveModuleVersionMetadataAsync(modulePath, version));
    }

    // If cache has not yet successfully been updated, returns empty
    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedModuleVersions(string modulePath)
    {
        var getVersionsTask = GetCachedModule(modulePath, false)?.GetVersionsTask;
        if (getVersionsTask?.IsCompletedSuccessfully == true)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            return [.. getVersionsTask.Result
                .Select(v => v.Metadata ?? new RegistryModuleVersionMetadata(v.Version, null, null/*asdfg needed? test */))];
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        return [];
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
                        delay = int.Max(delay, GetExponentialDelay(InitialThrottleDelay, this.consecutiveFailures, MaxThrottleDelay).Milliseconds); // make second try fast
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

    private async Task<ImmutableArray<CachableModule>?> TryGetLiveDataAsync()
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
