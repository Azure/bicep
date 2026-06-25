// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Bicep.Core.UnitTests.Utils;
using Bicep.RpcClient.Models;
using FluentAssertions;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class PooledBicepClientFactoryTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(TestContext.TestResultsDirectory))
        {
            Directory.Delete(TestContext.TestResultsDirectory, true);
        }
    }

    [TestMethod]
    public async Task Disposing_one_wrapper_does_not_dispose_shared_client()
    {
        using var factory = new PooledBicepClientFactory();
        var config = new BicepClientConfiguration { ExistingCliPath = GetCliPath() };

        var wrapperA = await factory.Initialize(config, TestContext.CancellationTokenSource.Token);
        var wrapperB = await factory.Initialize(config, TestContext.CancellationTokenSource.Token);

        wrapperA.Dispose();

        var version = await wrapperB.GetVersion(TestContext.CancellationTokenSource.Token);

        version.Should().MatchRegex(@"^\d+\.\d+\.\d+(-.+)?$");

        wrapperB.Dispose();
    }

    [TestMethod]
    public async Task Initialize_throws_after_factory_disposed()
    {
        var factory = new PooledBicepClientFactory();
        factory.Dispose();

        await FluentActions.Invoking(() =>
                factory.Initialize(new BicepClientConfiguration { ExistingCliPath = GetCliPath() }, TestContext.CancellationTokenSource.Token))
            .Should().ThrowAsync<ObjectDisposedException>();
    }

    [TestMethod]
    public async Task Concurrent_requests_succeed_with_multiple_wrappers()
    {
        using var factory = new PooledBicepClientFactory();
        var config = new BicepClientConfiguration { ExistingCliPath = GetCliPath() };

        var wrappers = await Task.WhenAll(Enumerable.Range(0, 4)
            .Select(_ => factory.Initialize(config, TestContext.CancellationTokenSource.Token)));

        try
        {
            var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", "param location string");

            var results = await Task.WhenAll(wrappers.Select(wrapper =>
                wrapper.Compile(new CompileRequest(bicepFile), TestContext.CancellationTokenSource.Token)));

            results.Should().OnlyContain(result => result.Success);
        }
        finally
        {
            foreach (var wrapper in wrappers)
            {
                wrapper.Dispose();
            }
        }
    }

    [TestMethod]
    public void Constructor_throws_for_non_positive_inactivity_interval()
    {
        FluentActions.Invoking(() => new PooledBicepClientFactory(inactivityInterval: TimeSpan.Zero))
            .Should().Throw<ArgumentOutOfRangeException>();

        FluentActions.Invoking(() => new PooledBicepClientFactory(inactivityInterval: TimeSpan.FromSeconds(-1)))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void Constructor_accepts_positive_inactivity_interval()
    {
        FluentActions.Invoking(() =>
        {
            using var factory = new PooledBicepClientFactory(inactivityInterval: TimeSpan.FromSeconds(1));
        }).Should().NotThrow();
    }

    [TestMethod]
    public async Task Equal_configurations_reuse_a_single_underlying_client()
    {
        var inner = new FakeBicepClientFactory();
        using var factory = CreatePooledFactory(inner);

        // Two distinct-but-equal configuration instances must resolve to the same pool entry (record equality).
        var wrapperA = await factory.Initialize(new BicepClientConfiguration(), Token);
        var wrapperB = await factory.Initialize(new BicepClientConfiguration(), Token);

        await wrapperA.GetVersion(Token);
        await wrapperB.GetVersion(Token);

        inner.InitializeCallCount.Should().Be(1);
        inner.CreatedClients.Should().HaveCount(1);

        wrapperA.Dispose();
        wrapperB.Dispose();
    }

    [TestMethod]
    public async Task Different_configurations_create_separate_clients()
    {
        var inner = new FakeBicepClientFactory();
        using var factory = CreatePooledFactory(inner);

        var wrapperA = await factory.Initialize(new BicepClientConfiguration(), Token);
        var wrapperB = await factory.Initialize(new BicepClientConfiguration { BicepVersion = "1.2.3" }, Token);

        await wrapperA.GetVersion(Token);
        await wrapperB.GetVersion(Token);

        inner.InitializeCallCount.Should().Be(2);
        inner.CreatedClients.Should().HaveCount(2);

        wrapperA.Dispose();
        wrapperB.Dispose();
    }

    [TestMethod]
    public async Task Idle_client_is_closed_and_recreated_on_next_request()
    {
        var inner = new FakeBicepClientFactory();
        using var factory = CreatePooledFactory(inner, inactivityInterval: TimeSpan.FromMilliseconds(50), pollInterval: TimeSpan.FromMilliseconds(20));
        var wrapper = await factory.Initialize(new BicepClientConfiguration(), Token);

        await wrapper.GetVersion(Token);
        var firstClient = inner.CreatedClients.Single();

        await WaitUntilAsync(() => firstClient.IsDisposed, TimeSpan.FromSeconds(5));
        firstClient.IsDisposed.Should().BeTrue();

        // A subsequent request should transparently spin up a fresh underlying client.
        await wrapper.GetVersion(Token);
        inner.InitializeCallCount.Should().Be(2);
        inner.CreatedClients.Should().HaveCount(2);

        wrapper.Dispose();
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Test gating; not an issue in test code.")]
    public async Task Active_request_prevents_idle_eviction()
    {
        var requestStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseRequest = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var inner = new FakeBicepClientFactory(clientFactory: () => new FakeBicepClient(async _ =>
        {
            requestStarted.TrySetResult(true);
            await releaseRequest.Task;
        }));
        using var factory = CreatePooledFactory(inner, inactivityInterval: TimeSpan.FromMilliseconds(50), pollInterval: TimeSpan.FromMilliseconds(20));
        var wrapper = await factory.Initialize(new BicepClientConfiguration(), Token);

        var requestTask = wrapper.GetVersion(Token);
        await requestStarted.Task;

        // Wait well past the inactivity interval; the in-flight request must keep the client alive.
        await Task.Delay(TimeSpan.FromMilliseconds(250));
        inner.CreatedClients.Single().IsDisposed.Should().BeFalse();

        releaseRequest.SetResult(true);
        (await requestTask).Should().Be(FakeBicepClient.Version);

        wrapper.Dispose();
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Test gating; not an issue in test code.")]
    public async Task Concurrent_first_requests_create_a_single_client()
    {
        var releaseInitialize = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var inner = new FakeBicepClientFactory(onInitialize: async _ => await releaseInitialize.Task);
        using var factory = CreatePooledFactory(inner);
        var config = new BicepClientConfiguration();

        var wrappers = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => factory.Initialize(config, Token)));
        var requests = wrappers.Select(wrapper => wrapper.GetVersion(Token)).ToArray();

        // Give the concurrent requests time to converge on the single-client acquisition path.
        await Task.Delay(TimeSpan.FromMilliseconds(50));
        releaseInitialize.SetResult(true);
        await Task.WhenAll(requests);

        inner.InitializeCallCount.Should().Be(1);
        inner.CreatedClients.Should().HaveCount(1);

        foreach (var wrapper in wrappers)
        {
            wrapper.Dispose();
        }
    }

    [TestMethod]
    public async Task Request_on_disposed_wrapper_throws_object_disposed()
    {
        var inner = new FakeBicepClientFactory();
        using var factory = CreatePooledFactory(inner);
        var wrapper = await factory.Initialize(new BicepClientConfiguration(), Token);

        wrapper.Dispose();

        await FluentActions.Invoking(() => wrapper.GetVersion(Token))
            .Should().ThrowAsync<ObjectDisposedException>();
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Test gating; not an issue in test code.")]
    public async Task Disposing_one_wrapper_does_not_affect_in_flight_request_on_another()
    {
        var releaseRequest = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var requestStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var inner = new FakeBicepClientFactory(clientFactory: () => new FakeBicepClient(async _ =>
        {
            requestStarted.TrySetResult(true);
            await releaseRequest.Task;
        }));
        using var factory = CreatePooledFactory(inner);
        var config = new BicepClientConfiguration();

        var wrapperA = await factory.Initialize(config, Token);
        var wrapperB = await factory.Initialize(config, Token);

        var requestTask = wrapperB.GetVersion(Token);
        await requestStarted.Task;

        wrapperA.Dispose();

        releaseRequest.SetResult(true);
        (await requestTask).Should().Be(FakeBicepClient.Version);

        wrapperB.Dispose();
    }

    [TestMethod]
    public async Task Request_after_factory_disposed_throws_object_disposed()
    {
        var inner = new FakeBicepClientFactory();
        var factory = CreatePooledFactory(inner);
        var wrapper = await factory.Initialize(new BicepClientConfiguration(), Token);

        factory.Dispose();

        await FluentActions.Invoking(() => wrapper.GetVersion(Token))
            .Should().ThrowAsync<ObjectDisposedException>();
    }

    [TestMethod]
    public async Task Request_with_canceled_token_throws()
    {
        var inner = new FakeBicepClientFactory();
        using var factory = CreatePooledFactory(inner);
        var wrapper = await factory.Initialize(new BicepClientConfiguration(), Token);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await FluentActions.Invoking(() => wrapper.GetVersion(cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();

        wrapper.Dispose();
    }

    private CancellationToken Token => TestContext.CancellationTokenSource.Token;

    private static PooledBicepClientFactory CreatePooledFactory(FakeBicepClientFactory inner, TimeSpan? inactivityInterval = null, TimeSpan? pollInterval = null)
        => new(inner, inactivityInterval ?? TimeSpan.FromMinutes(5), pollInterval ?? TimeSpan.FromMilliseconds(20));

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            if (stopwatch.Elapsed > timeout)
            {
                throw new TimeoutException("The expected condition was not met within the timeout.");
            }

            await Task.Delay(10);
        }
    }

    private static string GetCliPath()
    {
        var cliName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        return Path.GetFullPath(Path.Combine(typeof(PooledBicepClientFactoryTests).Assembly.Location, $"../{cliName}"));
    }

    private sealed class FakeBicepClientFactory(
        Func<FakeBicepClient>? clientFactory = null,
        Func<CancellationToken, Task>? onInitialize = null) : IBicepClientFactory
    {
        private int initializeCallCount;

        public int InitializeCallCount => Volatile.Read(ref initializeCallCount);

        public ConcurrentQueue<FakeBicepClient> CreatedClients { get; } = new();

        public async Task<IBicepClient> Initialize(BicepClientConfiguration configuration, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref initializeCallCount);

            if (onInitialize is { })
            {
                await onInitialize(cancellationToken);
            }

            var client = (clientFactory ?? (() => new FakeBicepClient()))();
            CreatedClients.Enqueue(client);
            return client;
        }

        public Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    private sealed class FakeBicepClient(Func<CancellationToken, Task>? onRequest = null) : IBicepClient
    {
        public const string Version = "1.2.3";

        private int disposeCount;

        public bool IsDisposed => Volatile.Read(ref disposeCount) > 0;

        public async Task<string> GetVersion(CancellationToken cancellationToken = default)
        {
            if (onRequest is { })
            {
                await onRequest(cancellationToken);
            }

            return Version;
        }

        public Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<GetSnapshotResponse> GetSnapshot(GetSnapshotRequest request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public void Dispose() => Interlocked.Increment(ref disposeCount);
    }
}
