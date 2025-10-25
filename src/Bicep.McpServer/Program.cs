// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services
    .AddBicepMcpServer()
    .WithStdioServerTransport();

await builder.Build().RunAsync();
