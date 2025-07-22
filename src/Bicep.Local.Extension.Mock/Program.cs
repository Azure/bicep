// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bicep.Local.Extension.Host.Extensions;
using Microsoft.AspNetCore.Builder;
using Bicep.Local.Extension.Mock.Handlers;

var builder = WebApplication.CreateBuilder();

builder.AddBicepExtensionHost(args);
builder.Services
    .AddBicepExtensionServices("MockExtension", "0.0.1", true, typeof(Program).Assembly)
    .AddBicepResourceHandler<EchoResourceHandler>();

var app = builder.Build();
app.MapBicepDispatcher();

await app.RunBicepExtensionAsync();