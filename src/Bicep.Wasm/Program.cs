// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Bicep.Wasm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddSingleton<IFileExplorer, InMemoryFileExplorer>();
        builder.Services.AddSingleton<IArtifactRegistryProvider, WasmModuleRegistryProvider>();
        builder.Services.AddSingleton<IPublicModuleMetadataProvider, WasmPublicModuleMetadataProvider>();
        builder.Services.AddBicepCore();
        builder.Services.AddBicepDecompiler();

        var serviceProvider = builder.Services.BuildServiceProvider();

        var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
        var interop = new Interop(jsRuntime, serviceProvider);
        await jsRuntime.InvokeAsync<object>("InteropInitialize", DotNetObjectReference.Create(interop));

        await builder.Build().RunAsync();
    }
}
