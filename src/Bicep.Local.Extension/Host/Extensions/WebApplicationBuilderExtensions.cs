// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bicep.Local.Extension.Host.Extensions;

public static class WebApplicationBuilderExtensions
{
    private static Dictionary<string, string> ArgumentMappings => new()
            {
                { "-d", "describe" },
                { "--describe", "describe" },

                { "-s", "socket" },
                { "--socket", "socket" },

                { "-t", "http" },
                { "--http", "http" },

                { "-p", "pipe" },
                { "--pipe", "pipe" }
            };

    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var isEnabled) && isEnabled;


    public static WebApplicationBuilder AddBicepExtensionHost(this WebApplicationBuilder builder, string[] args)
    {
        if (IsTracingEnabled)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        builder.Configuration.AddCommandLine(args, ArgumentMappings);

        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            (string? Socket, string? Pipe, int Http) connectionOptions = (context.Configuration.GetValue<string>("socket"),
                                                                          context.Configuration.GetValue<string>("pipe"),
                                                                          context.Configuration.GetValue<int>("http", 5000));

            switch (connectionOptions)
            {
                case { Socket: { }, Pipe: null }:
                    options.ListenUnixSocket(connectionOptions.Socket, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                case { Socket: null, Pipe: { } }:
                    options.ListenNamedPipe(connectionOptions.Pipe, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
                default:
                    options.ListenLocalhost(connectionOptions.Http, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    break;
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
}
