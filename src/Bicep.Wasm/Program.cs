// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Bicep.Wasm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddBicepWasm();

        var serviceProvider = builder.Services.BuildServiceProvider();

        var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
        var interop = new Interop(
            filePath => jsRuntime.InvokeAsync<string?>("LoadQuickstartsFile", filePath).AsTask(),
            serviceProvider);
        await jsRuntime.InvokeAsync<object>("InteropInitialize", DotNetObjectReference.Create(interop));

        await builder.Build().RunAsync();
    }
}
