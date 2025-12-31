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
using ModelContextProtocol.Protocol;

namespace Bicep.McpServer;

public static class IServiceCollectionExtensions
{
    public static IMcpServerBuilder AddBicepMcpServer(this IServiceCollection services)
    {
        services
            .AddSingleton<ILogger<ResourceVisitor>>(NullLoggerFactory.Instance.CreateLogger<ResourceVisitor>())
            .AddSingleton<AzResourceTypeLoader>(provider => new(new AzTypeLoader()))
            .AddSingleton<ResourceVisitor>()
            .AddBicepCore()
            .AddBicepDecompiler();

        services
            .AddSingleton<BicepTools>()
            .AddSingleton<BicepCompilerTools>()
            .AddSingleton<BicepDecompilerTools>();

        return services.AddMcpServer(options =>
        {
            options.ServerInstructions = Constants.ServerInstructions;
        })
        .WithTools<BicepTools>()
        .WithTools<BicepCompilerTools>()
        .WithTools<BicepDecompilerTools>()
        .AddCallToolFilter((next) => async (request, cancellationToken) =>
        {
            try
            {
                return await next(request, cancellationToken);
            }
            catch (Exception ex)
            {
                return new CallToolResult
                {
                    Content = [new TextContentBlock { Text = $"Error: {ex.Message}" }],
                    IsError = true
                };
            }
        });
    }
}
