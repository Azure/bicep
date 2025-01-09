// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Bicep.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

public static class RegistryModuleIndexerExtensionsForIServiceCollection
{
    public static IServiceCollection AddRegistryIndexerServices(this IServiceCollection services)
    {
        services.AddSingleton<IPublicRegistryModuleMetadataProvider, PublicRegistryModuleMetadataProvider>();
        services.AddSingleton<IRegistryModuleIndexer, RegistryModuleIndexer>();

        //asdfg mock these instead of mocking IPublicRegistryModuleMetadataProvider?
        // using type based registration for Http clients so dependencies can be injected automatically
        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        services
            .AddHttpClient<IPublicRegistryModuleIndexHttpClient, PublicRegistryModuleMetadataHttpClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        //services asdfg remove
        //    .AddHttpClient<IOciCatalogHttpClient, OciCatalogHttpClient>()
        //    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        //    {
        //        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        //    });

        return services;
    }
}
