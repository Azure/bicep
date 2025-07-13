// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpDependencies()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<BicepTools>();

await builder.Build().RunAsync();
