// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Mock;
using Bicep.Local.Extension.Mock.Handlers;
using Bicep.Local.Extension.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.AddBicepExtensionHost(args);

builder.Services
    .AddBicepExtension()
    .WithDefaults("Extension.Mock", "1.0.0", isSingleton: true)
    .WithConfigurationType<Configuration>()
    //.WithTypeAssembly(typeof(Program).Assembly)
    .WithResourceHandler<EchoResourceHandler>();

var app = builder.Build();

app.MapBicepExtension();

await app.RunAsync();
