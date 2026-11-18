// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
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
    public void GenerateDocs_models_expose_constructor_values()
    {
        var customValues = new Dictionary<string, string> { ["owner"] = "Platform" };
        var request = new GenerateDocsRequest("main.bicep", customValues);
        var response = new GenerateDocsResponse([], "# Module\n");

        request.Path.Should().Be("main.bicep");
        request.CustomTemplateValues.Should().BeSameAs(customValues);
        response.Diagnostics.Should().BeEmpty();
        response.Contents.Should().Be("# Module\n");
    }

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
                new("main.bicepparam", new(null, null, null, null, null, null), null), Token))
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
            new("main.bicepparam", new(null, null, null, null, null, null), null), Token);

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
    public async Task GenerateDocs_forwards_request_to_the_expected_method()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.47.0"));
        rpc.SetResponse(
            "bicep/generateDocs",
            new GenerateDocsResponse([], "# Module\n"));
        using var client = new BicepClient(rpc);

        var result = await client.GenerateDocs(
            new("main.bicep"),
            Token);

        result.Contents.Should().Be("# Module\n");
        rpc.CallCount("bicep/generateDocs").Should().Be(1);
    }

    [TestMethod]
    public async Task GenerateDocs_throws_when_cli_version_is_below_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.46.1"));
        using var client = new BicepClient(rpc);

        await FluentActions.Invoking(() => client.GenerateDocs(
                new("main.bicep"),
                Token))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*requires Bicep CLI version '0.47.0' or later*");

        rpc.CallCount("bicep/generateDocs").Should().Be(0);
    }

    private static readonly IReadOnlyDictionary<string, (string Method, Func<BicepClient, CancellationToken, Task> Invoke)> RequestsWithConfigPath =
        new Dictionary<string, (string, Func<BicepClient, CancellationToken, Task>)>
        {
            [nameof(BicepClient.Compile)] = ("bicep/compile", (client, token) => client.Compile(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.CompileParams)] = ("bicep/compileParams", (client, token) => client.CompileParams(new("main.bicepparam", new()) { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.Format)] = ("bicep/format", (client, token) => client.Format(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.GenerateDocs)] = ("bicep/generateDocs", (client, token) => client.GenerateDocs(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.GetDeploymentGraph)] = ("bicep/getDeploymentGraph", (client, token) => client.GetDeploymentGraph(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.GetFileReferences)] = ("bicep/getFileReferences", (client, token) => client.GetFileReferences(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.GetMetadata)] = ("bicep/getMetadata", (client, token) => client.GetMetadata(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, token)),
            [nameof(BicepClient.GetSnapshot)] = ("bicep/getSnapshot", (client, token) => client.GetSnapshot(new("main.bicepparam", new(null, null, null, null, null, null), null) { ConfigPath = "bicepconfig.ci.json" }, token)),
        };

    private static IEnumerable<object[]> GetOperationsWithConfigPath() => RequestsWithConfigPath.Keys.Select(x => new object[] { x });

    [TestMethod]
    [DynamicData(nameof(GetOperationsWithConfigPath))]
    public async Task Requests_with_config_path_throw_when_cli_version_is_below_minimum(string operationName)
    {
        var (method, invoke) = RequestsWithConfigPath[operationName];
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.47.16"));
        using var client = new BicepClient(rpc);

        await FluentActions.Invoking(() => invoke(client, Token))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Operation '{operationName} with ConfigPath' requires Bicep CLI version '0.48.0' or later, whereas '0.47.16' is currently installed.");

        rpc.CallCount(method).Should().Be(0);
    }

    [TestMethod]
    public async Task Compile_with_config_path_succeeds_when_cli_version_meets_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.48.0"));
        rpc.SetResponse("bicep/compile", new CompileResponse(true, [], "{}"));
        using var client = new BicepClient(rpc);

        var result = await client.Compile(new("main.bicep") { ConfigPath = "bicepconfig.ci.json" }, Token);

        result.Success.Should().BeTrue();
        rpc.CallCount("bicep/compile").Should().Be(1);
    }

    [TestMethod]
    public async Task Compile_without_config_path_does_not_check_cli_version()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/compile", new CompileResponse(true, [], "{}"));
        using var client = new BicepClient(rpc);

        await client.Compile(new("main.bicep"), Token);

        rpc.CallCount("bicep/version").Should().Be(0);
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
