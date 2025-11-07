// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Mock;
using Bicep.Local.Extension.Mock.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.AddBicepExtensionHost(args);

builder.Services
    .AddBicepExtension()
    .WithExtensionInfo(name: "MockExtension", version: "0.0.1")
    .WithDefaultTypeBuilder(typeAssembly: typeof(Program).Assembly, configurationType: typeof(Configuration))
    .WithResourceHandler<EchoResourceHandler>();

var app = builder.Build();

app.MapBicepExtension();

await app.RunAsync();
