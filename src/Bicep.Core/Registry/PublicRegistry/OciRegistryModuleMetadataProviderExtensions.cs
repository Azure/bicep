// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Registry.PublicRegistry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bicep.Core.Registry.PublicRegistry
{
    public static class OciRegistryModuleMetadataProviderExtensions
    {
        public static IServiceCollection AddPublicRegistryModuleMetadataProviderServices(this IServiceCollection services)//asdfg rename
        {
            services.AddSingleton<IRegistryModuleMetadataProvider, PublicRegistryModuleMetadataProvider>();
            services.AddSingleton<IRegistryModuleMetadataProvider, AcrRegistryModuleMetadataProvider>();

            services.AddSingleton<PublicRegistryModuleMetadataProvider>(); //asdfg don't do this - used keyed?

            // using type based registration for Http clients so dependencies can be injected automatically
            // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
            services
                .AddHttpClient<IPublicRegistryModuleIndexClient, PublicRegistryModuleMetadataClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });
            services
                .AddHttpClient<IAcrRegistryModuleCatalogClient, AcrRegistryModuleCatalogClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            return services;
        }
    }
}
