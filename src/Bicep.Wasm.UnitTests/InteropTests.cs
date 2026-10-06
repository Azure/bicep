// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Wasm.UnitTests;

[TestClass]
public class InteropTests
{
    [TestMethod]
    public async Task CompileAndEmitDiagnostics_WithQuickstartModules_LoadsModulesRecursively()
    {
        var quickstartFiles = new Dictionary<string, string>
        {
            ["example/modules/child.bicep"] = """
                module grandchild '../shared/grandchild.bicep' = {
                  name: 'grandchild'
                }

                output name string = grandchild.outputs.name
                """,
            ["example/shared/grandchild.bicep"] = """
                output name string = 'from-grandchild'
                """,
        };

        var jsRuntime = new MockJsRuntime(quickstartFiles);
        var fileExplorer = new InMemoryFileExplorer();
        using var serviceProvider = CreateServiceProvider(fileExplorer);
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var result = await interop.CompileAndEmitDiagnostics(
            """
            module child './modules/child.bicep' = {
              name: 'child'
            }

            output childName string = child.outputs.name
            """,
            "example/main.bicep");

        using var template = JsonDocument.Parse(result.template);
        template.RootElement.GetProperty("resources").GetArrayLength().Should().Be(1);
        jsRuntime.LoadedPaths.Should().Equal(
            "example/modules/child.bicep",
            "example/shared/grandchild.bicep");
        fileExplorer.GetFile(IOUri.FromFilePath("/quickstarts/example/modules/child.bicep")).Exists().Should().BeTrue();
        fileExplorer.GetFile(IOUri.FromFilePath("/quickstarts/example/shared/grandchild.bicep")).Exists().Should().BeTrue();
    }

    [TestMethod]
    public async Task Decompile_WithInMemoryFileExplorer_UsesLocalEntrypointUri()
    {
        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        var fileExplorer = new InMemoryFileExplorer();
        using var serviceProvider = CreateServiceProvider(fileExplorer);
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var result = await interop.Decompile(
            """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "parameters": {
                "foo": {
                  "type": "string"
                }
              },
              "resources": []
            }
            """);

        result.error.Should().BeNull();
        result.bicepFile.Should().Contain("param foo string");
        result.entrypoint.Should().Be("main.bicep");
        result.files.Should().ContainSingle().Which.Should().Be(
            new KeyValuePair<string, string>("main.bicep", result.bicepFile!));
    }

    [TestMethod]
    public async Task Decompile_WithNestedOuterScopedTemplates_ReturnsEveryGeneratedFile()
    {
        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        using var serviceProvider = CreateServiceProvider(new InMemoryFileExplorer());
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);
        var template = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Files", "create-vm.json"));

        var result = await interop.Decompile(template);

        result.error.Should().BeNull();
        result.entrypoint.Should().Be("main.bicep");
        result.files.Should().HaveCount(4);
        result.files!.Keys.Should().BeEquivalentTo("main.bicep", "nested_newVnet.bicep", "nested_storage.bicep", "nested_metadata.bicep");
        result.bicepFile.Should().Be(result.files[result.entrypoint!]);
        result.bicepFile.Should().Contain("Microsoft.Compute/virtualMachines");
        result.bicepFile.Should().Contain("vnetName: vnetName");
        result.bicepFile.Should().Contain("'./nested_metadata.bicep'").And.Contain("innerVmName: vmName");
        result.files["nested_newVnet.bicep"].Should()
            .Contain("param vnetName string")
            .And.Contain("Microsoft.Network/virtualNetworks")
            .And.Contain("location: resourceGroup().location")
            .And.Contain("'./nested_storage.bicep'");
        result.files["nested_storage.bicep"].Should()
            .Contain("Microsoft.Storage/storageAccounts")
            .And.Contain("'hackathonstorage'");
        result.files["nested_metadata.bicep"].Should()
            .Contain("param innerVmName string")
            .And.Contain("output name string = innerVmName");

