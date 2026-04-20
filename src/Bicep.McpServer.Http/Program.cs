// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
