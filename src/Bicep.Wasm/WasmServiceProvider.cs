// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Wasm;

public static class WasmServiceProvider
{
    public static IServiceCollection AddBicepWasm(this IServiceCollection services) =>
        services
            .AddSingleton<IFileExplorer, InMemoryFileExplorer>()
            .AddSingleton<IArtifactRegistryProvider, WasmModuleRegistryProvider>()
            .AddSingleton<IPublicModuleMetadataProvider, WasmPublicModuleMetadataProvider>()
            .AddBicepCore()
            .AddBicepDecompiler();

    public static IServiceProvider Create() =>
        new ServiceCollection()
            .AddBicepWasm()
            .BuildServiceProvider();
}
