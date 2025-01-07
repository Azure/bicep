// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Packaging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
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
/// Provider to get modules metadata from a private ACR registry
/// </summary>
public class PrivateAcrRegistryModuleMetadataProvider : RegistryModuleMetadataProviderBase, IRegistryModuleMetadataProvider
{
    private readonly AzureContainerRegistryManager azureContainerRegistryManager;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;
    private readonly IConfigurationManager configurationManager;

    public PrivateAcrRegistryModuleMetadataProvider(
        string registry,
        AzureContainerRegistryManager azureContainerRegistryManager,
        IContainerRegistryClientFactory containerRegistryClientFactory,
        IConfigurationManager configurationManager)
        : base(registry)
    {
        this.azureContainerRegistryManager = azureContainerRegistryManager;
        this.containerRegistryClientFactory = containerRegistryClientFactory;
        this.configurationManager = configurationManager;
    }

    protected override async Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        Uri sourceFileUri = new("C:\\Users\\stephwe\\Downloads\\main.bicep"); //asdfg

        var registry = Registry;
        var repository = modulePath;

        var tags = await this.azureContainerRegistryManager.GetArtifactTags(configurationManager.GetConfiguration(sourceFileUri), registry, repository);
        return [.. tags.Select(t =>
            new RegistryModuleVersionMetadata(
                t,
                null,
                null
            )
        )];
    }

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        Uri sourceFileUri = new("C:\\Users\\stephwe\\Downloads\\main.bicep"); //asdfg
        var filter = new Regex(""); //asdfg

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var rootConfiguration = configurationManager.GetConfiguration(sourceFileUri);
        var catalog = await acrManager.GetCatalogAsync(rootConfiguration, Registry);
        var modules = catalog
            .Where(m => filter.IsMatch(m))
            .Select(m =>
            new CachedModule(
                new RegistryModuleMetadata(Registry, m, "asdfg description", "asdfg documentation uri"),
                null
            )
        ).ToImmutableArray();

        return modules;
    }
}
