// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Utils;
using CommandLine;

namespace Bicep.LanguageServer
{
    public class Program
    {
        public class CommandLineOptions
        {
            [Option("pipe", Required = false, HelpText = "The named pipe to connect to for LSP communication")]
            public string? Pipe { get; set; }

            [Option("socket", Required = false, HelpText = "The TCP port to connect to for LSP communication")]
            public int? Socket { get; set; }

            [Option("stdio", Required = false, HelpText = "If set, use stdin/stdout for LSP communication")]
            public bool Stdio { get; set; }

            [Option("wait-for-debugger", Required = false, HelpText = "If set, wait for a dotnet debugger to be attached before starting the server")]
            public bool WaitForDebugger { get; set; }
        }

        public static async Task Main(string[] args)
            => await RunWithCancellationAsync(async cancellationToken =>
            {
                var profilePath = DirHelper.GetTempPath();
                ProfileOptimization.SetProfileRoot(profilePath);
                ProfileOptimization.StartProfile("bicepserver.profile");

                var parser = new Parser(settings =>
                {
                    settings.IgnoreUnknownArguments = true;
                });

                await parser.ParseArguments<CommandLineOptions>(args)
                    .WithNotParsed((x) => Environment.Exit(1))
                    .WithParsedAsync(async options => await RunServer(options, cancellationToken));
            });

        private static async Task RunServer(CommandLineOptions options, CancellationToken cancellationToken)
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

            Server server;
            if (options.Pipe is { } pipeName)
            {
                if (pipeName.StartsWith(@"\\.\pipe\"))
                {
                    // VSCode on Windows prefixes the pipe with \\.\pipe\
                    pipeName = pipeName.Substring(@"\\.\pipe\".Length);
                }

                var clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

                await clientPipe.ConnectAsync(cancellationToken);

                server = new(
                    options => options
                        .WithInput(clientPipe)
                        .WithOutput(clientPipe));
            }
            else if (options.Socket is { } port)
            {
                var tcpClient = new TcpClient();

                await tcpClient.ConnectAsync(IPAddress.Loopback, port, cancellationToken);
                var tcpStream = tcpClient.GetStream();

                server = new(
                    options => options
                        .WithInput(tcpStream)
                        .WithOutput(tcpStream)
                        .RegisterForDisposal(tcpClient));
            }
            else
            {
                server = new(
                    options => options
                        .WithInput(Console.OpenStandardInput())
                        .WithOutput(Console.OpenStandardOutput()));
            }

            await server.RunAsync(cancellationToken);
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
    }
}
