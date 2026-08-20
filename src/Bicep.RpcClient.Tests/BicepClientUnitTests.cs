// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    public void Docs_model_definition_round_trips_through_the_json_rpc_serializer()
    {
        // The RPC client serializes with System.Text.Json on netstandard2.0. This guards the
        // ImmutableSortedDictionary and nested ImmutableArray members of the docs model.
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        var response = new GetDocsModelResponse([
            new(
                "main.bicep",
                true,
                [],
                new DocsModelDefinition(
                    "Module",
                    "A module.",
                    "main.bicep",
                    "resourceGroup",
                    ImmutableSortedDictionary<string, string>.Empty
                        .Add("owner", "Platform")
                        .Add("team", "Core"),
                    [new("Microsoft.Storage/storageAccounts@2023-05-01", IsExisting: false)],
                    [
                        new(
                            "sku",
                            "skuType",
                            IsRequired: true,
                            IsSecure: false,
                            "The SKU.",
                            DefaultValue: null,
                            AllowedValues: [],
                            MinValue: null,
                            MaxValue: null,
                            MinLength: null,
                            MaxLength: null,
                            Pattern: null,
                            IsTruncated: false,
                            NestedProperties:
                            [
                                new(
                                    "name",
                                    "string",
                                    IsRequired: true,
                                    IsSecure: false,
                                    "The SKU name.",
                                    DefaultValue: null,
                                    AllowedValues: ["Premium_LRS", "Standard_LRS"],
                                    MinValue: null,
                                    MaxValue: null,
                                    MinLength: 3,
                                    MaxLength: 24,
                                    Pattern: "^[a-z]+$",
                                    IsTruncated: false,
                                    NestedProperties: [],
                                    Discriminator: null),
                            ],
                            Discriminator: new(
                                "kind",
                                [new("StorageV2", [])])),
                    ],
                    [new("resourceId", "string", IsSecure: false, "The resource id.")],
                    ExportedTypes: [],
                    ExportedVariables: [],
                    ExportedFunctions: [new("buildName", [new("prefix", "string", null)], "string", null)],
                    References: [new("child", "./child.bicep", "The child module.")],
                    UsageExamples: [new("Default", "examples/default/main.bicep", null, "module x '../../main.bicep' = {}")])),
        ]);

        var json = JsonSerializer.Serialize(response, options);
        json.Should().Contain("\"typeName\"").And.Contain("\"isRequired\"").And.Contain("\"nestedProperties\"");

        var deserialized = JsonSerializer.Deserialize<GetDocsModelResponse>(json, options);

        deserialized.Should().NotBeNull();
        var model = deserialized!.Results.Single().Model;
        model.Should().NotBeNull();
        model!.Custom.Should().Equal(response.Results[0].Model!.Custom);
        model.ResourceTypes.Should().ContainSingle();
        model.Parameters.Should().ContainSingle();
        model.Parameters[0].NestedProperties.Should().ContainSingle();
        model.Parameters[0].NestedProperties[0].AllowedValues.Should().Equal("Premium_LRS", "Standard_LRS");
        model.Parameters[0].NestedProperties[0].MaxLength.Should().Be(24);
        model.Parameters[0].Discriminator!.Cases.Should().ContainSingle();
        model.ExportedFunctions[0].Parameters.Should().ContainSingle();
        model.References[0].Path.Should().Be("./child.bicep");
        model.UsageExamples[0].RelativePath.Should().Be("examples/default/main.bicep");
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
    public async Task RenderDocs_forwards_request_to_the_expected_method()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.47.0"));
        rpc.SetResponse(
            "bicep/renderDocs",
            new RenderDocsResponse([new("main.bicep", true, [], "# Module\n")]));
        using var client = new BicepClient(rpc);

        var result = await client.RenderDocs(
            new(["main.bicep"], null, null, null, NoRestore: false),
            Token);

        result.Results.Should().ContainSingle();
        result.Results[0].Contents.Should().Be("# Module\n");
        rpc.CallCount("bicep/renderDocs").Should().Be(1);
    }

    [TestMethod]
    public async Task GetDocsModel_forwards_request_to_the_expected_method()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.47.0"));
        rpc.SetResponse("bicep/getDocsModel", new GetDocsModelResponse([]));
        using var client = new BicepClient(rpc);

        var result = await client.GetDocsModel(
            new(["main.bicep"], NoRestore: false),
            Token);

        result.Results.Should().BeEmpty();
        rpc.CallCount("bicep/getDocsModel").Should().Be(1);
    }

    [TestMethod]
    public void Docs_models_expose_constructor_values()
    {
        var custom = new Dictionary<string, string> { ["owner"] = "Platform" };
        var renderRequest = new RenderDocsRequest(
            ["main.bicep"],
            "template.scriban",
            "templates",
            custom,
            NoRestore: true);
        var modelRequest = new GetDocsModelRequest(
            ["main.bicep"],
            NoRestore: true);
        var result = new DocsResult("main.bicep", true, [], "# Module\n");

        renderRequest.Paths.Should().Equal("main.bicep");
        renderRequest.TemplateFile.Should().Be("template.scriban");
        renderRequest.TemplateRoot.Should().Be("templates");
        renderRequest.CustomTemplateValues.Should().BeSameAs(custom);
        renderRequest.NoRestore.Should().BeTrue();
        modelRequest.Paths.Should().Equal("main.bicep");
        modelRequest.NoRestore.Should().BeTrue();
        result.Path.Should().Be("main.bicep");
        result.Success.Should().BeTrue();
        result.Diagnostics.Should().BeEmpty();
        result.Contents.Should().Be("# Module\n");
    }

    [TestMethod]
    public void Docs_model_definition_exposes_constructor_values()
    {
        var parameter = new DocsModelDefinition.ParameterDefinition(
            "location",
            "string",
            IsRequired: false,
            IsSecure: false,
            "Deployment location.",
            "'westeurope'",
            ["westeurope", "eastus"],
            MinValue: null,
            MaxValue: null,
            MinLength: 1,
            MaxLength: 64,
            "^[a-z]+$",
            IsTruncated: false,
            NestedProperties: [],
            Discriminator: null);

        var model = new DocsModelDefinition(
            "Module",
            "A module.",
            "main.bicep",
            "resourceGroup",
            ImmutableSortedDictionary<string, string>.Empty.Add("owner", "Platform"),
            [new("Microsoft.Storage/storageAccounts@2023-05-01", IsExisting: false)],
            [parameter],
            [new("id", "string", IsSecure: false, "Resource id.")],
            ExportedTypes: [],
            ExportedVariables: [],
            ExportedFunctions: [],
            References: [],
            UsageExamples: []);

        model.Name.Should().Be("Module");
        model.TargetScope.Should().Be("resourceGroup");
        model.Custom["owner"].Should().Be("Platform");
        model.ResourceTypes.Should().ContainSingle();
        model.Parameters.Should().ContainSingle();
        model.Parameters[0].TypeName.Should().Be("string");
        model.Parameters[0].AllowedValues.Should().Equal("westeurope", "eastus");
        model.Parameters[0].MaxLength.Should().Be(64);
        model.Outputs.Should().ContainSingle();
        model.Outputs[0].Name.Should().Be("id");
    }

    [TestMethod]
    public async Task Docs_methods_throw_when_cli_version_is_below_minimum()
    {
        var rpc = new FakeJsonRpcClient();
        rpc.SetResponse("bicep/version", new VersionResponse("0.46.1"));
        using var client = new BicepClient(rpc);

        await FluentActions.Invoking(() => client.RenderDocs(
                new(["main.bicep"], null, null, null, NoRestore: false),
                Token))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*requires Bicep CLI version '0.47.0' or later*");
        await FluentActions.Invoking(() => client.GetDocsModel(
                new(["main.bicep"], NoRestore: false),
                Token))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*requires Bicep CLI version '0.47.0' or later*");

        rpc.CallCount("bicep/renderDocs").Should().Be(0);
        rpc.CallCount("bicep/getDocsModel").Should().Be(0);
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
