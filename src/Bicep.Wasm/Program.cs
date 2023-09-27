// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Wasm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddSingleton<LspWorker>();

        Environment.SetEnvironmentVariable("BICEP_TRACING_ENABLED", "true");

        var lspWorker = builder.Services.BuildServiceProvider().GetService<LspWorker>() ?? throw new InvalidOperationException($"Failed to load {nameof(LspWorker)}.");
        await lspWorker.RunAsync();
    }
}
