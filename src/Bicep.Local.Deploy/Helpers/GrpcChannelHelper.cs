// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Grpc.Core;
using Grpc.Net.Client;

namespace Bicep.Local.Deploy.Helpers;

public static class GrpcChannelHelper
{
    public class UnixDomainSocketsConnectionFactory
    {
        private readonly EndPoint endPoint;

        public UnixDomainSocketsConnectionFactory(EndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
            CancellationToken cancellationToken = default)
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            try
            {
                await socket.ConnectAsync(this.endPoint, cancellationToken).ConfigureAwait(false);
                return new NetworkStream(socket, true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }
    }

    public static GrpcChannel CreateDomainSocketChannel(string socketPath)
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = connectionFactory.ConnectAsync
        };

        // The URL is not used, but it must be a valid URI.
        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler
        });
    }

    public static GrpcChannel CreateNamedPipeChannel(string pipeName)
    {
        static async ValueTask<Stream> connectPipe(string pipeName, CancellationToken cancellationToken)
        {
            var clientStream = new NamedPipeClientStream(
                serverName: ".",
                pipeName: pipeName,
                direction: PipeDirection.InOut,
                options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
                impersonationLevel: TokenImpersonationLevel.Anonymous);

            try
            {
                await clientStream.ConnectAsync(cancellationToken).ConfigureAwait(false);
                return clientStream;
            }
            catch
            {
                await clientStream.DisposeAsync();
                throw;
            }
        }

        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = (context, cancellationToken) => connectPipe(pipeName, cancellationToken),
        };

        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler
        });
    }

    public static async Task WaitForConnectionAsync(Rpc.BicepExtension.BicepExtensionClient client, CancellationToken cancellationToken)
    {
        var connected = false;
        while (!connected)
        {
            try
            {
                await Task.Delay(50, cancellationToken);
                await client.PingAsync(new(), cancellationToken: cancellationToken);
                connected = true;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                // ignore
            }
        }
    }
}
