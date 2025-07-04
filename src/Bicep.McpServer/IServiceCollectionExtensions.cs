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
    public static IServiceCollection AddMcpDependencies(this IServiceCollection services) => services
        .AddSingleton<ILogger<ResourceVisitor>>(NullLoggerFactory.Instance.CreateLogger<ResourceVisitor>())
        .AddSingleton<AzResourceTypeLoader>(provider => new(new AzTypeLoader()))
        .AddSingleton<ResourceVisitor>();
}
