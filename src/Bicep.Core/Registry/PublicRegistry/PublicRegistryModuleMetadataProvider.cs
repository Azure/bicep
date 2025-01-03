// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
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
    private const string Registry = $"{LanguageConstants.BicepPublicMcrRegistry}";
    private const string BasePath = "bicep/";
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
        => new(Registry, $"{BasePath}/{modulePath}", description, documentationUri);

    private async Task<ImmutableArray<RegistryModuleMetadata>?> GetLiveDataAsync()
    {
            var client = serviceProvider.GetRequiredService<IPublicRegistryModuleIndexClient>();
            var modules = await client.GetModuleIndexAsync();

    asdfg
            return modules.Select(m => GetPublicRegistryModuleMetadata(m.ModulePath, m.de, m.DocumentationUri)).ToImmutableArray();
    }
        catch (Exception ex)
        {
            this.lastDownloadError = ex.Message;
            return null;
        }
    }

    public static TimeSpan GetExponentialDelay(TimeSpan initialDelay, int consecutiveFailures, TimeSpan maxDelay)
    {
        var maxFailuresToConsider = (int)Math.Ceiling(Math.Log(maxDelay.TotalSeconds, 2)); // Avoid overflow on Math.Pow()
        var secondsDelay = initialDelay.TotalSeconds * Math.Pow(2, Math.Min(consecutiveFailures, maxFailuresToConsider));
        var delay = TimeSpan.FromSeconds(secondsDelay);

        return delay > maxDelay ? maxDelay : delay;
    }
}
