// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Registry.Catalog;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bicep.Wasm;

public static class WasmServiceProvider
{
    public static IServiceCollection AddBicepWasm(this IServiceCollection services) =>
        services
            .AddSingleton<IFileExplorer, InMemoryFileExplorer>()
            .AddSingleton<IPublicModuleMetadataProvider, WasmPublicModuleMetadataProvider>()
            .AddBicepCore()
            .AddBicepDecompiler()
            .RemoveAll<IRemoteArtifactRegistry>()
            .AddSingleton<IRemoteArtifactRegistry>(sp => sp.GetRequiredService<OciArtifactRegistry>());

    public static IServiceProvider Create() =>
        new ServiceCollection()
            .AddBicepWasm()
            .BuildServiceProvider();
}
