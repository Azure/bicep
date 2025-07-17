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
using Bicep.Local.Extension.Rpc;
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
    /// builder.Services.AddBicepExtensionServices("MyExtension", "1.0.0", true);
    /// var app = builder.Build();
    /// app.MapBicepDispatcher&lt;MyResourceDispatcher&gt;();
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

            if(commandLindParser.ShouldExit)
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
                    if(connectionOptions.Http <= 0 || connectionOptions.Http > 65535)
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
    /// Maps the specified Bicep resource dispatcher to handle gRPC requests for the extension.
    /// </summary>
    /// <typeparam name="TDispatcher">The resource dispatcher type that inherits from BicepExtensionBase and handles resource operations.</typeparam>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application for method chaining.</returns>
    /// <remarks>
    /// This method registers the dispatcher as a gRPC service and enables gRPC reflection in development environments
    /// for easier debugging with tools like grpcurl or Postman. The dispatcher handles all CRUD operations for resources
    /// managed by your extension.
    /// </remarks>
    /// <example>
    /// <code>
    /// app.MapBicepDispatcher&lt;MyResourceRequestDispatcher&gt;();
    /// </code>
    /// </example>
    public static WebApplication MapBicepDispatcher<TDispatcher>(this WebApplication app)
        where TDispatcher : BicepExtension.BicepExtensionBase
    {
        app.MapGrpcService<TDispatcher>();

        var env = app.Environment;
        if (env.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        return app;
    }

    /// <summary>
    /// Starts the Bicep extension application, handling the --describe option or running the gRPC server.
    /// </summary>
    /// <param name="app">The configured web application to run.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method checks command-line options to determine the execution mode:
    /// - If --describe is specified, outputs JSON type definitions to stdout and exits
    /// - If other exit conditions are met, exits with the appropriate code
    /// - Otherwise, starts the gRPC server to handle resource requests
    /// Call this method as the final step in your application startup.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the app parameter is null.</exception>
    /// <example>
    /// <code>
    /// var app = builder.Build();
    /// app.MapBicepDispatcher&lt;MyResourceDispatcher&gt;();
    /// await app.RunBicepExtensionAsync();
    /// </code>
    /// </example>
    public static async Task RunBicepExtensionAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var commandLineParser = GetCommandLineParserService(app.Services);

        if (commandLineParser.ShouldExit)
        {
            Environment.Exit(commandLineParser.ExitCode);
        }
        else if (commandLineParser.Options.Describe)
        {
            var typeDefinitionBuilder = app.Services.GetRequiredService<ITypeDefinitionBuilder>();
            var typeDefinition = typeDefinitionBuilder.GenerateBicepResourceTypes();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
            };

            var stdout = Console.Out;
            await stdout.WriteLineAsync(JsonSerializer.Serialize(
                typeDefinition,
                jsonOptions));
        }
        else
        {
            await app.RunAsync();
        }
    }

    private static CommandLineParser GetCommandLineParserService(IServiceProvider serviceProvider)
        => serviceProvider.GetRequiredService<CommandLineParser>()
            ?? throw new InvalidOperationException("CommandLineParser service is not registered.");
}