        foreach (var (path, content) in result.files)
        {
            path.Should().NotContain("\\").And.NotContain(":").And.NotStartWith("/");
            path.Split('/').Should().NotContain("..");
            foreach (Match module in Regex.Matches(content, @"(?m)^module\s+\w+\s+'([^']+)'"))
            {
                var moduleUri = IOUri.FromFilePath($"/{path}").Resolve(module.Groups[1].Value);
                result.files.Should().ContainKey(moduleUri.GetPathRelativeTo(IOUri.FromFilePath("/")));
            }
        }

        using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        serialized.RootElement.GetProperty("bicepFile").GetString().Should().Be(result.bicepFile);
        serialized.RootElement.GetProperty("error").ValueKind.Should().Be(JsonValueKind.Null);
        serialized.RootElement.GetProperty("entrypoint").GetString().Should().Be("main.bicep");
        serialized.RootElement.GetProperty("files").EnumerateObject()
            .ToDictionary(file => file.Name, file => file.Value.GetString())
            .Should().BeEquivalentTo(result.files);
        jsRuntime.LoadedPaths.Should().BeEmpty();
    }

    [TestMethod]
    [DataRow("not json")]
    [DataRow("{}")]
    public async Task Decompile_WithInvalidTemplate_ReturnsOnlyError(string template)
    {
        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        using var serviceProvider = CreateServiceProvider(new InMemoryFileExplorer());
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var result = await interop.Decompile(template);

        result.error.Should().NotBeNullOrWhiteSpace();
        result.bicepFile.Should().BeNull();
        result.entrypoint.Should().BeNull();
        result.files.Should().BeNull();

        using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        serialized.RootElement.GetProperty("error").GetString().Should().Be(result.error);
        serialized.RootElement.GetProperty("bicepFile").ValueKind.Should().Be(JsonValueKind.Null);
        serialized.RootElement.GetProperty("entrypoint").ValueKind.Should().Be(JsonValueKind.Null);
        serialized.RootElement.GetProperty("files").ValueKind.Should().Be(JsonValueKind.Null);
    }

    [TestMethod]
    public async Task Decompile_WithInvalidOuterScopedTemplate_DoesNotReturnPartialFiles()
    {
        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        using var serviceProvider = CreateServiceProvider(new InMemoryFileExplorer());
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);
        var template = JsonNode.Parse(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Files", "create-vm.json")))!;
        var nestedTemplate = template["resources"]![1]!["properties"]!["template"]!;
        nestedTemplate["parameters"] = JsonNode.Parse("""{"unexpected": {"type": "string"}}""");

        var result = await interop.Decompile(template.ToJsonString());

        result.error.Should().Contain("Outer-scoped nested templates cannot contain parameters");
        result.bicepFile.Should().BeNull();
        result.entrypoint.Should().BeNull();
        result.files.Should().BeNull();
        jsRuntime.LoadedPaths.Should().BeEmpty();
    }

    [TestMethod]
    public async Task CompileAndEmitDiagnostics_WithRemoteOciModule_RestoresModule()
    {
        var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
            new MockFileSystem(),
            new ModuleToPublish(
                "br:mcr.microsoft.com/bicep/test/module:v1",
                "output name string = 'from-registry'"));

        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        var fileExplorer = new InMemoryFileExplorer();
        using var serviceProvider = CreateServiceProvider(fileExplorer, clientFactory);
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var result = await interop.CompileAndEmitDiagnostics(
            """
            module remote 'br:mcr.microsoft.com/bicep/test/module:v1' = {
              name: 'remote'
            }

            output remoteName string = remote.outputs.name
            """,
            null);

        using var template = JsonDocument.Parse(result.template);
        template.RootElement.GetProperty("resources").GetArrayLength().Should().Be(1);
    }

    [TestMethod]
    public async Task CompileAndEmitDiagnostics_WithEntrypointDiagnostic_MapsDiagnosticToEntrypointSpan()
    {
        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        var fileExplorer = new InMemoryFileExplorer();
        using var serviceProvider = CreateServiceProvider(fileExplorer);
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var result = await interop.CompileAndEmitDiagnostics(
            """
            var value = 'hello'
            output bad string = value.missing
            """,
            null);

        var diagnostic = result.diagnostics.Should().BeAssignableTo<object[]>().Subject.Should().ContainSingle().Subject;

        GetProperty<int>(diagnostic, "startLineNumber").Should().Be(2);
    }

    [TestMethod]
    public async Task CompileAndEmitDiagnostics_WithConcurrentRequests_DoesNotMixCompilationState()
    {
        var jsRuntime = new MockJsRuntime(new Dictionary<string, string>());
        var fileExplorer = new InMemoryFileExplorer();
        using var serviceProvider = CreateServiceProvider(fileExplorer);
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var firstCompilation = interop.CompileAndEmitDiagnostics(
            "output result string = 'first'",
            null);
        var secondCompilation = interop.CompileAndEmitDiagnostics(
            "output result string = 'second'",
            null);

        var results = await Task.WhenAll(firstCompilation, secondCompilation);

        using var firstTemplate = JsonDocument.Parse(results[0].template);
        using var secondTemplate = JsonDocument.Parse(results[1].template);
        firstTemplate.RootElement
            .GetProperty("outputs")
            .GetProperty("result")
            .GetProperty("value")
            .GetString()
            .Should()
            .Be("first");
        secondTemplate.RootElement
            .GetProperty("outputs")
            .GetProperty("result")
            .GetProperty("value")
            .GetString()
            .Should()
            .Be("second");
    }

    [TestMethod]
    public async Task CompileAndEmitDiagnostics_WithSameContentAndDifferentSourcePath_DoesNotReuseCachedCompilation()
    {
        const string source = """
            module child './modules/child.bicep' = {
              name: 'child'
            }

            output childName string = child.outputs.name
            """;
        var quickstartFiles = new Dictionary<string, string>
        {
            ["example/modules/child.bicep"] = "output name string = 'from-child'",
        };
        var jsRuntime = new MockJsRuntime(quickstartFiles);
        var fileExplorer = new InMemoryFileExplorer();
        using var serviceProvider = CreateServiceProvider(fileExplorer);
        var interop = new Interop(jsRuntime.LoadQuickstart, serviceProvider);

        var quickstartResult = await interop.CompileAndEmitDiagnostics(source, "example/main.bicep");
        var standaloneResult = await interop.CompileAndEmitDiagnostics(source, null);

        using var quickstartTemplate = JsonDocument.Parse(quickstartResult.template);
        quickstartTemplate.RootElement.GetProperty("resources").GetArrayLength().Should().Be(1);
        standaloneResult.template.Should().Be("Compilation failed!");
        standaloneResult.diagnostics.Should().BeAssignableTo<object[]>().Subject.Should().NotBeEmpty();
        jsRuntime.LoadedPaths.Should().Equal("example/modules/child.bicep");
    }

    private static ServiceProvider CreateServiceProvider(IFileExplorer fileExplorer, IContainerRegistryClientFactory? clientFactory = null)
    {
        var services = new ServiceCollection();

        services.AddSingleton(fileExplorer);
        services.AddSingleton<IArtifactRegistryProvider, WasmModuleRegistryProvider>();
        services.AddSingleton<IPublicModuleMetadataProvider, WasmPublicModuleMetadataProvider>();

        if (clientFactory is not null)
        {
            services.AddSingleton(clientFactory);
        }

        services.AddBicepCore();
        services.AddBicepDecompiler();

        return services.BuildServiceProvider();
    }

    private static T GetProperty<T>(object @object, string propertyName)
        => (T)@object.GetType().GetProperty(propertyName)!.GetValue(@object)!;

    private sealed class MockJsRuntime(IReadOnlyDictionary<string, string> files)
    {
        private readonly List<string> loadedPaths = [];

        public IReadOnlyList<string> LoadedPaths => this.loadedPaths;

        public Task<string?> LoadQuickstart(string filePath)
        {
            this.loadedPaths.Add(filePath);
            files.TryGetValue(filePath, out var contents);

            return Task.FromResult(contents);
        }
    }
}
