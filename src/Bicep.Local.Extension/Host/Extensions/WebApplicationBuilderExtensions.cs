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
using Bicep.Local.Rpc;
using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static System.Net.WebRequestMethods;

namespace Bicep.Local.Extension.Host.Extensions;

public static class WebApplicationBuilderExtensions
{
    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var isEnabled) && isEnabled;

    /// <summary>
    /// Configures an authenticated extension host while preserving the original synchronous startup API.
    /// Blocks until the bounded stdin bootstrap completes; authentication failures are propagated to the caller.
    /// </summary>
    public static WebApplicationBuilder AddBicepExtensionHost(this WebApplicationBuilder builder, string[] args)
    {
        // Run the bootstrap without the caller's synchronization context before blocking for compatibility.
#pragma warning disable VSTHRD002
        return Task.Run(() => builder.AddBicepExtensionHostAsync(args)).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
    }

    /// <summary>
    /// Configures the web application builder to host a Bicep extension with command-line argument parsing and gRPC server setup.
    /// </summary>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder();
    /// await builder.AddBicepExtensionHostAsync(args);
    /// builder.Services.AddBicepExtension("MyExtension", "1.0.0", true);
    /// var app = builder.Build();
    /// app.MapBicepExtension&lt;MyResourceDispatcher&gt;();
    /// await app.RunAsync();
    /// </code>
    /// </example>
    public static async Task<WebApplicationBuilder> AddBicepExtensionHostAsync(this WebApplicationBuilder builder, string[] args)
    {
        using var input = Console.OpenStandardInput();
        return await builder.AddBicepExtensionHostAsync(args, input);
    }

    /// <summary>
    /// Configures an authenticated host using a caller-owned bootstrap input, for embedded hosts and tests.
    /// </summary>
    public static async Task<WebApplicationBuilder> AddBicepExtensionHostAsync(this WebApplicationBuilder builder, string[] args, Stream bootstrapInput)
    {
        if (IsTracingEnabled)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        var parser = new CommandLineParser(args, NullLogger<CommandLineParser>.Instance);
        if (parser.ShouldExit)
        {
            Environment.Exit(parser.ExitCode);
        }

        var authentication = await LocalExtensionAuthentication.ReadBootstrapAsync(bootstrapInput, CancellationToken.None);
        builder.Services.AddSingleton(_ => authentication);
        builder.Services.AddTransient<IStartupFilter, ClientCertificateValidationStartupFilter>();

        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            var authentication = options.ApplicationServices.GetRequiredService<LocalExtensionAuthentication>();
            void ConfigureListener(ListenOptions listener)
            {
                listener.Protocols = HttpProtocols.Http2;
                listener.UseHttps(https =>
                {
                    https.ServerCertificate = authentication.ServerCertificate;
                    https.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    https.CheckCertificateRevocation = false;
                    https.ClientCertificateValidation = (certificate, _, _) =>
                        LocalExtensionAuthentication.MatchesCertificate(certificate, authentication.ClientCertificate);
                });
            }

            switch (parser.Options)
            {
                case { Socket: { } socket, Pipe: null, Http: null }:
                    options.ListenUnixSocket(socket, ConfigureListener);
                    break;
                case { Socket: null, Pipe: { } pipe, Http: null }:
                    options.ListenNamedPipe(pipe, ConfigureListener);
                    break;
                case { Socket: null, Pipe: null, Http: { } port }:
                    if (port <= 0 || port > 65535)
                    {
                        throw new ArgumentOutOfRangeException(nameof(port), "HTTP port must be between 1 and 65535.");
                    }
                    options.ListenLocalhost(port, ConfigureListener);
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
    /// await app.RunAsync();
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

    private sealed class ClientCertificateValidationStartupFilter(LocalExtensionAuthentication authentication) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            => app =>
            {
                app.Use(async (context, next) =>
                {
                    if (!LocalExtensionAuthentication.MatchesCertificate(context.Connection.ClientCertificate, authentication.ClientCertificate))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }

                    await next(context);
                });
                next(app);
            };
    }
}
