// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Rpc;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using StreamJsonRpc;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class JsonRpcCommand(
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver,
    IEnvironment environment) : ICommand
{
    public async Task<int> RunAsync(JsonRpcArguments args, CancellationToken cancellationToken)
    {
        if (args.Pipe is { } pipeName)
        {
            if (pipeName.StartsWith(@"\\.\pipe\"))
            {
                pipeName = pipeName.Substring(@"\\.\pipe\".Length);
            }

            using var clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            await clientPipe.ConnectAsync(cancellationToken);
            await RunServer(clientPipe, clientPipe, cancellationToken);
        }
        else if (args.Socket is { } port)
        {
            using var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(IPAddress.Loopback, port, cancellationToken);
            using var tcpStream = tcpClient.GetStream();
            await RunServer(tcpStream, tcpStream, cancellationToken);
        }
        else
        {
            await RunServer(Console.OpenStandardOutput(), Console.OpenStandardInput(), cancellationToken);
        }

        return 0;
    }

    private async Task RunServer(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
    {
        using var jsonRpc = new JsonRpc(CliJsonRpcServer.CreateMessageHandler(inputStream, outputStream));
        if (FeatureProvider.TracingEnabled)
        {
            jsonRpc.TraceSource = new TraceSource("JsonRpc", SourceLevels.Verbose);
            jsonRpc.TraceSource.Listeners.AddRange(Trace.Listeners);
        }

        var server = new CliJsonRpcServer(compiler, inputOutputArgumentsResolver, environment);
        jsonRpc.AddLocalRpcTarget<ICliJsonRpcProtocol>(server, null);

        jsonRpc.StartListening();

        await Task.WhenAny(jsonRpc.Completion, WaitForCancellation(cancellationToken));
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

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.JsonRpc, "Starts the Bicep CLI listening for JSONRPC messages, for programatically interacting with Bicep.");

        var pipeOption = new System.CommandLine.Option<string?>(Option.Pipe)
        {
            Description = "Bicep CLI will connect to the supplied named pipe as a client, and start listening for JSONRPC requests.",
        };
        var socketOption = new System.CommandLine.Option<int?>(Option.Socket)
        {
            Description = "Bicep CLI will connect to the supplied TCP port on the loopback interface as a client, and start listening for JSONRPC requests.",
        };
        var stdioOption = new System.CommandLine.Option<bool>(Option.Stdio)
        {
            Description = "Bicep CLI will use stdin/stdout for JSONRPC requests.",
        };

        command.Add(pipeOption);
        command.Add(socketOption);
        command.Add(stdioOption);

        command.Validators.Add((System.CommandLine.Parsing.CommandResult result) =>
        {
            var hasPipe = result.GetResult(pipeOption) is { Implicit: false };
            var hasSocket = result.GetResult(socketOption) is { Implicit: false };
            var hasStdio = result.GetResult(stdioOption) is { Implicit: false };

            if ((hasPipe ? 1 : 0) + (hasSocket ? 1 : 0) + (hasStdio ? 1 : 0) > 1)
            {
                result.AddError("Only one of --pipe, --socket, or --stdio may be specified.");
            }
        });

        command.SetAction(async (result, cancellationToken) =>
        {
            JsonRpcArguments args = new(
                Pipe: result.GetValue(pipeOption),
                Socket: result.GetValue(socketOption),
                Stdio: result.GetValue(stdioOption));

            return await context.GetCommand<JsonRpcCommand>().RunAsync(args, cancellationToken);
        });

        return command;
    }
}
