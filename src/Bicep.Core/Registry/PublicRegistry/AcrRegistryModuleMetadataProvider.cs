// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;//asdfg rename folder/namespace PublicRegistry?

//asdfg public interface IAcrRegistryModuleMetadataProvider : IRegistryModuleMetadataProvider { }

//public static class AcrRegistryModuleMetadataProviderExtensions asdfg
//{
//    public static IServiceCollection AddAcrRegistryModuleMetadataProviderServices(this IServiceCollection services)
//    {
//        services.AddSingleton<AcrRegistryModuleMetadataProvider, AcrRegistryModuleMetadataProvider>();

//        // using type based registration for Http clients so dependencies can be injected automatically
//        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
//        services
//            .AddHttpClient<IAcrRegistryModuleCatalogClient, AcrRegistryModuleCatalogClient>()
//            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//            {
//                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
//            });

//        return services;
//    }
//}

/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// </summary>
public class AcrRegistryModuleMetadataProvider : RegistryModuleMetadataProviderBase
{
    private readonly IServiceProvider serviceProvider;

    public AcrRegistryModuleMetadataProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        var client = serviceProvider.GetRequiredService<IAcrRegistryModuleCatalogClient>();
        var modules = await client.TryGetCatalog("sawbiceppublic.azurecr.io"); //asdfgasdfg move to config
        return [];
    }
}
