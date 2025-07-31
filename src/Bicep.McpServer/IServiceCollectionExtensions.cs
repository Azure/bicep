// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Az;
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
}
