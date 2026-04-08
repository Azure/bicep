// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var transportArg = args
    .SkipWhile(a => !a.Equals("--transport", StringComparison.OrdinalIgnoreCase))
    .Skip(1)
    .FirstOrDefault() ?? "stdio";

if (transportArg.Equals("http", StringComparison.OrdinalIgnoreCase))
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseUrls("http://*:8080");

    builder.Services
        .AddBicepMcpServer()
        .WithHttpTransport(options =>
        {
            options.Stateless = true;
        });

    var app = builder.Build();

    app.MapMcp();

    await app.RunAsync();
}
else
{
    var builder = Host.CreateEmptyApplicationBuilder(settings: null);

    builder.Services
        .AddBicepMcpServer()
        .WithStdioServerTransport();

    await builder.Build().RunAsync();
}
