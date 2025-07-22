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

/// <summary>
/// Provides extension methods for configuring ASP.NET Core applications to host Bicep extensions.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var isEnabled) && isEnabled;

    /// <summary>
    /// Configures the web application builder to host a Bicep extension with command-line argument parsing and gRPC server setup.
    /// </summary>
    /// <param name="builder">The web application builder to configure.</param>
    /// <param name="args">Command-line arguments passed to the application for parsing connection options.</param>
    /// <returns>The configured web application builder for method chaining.</returns>
    /// <remarks>
    /// This method sets up command-line parsing, configures Kestrel to listen on the specified transport
    /// (Unix socket, named pipe, or HTTP), and enables diagnostic tracing if the BICEP_TRACING_ENABLED
    /// environment variable is set to true. Call this method early in your application startup.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the HTTP port is not between 1 and 65535.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no valid connection option is specified or multiple conflicting options are provided.</exception>
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
            var commandLindParser = GetCommandLineParserService(options.ApplicationServices);

            if (commandLindParser.ShouldExit)
            {
                Environment.Exit(commandLindParser.ExitCode);
            }

            var connectionOptions = (commandLindParser.Options.Socket,
                                     commandLindParser.Options.Pipe,
                                     commandLindParser.Options.Http);

            switch (connectionOptions)
            {
                case { Socket: { }, Pipe: null, Http: null }:
                    options.ListenUnixSocket(connectionOptions.Socket, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                case { Socket: null, Pipe: { }, Http: null }:
                    options.ListenNamedPipe(connectionOptions.Pipe, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                case { Socket: null, Pipe: null, Http: { } }:
                    if (connectionOptions.Http <= 0 || connectionOptions.Http > 65535)
                    {
                        throw new ArgumentOutOfRangeException(nameof(connectionOptions.Socket), "HTTP port must be between 1 and 65535.");
                    }
                    options.ListenLocalhost(connectionOptions.Http.Value, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
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
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application for method chaining.</returns>
    /// <remarks>
    /// This method is a convenience overload that registers the default <see cref="ResourceRequestDispatcher"/>
    /// as a gRPC service without requiring explicit type specification. It automatically enables gRPC reflection
    /// in development environments for easier debugging with tools like grpcurl or Postman. Use this method when
    /// you don't need a custom dispatcher implementation and want to use the built-in resource handling infrastructure.
    /// </remarks>
    /// <example>
    /// <code>
    /// var app = builder.Build();
    /// app.MapBicepExtension(); // Uses default ResourceRequestDispatcher
    /// await app.RunBicepExtensionAsync();
    /// </code>
    /// </example>
    public static WebApplication MapBicepExtension(this WebApplication app)
    {
        app.MapGrpcService<ResourceRequestDispatcher>();

        var env = app.Environment;
        if (env.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        return app;
    }

    private static CommandLineParser GetCommandLineParserService(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<CommandLineParser>()
            ?? throw new InvalidOperationException("CommandLineParser service is not registered.");
}
