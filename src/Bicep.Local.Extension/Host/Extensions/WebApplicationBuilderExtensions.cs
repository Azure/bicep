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


public static class WebApplicationBuilderExtensions
{
    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var isEnabled) && isEnabled;


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
