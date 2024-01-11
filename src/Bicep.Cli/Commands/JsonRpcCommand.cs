// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Rpc;
using Bicep.Core;
using Bicep.Core.Registry.Auth;
using StreamJsonRpc;

namespace Bicep.Cli.Commands;

public class JsonRpcCommand : ICommand
{
    private readonly BicepCompiler compiler;
    private readonly IOContext io;

    public JsonRpcCommand(BicepCompiler compiler, IOContext io)
    {
        this.compiler = compiler;
        this.io = io;
    }

    public async Task<int> RunAsync(JsonRpcArguments args, CancellationToken cancellationToken)
    {
        await io.Error.WriteLineAsync("The 'jsonrpc' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

        if (args.Pipe is { } pipeName)
        {
            if (pipeName.StartsWith(@"\\.\pipe\"))
            {
                pipeName = pipeName.Substring(@"\\.\pipe\".Length);
            }

            using var clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            await clientPipe.ConnectAsync(cancellationToken);

            using var rpc = new JsonRpc(CliJsonRpcServer.CreateMessageHandler(clientPipe, clientPipe));
            await RunServer(rpc, cancellationToken);
        }
        else if (args.Socket is { } port)
        {
            using var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(IPAddress.Loopback, port, cancellationToken);
            using var tcpStream = tcpClient.GetStream();

            using var rpc = new JsonRpc(CliJsonRpcServer.CreateMessageHandler(tcpStream, tcpStream));
            await RunServer(rpc, cancellationToken);
        }
        else
        {
            using var rpc = new JsonRpc(CliJsonRpcServer.CreateMessageHandler(Console.OpenStandardOutput(), Console.OpenStandardInput()));
            await RunServer(rpc, cancellationToken);
        }

        return 0;
    }

    private async Task RunServer(JsonRpc jsonRpc, CancellationToken cancellationToken)
    {
        var server = new CliJsonRpcServer(compiler);
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
}
