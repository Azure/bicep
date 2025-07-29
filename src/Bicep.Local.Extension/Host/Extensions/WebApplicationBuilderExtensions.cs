// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.CommandLineArguments;
using Bicep.Local.Extension.Types;
using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace Bicep.Local.Extension.Host.Extensions;

public static class WebApplicationBuilderExtensions
{
    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var isEnabled) && isEnabled;

    /// <summary>
    /// Configures the web application builder to host a Bicep extension with command-line argument parsing and gRPC server setup.
    /// </summary>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder();
    /// builder.AddBicepExtensionHost(args);
    /// builder.Services.AddBicepExtension("MyExtension", "1.0.0", true);
    /// var app = builder.Build();
    /// app.MapBicepExtension&lt;MyResourceDispatcher&gt;();
    /// await app.RunBicepExtensionAsync();
    /// </code>
    /// </example>
    public static WebApplicationBuilder AddBicepExtensionHost(this WebApplicationBuilder builder, string[] args)
    {
        if (IsTracingEnabled)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        builder.Services.AddSingleton(serviceProvider => new CommandLineParser(args, serviceProvider.GetRequiredService<ILogger<CommandLineParser>>()));

        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            var commandLindParser = options.ApplicationServices.GetRequiredService<CommandLineParser>();

            if (commandLindParser.ShouldExit)
            {
                Environment.Exit(commandLindParser.ExitCode);
            }

            switch (commandLindParser.Options)
            {
                case { Socket: { } socket, Pipe: null, Http: null }:
                    options.ListenUnixSocket(socket, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                case { Socket: null, Pipe: { } pipe, Http: null }:
                    options.ListenNamedPipe(pipe, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                case { Socket: null, Pipe: null, Http: { } port }:
                    if (port <= 0 || port > 65535)
                    {
                        throw new ArgumentOutOfRangeException(nameof(port), "HTTP port must be between 1 and 65535.");
                    }
                    options.ListenLocalhost(port, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                default:
                    throw new InvalidOperationException("You must specify exactly one valid value for a member of the mutual exclusion set socket, pipe, or http options.");
            }
        });

        return builder;
    }

    /// <summary>
    /// Maps the default Bicep resource dispatcher to handle gRPC requests for the extension.
    /// </summary>
    /// <example>
    /// <code>
    /// var app = builder.Build();
    /// app.MapBicepExtension(); // Uses default ResourceRequestDispatcher
    /// await app.RunBicepExtensionAsync();
    /// </code>
    /// </example>
    public static WebApplication MapBicepExtension(this WebApplication app)
    {
        app.MapGrpcService<BicepExtension>();

        var env = app.Environment;
        if (env.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        return app;
    }
}
