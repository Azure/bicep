// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bicep.Core.Configuration;
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
public class AcrRegistryModuleMetadataProvider(
    IContainerRegistryClientFactory containerRegistryClientFactory, //asdfg lifetime
        // IAcrRegistryModuleCatalogClient acrRegistryModuleCatalogClient, //asdfg lifetime
        IConfigurationManager configurationManager) : RegistryModuleMetadataProviderBase //asdfg rename PrivateAcr...
{
    //adsfg test only gets called when registry is fully completed in input string
    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        Uri sourceFileUri = new("C:\\Users\\stephwe\\Downloads\\main.bicep"); //asdfg
        string registry = "sawbiceppublic.azurecr.io"; //asdfg
        var filter = new Regex(""); //asdfg

        if (registry.Equals(LanguageConstants.BicepPublicMcrRegistry, StringComparison.InvariantCulture))
        {
            return []; //asdfg
        }


        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
        var catalog = await acrManager.GetCatalogAsync(rootConfiguration, registry); //asdfg cache
        var modules = catalog
            .Where(m => filter.IsMatch(m))
            .Select(m =>
            new CachedModule(
                new RegistryModuleMetadata(registry, m, "asdfg description", "asdfg documentation uri"),
                [new RegistryModuleVersionMetadata("1.2.3.4", null, null)]
            )
        ).ToImmutableArray();




        //var modules = await acrRegistryModuleCatalogClient.TryGetCatalog(registry); //asdfgasdfg move to config
        return modules;
    }
}
