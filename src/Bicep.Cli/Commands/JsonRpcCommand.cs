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
