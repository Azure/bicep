// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

public static class PublicRegistryModuleMetadataProviderExtensions
{
    public static IServiceCollection AddPublicRegistryModuleMetadataProviderServices(this IServiceCollection services)
    {
        services.AddSingleton<IRegistryModuleMetadataProvider, PublicRegistryModuleMetadataProvider>();

        // using type based registration for Http clients so dependencies can be injected automatically
        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        services
            .AddHttpClient<IPublicRegistryModuleIndexClient, PublicRegistryModuleMetadataClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

        return services;
    }
}


/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// </summary>
public class PublicRegistryModuleMetadataProvider : RegistryModuleMetadataProviderBase, IRegistryModuleMetadataProvider
{
    private readonly IServiceProvider serviceProvider;

    //asdfg
    //private readonly object queryingLiveSyncObject = new();
    //private Task? queryLiveDataTask;
    //private DateTime? lastSuccessfulQuery;
    //private int consecutiveFailures = 0;

    public PublicRegistryModuleMetadataProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public static RegistryModuleMetadata GetPublicRegistryModuleMetadata(string modulePath, string? description, string? documentationUri)
        => new(LanguageConstants.BicepPublicMcrRegistry, $"{LanguageConstants.BicepPublicMcrPathPrefix}{modulePath}", description, documentationUri);

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        var client = serviceProvider.GetRequiredService<IPublicRegistryModuleIndexClient>();
        var modules = await client.GetModuleIndexAsync();

        return [.. modules.Select(m =>
            new CachedModule(
                GetPublicRegistryModuleMetadata(m.ModulePath, m.GetDescription(), m.GetDocumentationUri()),
                [.. m.Versions.Select(
                    t => new RegistryModuleVersionMetadata(
                        t,
                        m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].Description:null,
                        m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].DocumentationUri:null
                    )
                )]
            )
        )];
    }
}
