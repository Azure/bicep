// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bicep.RpcClient.Models;

namespace Bicep.RpcClient;

/// <summary>
/// A factory that manages a pool of Bicep clients, allowing for reuse of clients and automatic disposal of idle clients after a specified inactivity interval.
/// </summary>
public class PooledBicepClientFactory : IBicepClientFactory, IDisposable
{
    private readonly object lockObj = new();
    private static readonly TimeSpan DefaultTimerPollInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan DefaultClientInactivityInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan timerPollInterval;
    private readonly TimeSpan clientInactivityInterval;
    private readonly IBicepClientFactory innerFactory;
    private readonly ConcurrentDictionary<BicepClientConfiguration, ClientPoolEntry> clientPool = new();
    private readonly CancellationTokenSource disposedCts = new();

    public PooledBicepClientFactory(HttpClient? httpClient = null, TimeSpan? inactivityInterval = null)
        : this(new BicepClientFactory(httpClient), ValidateInactivityInterval(inactivityInterval), DefaultTimerPollInterval)
    {
    }

    internal PooledBicepClientFactory(IBicepClientFactory innerFactory, TimeSpan inactivityInterval, TimeSpan timerPollInterval)
    {
        this.innerFactory = innerFactory;
        this.clientInactivityInterval = inactivityInterval;
        this.timerPollInterval = timerPollInterval;
        _ = Task.Run(TimerLoop);
    }

    private static TimeSpan ValidateInactivityInterval(TimeSpan? inactivityInterval)
    {
        if (inactivityInterval is { } interval && interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(inactivityInterval), $"The {nameof(inactivityInterval)} must be greater than zero.");
        }

