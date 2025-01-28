// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Catalog.Implementation;

public static class RegistryCatalogExtensionsForIServiceCollection
{
    public static IServiceCollection AddRegistryCatalogServices(this IServiceCollection services)
    {
        services.AddSingleton<IPublicModuleMetadataProvider, PublicModuleMetadataProvider>();
        services.AddSingleton<IRegistryModuleCatalog, RegistryModuleCatalog>();
        services.AddSingleton<IPrivateAcrModuleMetadataProviderFactory, PrivateAcrModuleMetadataProviderFactory>();

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
