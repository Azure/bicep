// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using Bicep.Local.Rpc;
using Grpc.Core;
using Grpc.Net.Client;

namespace Bicep.ExtensionHost.Extensions;

/// <summary>
/// Represents a running extension with its gRPC client.
/// </summary>
public class ExtensionInstance : IAsyncDisposable
{
    private readonly Process _process;
    private readonly GrpcChannel _channel;
    private readonly string _extensionName;
    private readonly string _binaryPath;
    private readonly ILogger _logger;

    public BicepExtension.BicepExtensionClient Client { get; }
    public string Name => _extensionName;
    public bool IsRunning => !_process.HasExited;

    private ExtensionInstance(
        string extensionName,
        string binaryPath,
        Process process,
        GrpcChannel channel,
        BicepExtension.BicepExtensionClient client,
        ILogger logger)
    {
        _extensionName = extensionName;
        _binaryPath = binaryPath;
        _process = process;
        _channel = channel;
        Client = client;
        _logger = logger;
    }

    /// <summary>
    /// Starts an extension process and establishes a gRPC connection.
    /// </summary>
    public static async Task<ExtensionInstance> StartAsync(
        string extensionName,
        string binaryPath,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting extension '{Name}' from {Path}", extensionName, binaryPath);

        string processArgs;
        Func<GrpcChannel> channelBuilder;

        // While modern Windows technically supports UDS, many libraries (like Rust's Tokio) do not
        if (Socket.OSSupportsUnixDomainSockets && !OperatingSystem.IsWindows())
        {
            var socketName = $"bicep-ext-{extensionName}-{Guid.NewGuid()}.sock";
            var socketPath = Path.Combine(Path.GetTempPath(), socketName);

            if (File.Exists(socketPath))
            {
                File.Delete(socketPath);
            }

            processArgs = $"--socket {socketPath}";
            channelBuilder = () => CreateDomainSocketChannel(socketPath);
        }
        else
        {
            var pipeName = $"bicep-ext-{extensionName}-{Guid.NewGuid()}";

            processArgs = $"--pipe {pipeName}";
            channelBuilder = () => CreateNamedPipeChannel(pipeName);
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = binaryPath,
                Arguments = processArgs,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            },
        };

        GrpcChannel? channel = null;

        try
        {
            // 30s timeout for starting up the RPC connection
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                logger.LogWarning("Extension '{Name}' process exited unexpectedly with code {ExitCode}",
                    extensionName, process.ExitCode);
                cts.Cancel();
            };
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    logger.LogDebug("[{Name}] stdout: {Data}", extensionName, e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    logger.LogDebug("[{Name}] stderr: {Data}", extensionName, e.Data);
                }
            };

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            channel = channelBuilder();
            var client = new BicepExtension.BicepExtensionClient(channel);

            await WaitForConnectionAsync(client, cts.Token);

            logger.LogInformation("Extension '{Name}' started successfully (PID: {Pid})",
                extensionName, process.Id);

            return new ExtensionInstance(extensionName, binaryPath, process, channel, client, logger);
        }
        catch (Exception ex)
        {
            await TerminateProcessAsync(extensionName, process, channel, logger);
            throw new InvalidOperationException($"Failed to start extension '{extensionName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Pings the extension to check if it's healthy.
    /// </summary>
    public async Task<bool> PingAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_process.HasExited)
            {
                return false;
            }

            await Client.PingAsync(new Empty(), cancellationToken: cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ping failed for extension '{Name}'", _extensionName);
            return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await TerminateProcessAsync(_extensionName, _process, _channel, _logger);
    }

    private static GrpcChannel CreateDomainSocketChannel(string socketPath)
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = async (context, cancellationToken) =>
            {
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                try
                {
                    await socket.ConnectAsync(udsEndPoint, cancellationToken);
                    return new NetworkStream(socket, ownsSocket: true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        };

        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler
        });
    }

    private static GrpcChannel CreateNamedPipeChannel(string pipeName)
    {
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = async (context, cancellationToken) =>
            {
                var clientStream = new NamedPipeClientStream(
                    serverName: ".",
                    pipeName: pipeName,
                    direction: PipeDirection.InOut,
                    options: PipeOptions.WriteThrough | PipeOptions.Asynchronous);

                try
                {
                    await clientStream.ConnectAsync(cancellationToken);
                    return clientStream;
                }
                catch
                {
                    await clientStream.DisposeAsync();
                    throw;
                }
            },
        };

        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = socketsHttpHandler
        });
    }

    private static async Task WaitForConnectionAsync(BicepExtension.BicepExtensionClient client, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(50, cancellationToken);
                await client.PingAsync(new Empty(), cancellationToken: cancellationToken);
                return;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                // Server not ready yet, keep trying
            }
        }
    }

    private static async Task TerminateProcessAsync(string extensionName, Process process, GrpcChannel? channel, ILogger logger)
    {
        try
        {
            if (!process.HasExited)
            {
                logger.LogInformation("Terminating extension '{Name}' (PID: {Pid})", extensionName, process.Id);
                process.Kill();

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await process.WaitForExitAsync(cts.Token);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to terminate extension '{Name}'", extensionName);
        }
        finally
        {
            channel?.Dispose();
            process.Dispose();
        }
    }
}
