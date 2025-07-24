// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using Bicep.Local.Extension.Protocol;
using CommandLine;

namespace Bicep.Local.Extension;

public abstract class ProviderExtension
{
    public record ConnectionOptions(
        string? Socket,
        string? Pipe);

    internal class CommandLineOptions
    {
        [Option("socket", Required = false, HelpText = "The path to the domain socket to connect on")]
        public string? Socket { get; set; }

        [Option("pipe", Required = false, HelpText = "The named pipe to connect on")]
        public string? Pipe { get; set; }

        [Option("wait-for-debugger", Required = false, HelpText = "If set, wait for a dotnet debugger to be attached before starting the server")]
        public bool WaitForDebugger { get; set; }
    }

    public static async Task Run(ProviderExtension extension, Action<ResourceDispatcherBuilder> registerHandlers, string[] args)
        => await RunWithCancellationAsync(async cancellationToken =>
        {
            if (IsTracingEnabled)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
            }

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

    protected abstract Task RunServer(ConnectionOptions options, ResourceDispatcher dispatcher, CancellationToken cancellationToken);

    private async Task RunServer(Action<ResourceDispatcherBuilder> registerHandlers, CommandLineOptions options, CancellationToken cancellationToken)
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

        ConnectionOptions connectionOptions = new(options.Socket, options.Pipe);
        await Task.WhenAny(RunServer(connectionOptions, dispatcher, cancellationToken), WaitForCancellation(cancellationToken));
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
