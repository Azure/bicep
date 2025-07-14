// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Bicep.McpServer;
using Azure.Identity;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services
    .AddBicepMcpServer()
    .WithStdioServerTransport();

await builder.Build().RunAsync();