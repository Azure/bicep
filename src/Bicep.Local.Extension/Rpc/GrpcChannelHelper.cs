// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Grpc.Core;
using Grpc.Net.Client;

namespace Bicep.Local.Extension.Rpc;

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

    public static GrpcChannel CreateChannel(string socketPath)
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

    public static async Task WaitForConnectionAsync(BicepExtension.BicepExtensionClient client, CancellationToken cancellationToken)
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
