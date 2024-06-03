// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using Microsoft.AspNetCore.Builder;
using Bicep.Extension.Rpc;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Bicep.Extension.Protocol;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Bicep.Extension;

public class ProviderExtension
{
    internal class CommandLineOptions
    {
        [Option("socket", Required = false, HelpText = "The path to the domain socket to connect on")]
        public string? Socket { get; set; }

        [Option("wait-for-debugger", Required = false, HelpText = "If set, wait for a dotnet debugger to be attached before starting the server")]
        public bool WaitForDebugger { get; set; }
    }

    public static async Task Run(Action<ResourceDispatcherBuilder> registerHandlers, string[] args)
        => await RunWithCancellationAsync(async cancellationToken =>
        {
            if (IsTracingEnabled)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
            }

            var extension = new ProviderExtension();

            await extension.RunAsync(args, registerHandlers, cancellationToken);
        });

    public async Task RunAsync(string[] args, Action<ResourceDispatcherBuilder> registerHandlers, CancellationToken cancellationToken)
    {
        var parser = new Parser(settings =>
        {
            settings.IgnoreUnknownArguments = true;
        });

        await parser.ParseArguments<CommandLineOptions>(args)
            .WithNotParsed((x) => System.Environment.Exit(1))
            .WithParsedAsync(async options => await RunServer(registerHandlers, options, cancellationToken));
    }

    private static async Task RunServer(Action<ResourceDispatcherBuilder> registerHandlers, CommandLineOptions options, CancellationToken cancellationToken)
    {
        if (options.WaitForDebugger)
        {
            // exit if we don't have a debugger attached within 5 minutes
            var debuggerTimeoutToken = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token).Token;

            while (!Debugger.IsAttached)
            {
                await Task.Delay(100, debuggerTimeoutToken);
            }

            Debugger.Break();
        }

        var handlerBuilder = new ResourceDispatcherBuilder();
        registerHandlers(handlerBuilder);
        var dispatcher = handlerBuilder.Build();

        if (options.Socket is { } socketPath)
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenUnixSocket(socketPath, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
    
            builder.Services.AddGrpc();
            builder.Services.AddSingleton(dispatcher);
            var app = builder.Build();
            app.MapGrpcService<BicepExtensionImpl>();

            await Task.WhenAny(app.RunAsync(), WaitForCancellation(cancellationToken));
            return;
        }

        throw new NotImplementedException();
    }

    private static async Task WaitForCancellation(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(-1, cancellationToken);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            // don't throw - continue
        }
    }

    private static async Task RunWithCancellationAsync(Func<CancellationToken, Task> runFunc)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            cancellationTokenSource.Cancel();
            e.Cancel = true;
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            cancellationTokenSource.Cancel();
        };

        try
        {
            await runFunc(cancellationTokenSource.Token);
        }
        catch (OperationCanceledException exception) when (exception.CancellationToken == cancellationTokenSource.Token)
        {
            // this is expected - no need to rethrow
        }
    }

    private static bool IsTracingEnabled
        => bool.TryParse(Environment.GetEnvironmentVariable("BICEP_TRACING_ENABLED"), out var value) && value;
}