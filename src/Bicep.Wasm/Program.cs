// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Registry;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Bicep.Wasm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddSingleton<IFileSystem, MockFileSystem>();
        builder.Services.AddSingleton<IArtifactRegistryProvider, EmptyModuleRegistryProvider>();
        builder.Services.AddBicepCore();
        builder.Services.AddBicepDecompiler();

        var serviceProvider = builder.Services.BuildServiceProvider();

        var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
        var interop = new Interop(jsRuntime, serviceProvider);
        await jsRuntime.InvokeAsync<object>("InteropInitialize", DotNetObjectReference.Create(interop));

        await builder.Build().RunAsync();
    }
}
