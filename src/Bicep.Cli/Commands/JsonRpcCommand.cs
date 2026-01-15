// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using Bicep.Cli.Arguments;
using Bicep.Cli.Rpc;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Utils;
using StreamJsonRpc;

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
}
