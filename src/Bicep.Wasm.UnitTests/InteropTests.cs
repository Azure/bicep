// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
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
        var interop = new Interop(jsRuntime, serviceProvider);

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
        var interop = new Interop(jsRuntime, serviceProvider);

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
        var interop = new Interop(jsRuntime, serviceProvider);

        var result = await interop.CompileAndEmitDiagnostics(
            """
            var value = 'hello'
            output bad string = value.missing
            """,
            null);

        var diagnostic = result.diagnostics.Should().BeAssignableTo<object[]>().Subject.Should().ContainSingle().Subject;

        GetProperty<int>(diagnostic, "startLineNumber").Should().Be(2);
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

        return services.BuildServiceProvider();
    }

    private static T GetProperty<T>(object @object, string propertyName)
        => (T)@object.GetType().GetProperty(propertyName)!.GetValue(@object)!;

    private sealed class MockJsRuntime(IReadOnlyDictionary<string, string> files) : IJSRuntime
    {
        private readonly List<string> loadedPaths = [];

        public IReadOnlyList<string> LoadedPaths => this.loadedPaths;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => this.InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            identifier.Should().Be("LoadQuickstartsFile");
            args.Should().NotBeNull();
            args.Should().ContainSingle();

            var filePath = args![0].Should().BeOfType<string>().Subject;
            this.loadedPaths.Add(filePath);

            files.TryGetValue(filePath, out var contents);

            return ValueTask.FromResult((TValue)(object?)contents!);
        }
    }
}
