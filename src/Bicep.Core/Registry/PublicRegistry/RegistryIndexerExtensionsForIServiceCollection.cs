// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.PublicRegistry.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

public static class RegistryIndexerExtensionsForIServiceCollection
{
    public static IServiceCollection AddRegistryIndexerServices(this IServiceCollection services)
    {
        services.AddSingleton<IPublicModuleMetadataProvider, PublicModuleMetadataProvider>();
        services.AddSingleton<IRegistryIndexer, RegistryIndexer>();

        //asdfg mock these instead of mocking IPublicModuleMetadataProvider?
        // using type based registration for Http clients so dependencies can be injected automatically
        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        services
            .AddHttpClient<IPublicModuleIndexHttpClient, PublicModuleMetadataHttpClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

        return services;
    }
}
