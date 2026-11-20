// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Bicep.Local.Rpc;
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

    public static GrpcChannel CreateDomainSocketChannel(string socketPath, LocalExtensionAuthentication authentication)
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = connectionFactory.ConnectAsync,
            SslOptions = CreateTlsOptions(authentication),
        };

        return GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler,
            DisposeHttpClient = true,
        });
    }

    public static GrpcChannel CreateNamedPipeChannel(string pipeName, LocalExtensionAuthentication authentication)
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
            SslOptions = CreateTlsOptions(authentication),
        };

        return GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler,
            DisposeHttpClient = true,
        });
    }

    private static SslClientAuthenticationOptions CreateTlsOptions(LocalExtensionAuthentication authentication)
        => new()
        {
            EnabledSslProtocols = SslProtocols.None,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
            ClientCertificates = [authentication.ClientCertificate],
            // These are self-signed, per-launch identities, not certificates from the machine trust store.
            RemoteCertificateValidationCallback = (_, certificate, _, _) =>
                LocalExtensionAuthentication.MatchesCertificate(certificate, authentication.ServerCertificate),
        };

    public static async Task WaitForConnectionAsync(Rpc.BicepExtension.BicepExtensionClient client, CancellationToken cancellationToken)
    {
        RpcException? lastConnectionError = null;
        try
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
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable && !IsAuthenticationFailure(ex))
                {
                    lastConnectionError = ex;
                }
            }
        }
        catch (Exception) when (cancellationToken.IsCancellationRequested && lastConnectionError is not null)
        {
            throw new InvalidOperationException("The authenticated extension endpoint did not become ready.", lastConnectionError);
        }
    }

    internal static bool IsAuthenticationFailure(Exception exception)
        => exception is AuthenticationException ||
            (exception.InnerException is { } inner && IsAuthenticationFailure(inner));
}
