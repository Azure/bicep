// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Bicep.Core.Configuration;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.McpServer.Core.Extensions;
using Bicep.McpServer.Core.ResourceProperties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Protocol;

namespace Bicep.McpServer.Core;

public static class IServiceCollectionExtensions
{
    public static IMcpServerBuilder AddBicepMcpServer(this IServiceCollection services)
    {
        services
            .AddSingleton(NullLoggerFactory.Instance.CreateLogger<ResourceVisitor>())
            .AddSingleton<ITypeLoader>(new AzTypeLoader())
            .AddSingleton<AzResourceTypeLoader>(provider => new(provider.GetRequiredService<ITypeLoader>()))
            .AddSingleton<ResourceVisitor>()
            .AddSingleton<ExtensionTypeLoaderProvider>()
            // Registered before AddBicepCore, which keeps the first registration. The server compiles on request,
            // so files read by load*() functions aren't cached between tool calls (see NonCachingAuxiliaryFileCache).
            .AddSingleton<IAuxiliaryFileCache, NonCachingAuxiliaryFileCache>()
            .AddBicepCore()
            .AddBicepDecompiler();

        services
            .AddSingleton<BicepTools>()
            .AddSingleton<BicepCompilerTools>()
            .AddSingleton<BicepDecompilerTools>()
            .AddSingleton<BicepDeploymentTools>();

        return services.AddMcpServer(options =>
        {
            options.ServerInstructions = Constants.ServerInstructions;
        })
        .WithTools<BicepTools>()
        .WithTools<BicepCompilerTools>()
        .WithTools<BicepDecompilerTools>()
        .WithTools<BicepDeploymentTools>()
        .WithRequestFilters(filters =>
        {
            filters.AddCallToolFilter(next => async (context, cancellationToken) =>
            {
                // The server is long-running and doesn't watch the file system, so configuration cached by an earlier call may be stale.
                context.Services?.GetService<IBicepConfigurationManager>()?.InvalidateAll();

                try
                {
                    return await next(context, cancellationToken);
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
        });
    }
}
