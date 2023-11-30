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
    private readonly ITokenCredentialFactory credentialFactory;

    public JsonRpcCommand(BicepCompiler compiler, ITokenCredentialFactory credentialFactory)
    {
        this.compiler = compiler;
        this.credentialFactory = credentialFactory;
    }

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

            using var rpc = new JsonRpc(clientPipe, clientPipe);
            await RunServer(rpc, cancellationToken);
        }
        else if (args.Socket is { } port)
        {
            using var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(IPAddress.Loopback, port, cancellationToken);
            using var tcpStream = tcpClient.GetStream();

            using var rpc = new JsonRpc(tcpStream, tcpStream);
            await RunServer(rpc, cancellationToken);
        }
        else
        {
            using var rpc = new JsonRpc(Console.OpenStandardOutput(), Console.OpenStandardInput());
            await RunServer(rpc, cancellationToken);
        }

        return 0;
    }

    private async Task RunServer(JsonRpc jsonRpc, CancellationToken cancellationToken)
    {
        var server = new RpcServer(compiler, credentialFactory);
        jsonRpc.AddLocalRpcTarget(server);
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
