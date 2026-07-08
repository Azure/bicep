// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.RpcClient.JsonRpc;
using Bicep.RpcClient.Models;
using FluentAssertions;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class BicepClientUnitTests
{
    public TestContext TestContext { get; set; } = null!;

    private CancellationToken Token => TestContext.CancellationTokenSource.Token;

    [TestMethod]
    public async Task GetVersion_caches_result_and_does_not_re_issue_request()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("1.2.3"));
        using var client = new BicepClient(rpc);

        (await client.GetVersion(Token)).Should().Be("1.2.3");
        (await client.GetVersion(Token)).Should().Be("1.2.3");

        rpc.CallCount("bicep/version").Should().Be(1);
    }

    [TestMethod]
    public async Task Format_throws_when_cli_version_is_below_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.37.0"));
        using var client = new BicepClient(rpc);

        await FluentActions.Invoking(() => client.Format(new("main.bicep"), Token))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*requires Bicep CLI version '0.37.1' or later*0.37.0*");

        rpc.CallCount("bicep/format").Should().Be(0);
    }

    [TestMethod]
    public async Task Format_succeeds_when_cli_version_meets_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.37.1"));
        rpc.SetResponse("bicep/format", new FormatResponse("formatted"));
        using var client = new BicepClient(rpc);

        var result = await client.Format(new("main.bicep"), Token);

        result.Contents.Should().Be("formatted");
        rpc.CallCount("bicep/format").Should().Be(1);
    }

    [TestMethod]
    public async Task GetSnapshot_throws_when_cli_version_is_below_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.36.0"));
        using var client = new BicepClient(rpc);

        await FluentActions.Invoking(() => client.GetSnapshot(
                new("main.bicepparam", new(null, null, null, null, null), null), Token))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*requires Bicep CLI version '0.36.1' or later*0.36.0*");

        rpc.CallCount("bicep/getSnapshot").Should().Be(0);
    }

    [TestMethod]
    public async Task GetSnapshot_succeeds_when_cli_version_meets_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.36.1"));
        rpc.SetResponse("bicep/getSnapshot", new GetSnapshotResponse("snapshot-contents"));
        using var client = new BicepClient(rpc);

        var result = await client.GetSnapshot(
            new("main.bicepparam", new(null, null, null, null, null), null), Token);

        result.Snapshot.Should().Be("snapshot-contents");
        rpc.CallCount("bicep/getSnapshot").Should().Be(1);
    }

    [TestMethod]
    public async Task Compile_forwards_request_to_the_expected_method()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/compile", new CompileResponse(true, [], "{}"));
        using var client = new BicepClient(rpc);

        var result = await client.Compile(new("main.bicep"), Token);

        result.Success.Should().BeTrue();
        rpc.CallCount("bicep/compile").Should().Be(1);
    }

    [TestMethod]
    public async Task GetDeploymentGraph_forwards_request_to_the_expected_method()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/getDeploymentGraph", new GetDeploymentGraphResponse([], []));
        using var client = new BicepClient(rpc);

        var result = await client.GetDeploymentGraph(new("main.bicep"), Token);

        result.Nodes.Should().BeEmpty();
        rpc.CallCount("bicep/getDeploymentGraph").Should().Be(1);
    }

    [TestMethod]
    public async Task GetFileReferences_forwards_request_to_the_expected_method()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/getFileReferences", new GetFileReferencesResponse(["main.bicep"]));
        using var client = new BicepClient(rpc);

        var result = await client.GetFileReferences(new("main.bicep"), Token);

        result.FilePaths.Should().Contain("main.bicep");
        rpc.CallCount("bicep/getFileReferences").Should().Be(1);
    }

    [TestMethod]
    public void Dispose_disposes_the_underlying_rpc_client()
    {
        var rpc = new FakeJsonRpcClient();
        var client = new BicepClient(rpc);

        client.Dispose();

        rpc.IsDisposed.Should().BeTrue();
    }

    private sealed class FakeJsonRpcClient : IJsonRpcClient
    {
        private readonly ConcurrentDictionary<string, object> responsesByMethod = new();
        private readonly ConcurrentDictionary<string, int> callCountsByMethod = new();

        public bool IsDisposed { get; private set; }

        public void SetResponse<TResponse>(string method, TResponse response)
            => responsesByMethod[method] = response!;

        public int CallCount(string method) => callCountsByMethod.TryGetValue(method, out var count) ? count : 0;

        public Task<TResponse> SendRequest<TRequest, TResponse>(string method, TRequest request, CancellationToken cancellationToken)
        {
            callCountsByMethod.AddOrUpdate(method, 1, (_, count) => count + 1);

            if (!responsesByMethod.TryGetValue(method, out var response))
            {
                throw new InvalidOperationException($"No response configured for method '{method}'.");
            }

            return Task.FromResult((TResponse)response);
        }

        public Task Listen(Action onComplete, CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose() => IsDisposed = true;
    }
}