        return inactivityInterval ?? DefaultClientInactivityInterval;
    }

    /// <inheritdoc/>
    public Task<IBicepClient> Initialize(BicepClientConfiguration configuration, CancellationToken cancellationToken)
    {
        BicepClientConfiguration.Validate(configuration);

        ClientPoolEntry poolEntry;
        lock (lockObj)
        {
            if (disposedCts.IsCancellationRequested)
            {
                throw new ObjectDisposedException(nameof(PooledBicepClientFactory));
            }

            poolEntry = clientPool.GetOrAdd(configuration, config => new ClientPoolEntry(innerFactory, config));
        }

        return Task.FromResult<IBicepClient>(poolEntry.GetPooledClient());
    }

    [Obsolete($"Use {nameof(Initialize)} with a {nameof(BicepClientConfiguration)} that has {nameof(BicepClientConfiguration.ExistingCliPath)} set instead.")]
    public Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken)
        => Initialize(new() { ExistingCliPath = bicepCliPath }, cancellationToken);

    [Obsolete($"Use {nameof(Initialize)} instead.")]
    public Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, CancellationToken cancellationToken)
        => Initialize(configuration, cancellationToken);

    private async Task TimerLoop()
    {
        while (!disposedCts.Token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(timerPollInterval, disposedCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (disposedCts.Token.IsCancellationRequested)
            {
                break;
            }

            foreach (var poolEntry in clientPool.Values)
            {
                poolEntry.CloseUnderlyingClientIfIdle(clientInactivityInterval);
            }
        }
    }

    public void Dispose()
    {
        ClientPoolEntry[] poolEntries;
        lock (lockObj)
        {
            disposedCts.Cancel();
            poolEntries = [.. clientPool.Values];
            clientPool.Clear();
        }

        foreach (var poolEntry in poolEntries)
        {
            poolEntry.Dispose();
        }
    }

    private class ClientPoolEntry(IBicepClientFactory innerFactory, BicepClientConfiguration configuration) : IDisposable
    {
        private readonly SemaphoreSlim acquireClientSemaphore = new(1, 1);
        private readonly object lockObj = new();
        private IBicepClient? activeClient;
        private DateTime lastUsed = DateTime.MinValue;
        private readonly CancellationTokenSource disposedCts = new();
        private int activeRequests;

        public bool IsDisposed => disposedCts.IsCancellationRequested;

        public void Dispose()
        {
            disposedCts.Cancel();
            CloseUnderlyingClient(() => true);
        }

        public PooledBicepClient GetPooledClient() => new(this);

        public void CloseUnderlyingClient(Func<bool> shouldClose)
        {
            IBicepClient? clientToDispose = null;
            lock (lockObj)
            {
                if (!shouldClose())
                {
                    return;
                }

                lastUsed = DateTime.MinValue;
                clientToDispose = activeClient;
                activeClient = null;
            }

            clientToDispose?.Dispose();
        }

        public void CloseUnderlyingClientIfIdle(TimeSpan inactivityInterval)
            => CloseUnderlyingClient(() =>
                activeClient is { } &&
                activeRequests == 0 &&
                lastUsed + inactivityInterval < DateTime.UtcNow);

        private IBicepClient? TryGetActiveClient()
        {
            lock (lockObj)
            {
                return activeClient;
            }
        }

        private IBicepClient UseCreatedOrExistingClient(IBicepClient createdClient)
        {
            lock (lockObj)
            {
                if (disposedCts.IsCancellationRequested)
                {
                    createdClient.Dispose();
                    disposedCts.Token.ThrowIfCancellationRequested();
                }

                if (activeClient is null)
                {
                    activeClient = createdClient;
                    return activeClient;
                }

                return activeClient;
            }
        }

        private async Task<IBicepClient> GetOrCreateUnderlyingClient(CancellationToken cancellationToken)
        {
            if (TryGetActiveClient() is { } existingClient)
            {
                return existingClient;
            }

            using var acquireCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disposedCts.Token);
            await acquireClientSemaphore.WaitAsync(acquireCts.Token).ConfigureAwait(false);
            try
            {
                if (TryGetActiveClient() is { } existingClientAfterWait)
                {
                    return existingClientAfterWait;
                }

                acquireCts.Token.ThrowIfCancellationRequested();

                var createdClient = await innerFactory.Initialize(configuration, acquireCts.Token).ConfigureAwait(false);
                var clientToUse = UseCreatedOrExistingClient(createdClient);

                if (!ReferenceEquals(clientToUse, createdClient))
                {
                    createdClient.Dispose();
                }

                return clientToUse;
            }
            finally
            {
                acquireClientSemaphore.Release();
            }
        }

        public async Task<T> MakeRequest<T>(Func<IBicepClient, CancellationToken, Task<T>> func, CancellationToken cancellationToken)
        {
            using var requestCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disposedCts.Token);

            try
            {
                lock (lockObj)
                {
                    activeRequests++;
                    lastUsed = DateTime.UtcNow;
                }

                var client = await GetOrCreateUnderlyingClient(requestCts.Token).ConfigureAwait(false);

                return await func(client, requestCts.Token).ConfigureAwait(false);
            }
            finally
            {
                lock (lockObj)
                {
                    activeRequests--;
                    lastUsed = DateTime.UtcNow;
                }
            }
        }
    }

    private class PooledBicepClient(ClientPoolEntry poolEntry) : IBicepClient
    {
        private readonly CancellationTokenSource disposedCts = new();

        public Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.Compile(request, ct), cancellationToken);

        public Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.CompileParams(request, ct), cancellationToken);

        public Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.Format(request, ct), cancellationToken);

        public Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.GetDeploymentGraph(request, ct), cancellationToken);

        public Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.GetFileReferences(request, ct), cancellationToken);

        public Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.GetMetadata(request, ct), cancellationToken);

        public Task<GetSnapshotResponse> GetSnapshot(GetSnapshotRequest request, CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.GetSnapshot(request, ct), cancellationToken);

        public Task<string> GetVersion(CancellationToken cancellationToken = default)
            => MakeRequest((client, ct) => client.GetVersion(ct), cancellationToken);

        public async Task<T> MakeRequest<T>(Func<IBicepClient, CancellationToken, Task<T>> func, CancellationToken cancellationToken)
        {
            // simulate disposal without actually releasing underlying resource, since the pool manages this lifecycle
            try
            {
                disposedCts.Token.ThrowIfCancellationRequested();

                using var requestCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disposedCts.Token);

                return await poolEntry.MakeRequest(func, requestCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (disposedCts.IsCancellationRequested || poolEntry.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(PooledBicepClient));
            }
        }

        public void Dispose()
        {
            disposedCts.Cancel();
        }
    }
}
