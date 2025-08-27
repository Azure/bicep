// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Azure.Bicep.Types.Az;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.McpServer.ResourceProperties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bicep.McpServer;

public static class IServiceCollectionExtensions
{
    public static IMcpServerBuilder AddBicepMcpServer(this IServiceCollection services)
    {
        services
            .AddSingleton<ILogger<ResourceVisitor>>(NullLoggerFactory.Instance.CreateLogger<ResourceVisitor>())
            .AddSingleton<AzResourceTypeLoader>(provider => new(new AzTypeLoader()))
            .AddSingleton<ResourceVisitor>();

        services
            .AddSingleton<BicepTools>();

        return services.AddMcpServer(options =>
        {
            options.ServerInstructions = Constants.ServerInstructions;
        })
        .WithTools<BicepTools>();
    }

    public static IServiceCollection WithAvmSupport(this IServiceCollection services)
    {
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
