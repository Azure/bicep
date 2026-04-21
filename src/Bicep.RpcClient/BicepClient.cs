// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Bicep.RpcClient;
using Bicep.RpcClient.JsonRpc;
using Bicep.RpcClient.Models;

namespace Bicep.RpcClient;

internal class BicepClient : IBicepClient
{
    private readonly Process cliProcess;
    private readonly JsonRpcClient jsonRpcClient;
    private readonly Task backgroundTask;
    private readonly CancellationTokenSource onDisposeCts;
    private string? cachedVersion;

    private BicepClient(Action onComplete, Process cliProcess, JsonRpcClient jsonRpcClient)
    {
        this.cliProcess = cliProcess;
        this.jsonRpcClient = jsonRpcClient;
        this.onDisposeCts = new CancellationTokenSource();
        this.backgroundTask = jsonRpcClient.Listen(onComplete: onComplete, onDisposeCts.Token);
    }

    /// <summary>
    /// Initializes the Bicep CLI by starting the process and establishing a JSON-RPC connection using a named pipe transport.
    /// </summary>
    public static Task<IBicepClient> InitializeWithNamedPipe(string bicepCliPath, TimeSpan connectionTimeout, CancellationToken cancellationToken)
    {
        var pipeName = Guid.NewGuid().ToString();
        var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, System.IO.Pipes.PipeOptions.Asynchronous);

        return InitializeWithArgs(bicepCliPath, $"jsonrpc --pipe {pipeName}", cancellationToken, async (process, combinedCancellationToken) =>
        {
            await WaitForPipeConnection(pipeStream, connectionTimeout, combinedCancellationToken).ConfigureAwait(false);

            var client = new JsonRpcClient(PipeReader.Create(pipeStream), PipeWriter.Create(pipeStream));

            return new BicepClient(pipeStream.Dispose, process, client);
        });
    }

    /// <summary>
    /// Initializes the Bicep CLI by starting the process and establishing a JSON-RPC connection using a stdin/stdout transport.
    /// </summary>
    public static Task<IBicepClient> InitializeWithStdio(string bicepCliPath, CancellationToken cancellationToken)
        => InitializeWithArgs(bicepCliPath, "jsonrpc --stdio", cancellationToken, async (process, combinedCancellationToken) =>
        {
            var reader = PipeReader.Create(process.StandardOutput.BaseStream);
            var writer = PipeWriter.Create(process.StandardInput.BaseStream);

            var client = new JsonRpcClient(reader, writer);

            return new BicepClient(onComplete: () => { }, process, client);
        });

    private static async Task<IBicepClient> InitializeWithArgs(string bicepCliPath, string arguments, CancellationToken cancellationToken, Func<Process, CancellationToken, Task<BicepClient>> initializeFunc)
    {
        var pipeName = Guid.NewGuid().ToString();
        var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, System.IO.Pipes.PipeOptions.Asynchronous);

        var process = new Process
        {
            StartInfo = new()
            {
                FileName = bicepCliPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        };

        var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => combinedCts.Cancel();

        process.Start();

        try
        {
            return await initializeFunc(process, combinedCts.Token).ConfigureAwait(false);
        }
        catch
        {
            TryKillProcess(process);
            throw;
        }
    }

    internal static async Task WaitForPipeConnection(NamedPipeServerStream pipeStream, TimeSpan connectionTimeout, CancellationToken cancellationToken)
    {
        var timeoutCts = new CancellationTokenSource(connectionTimeout);
        using var connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        try
        {
            await pipeStream.WaitForConnectionAsync(connectionCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            throw new TimeoutException($"Timed out waiting for the Bicep CLI process to connect after {connectionTimeout.TotalSeconds:0} seconds.");
        }
    }

    /// <inheritdoc/>
    public Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<CompileRequest, CompileResponse>("bicep/compile", request, cancellationToken);

    /// <inheritdoc/>
    public Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<CompileParamsRequest, CompileParamsResponse>("bicep/compileParams", request, cancellationToken);

    /// <inheritdoc/>
    public async Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken)
    {
        await EnsureMinimumVersion("0.37.1", nameof(Format), cancellationToken).ConfigureAwait(false);
        return await jsonRpcClient.SendRequest<FormatRequest, FormatResponse>("bicep/format", request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<GetDeploymentGraphRequest, GetDeploymentGraphResponse>("bicep/getDeploymentGraph", request, cancellationToken);

    /// <inheritdoc/>
    public Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<GetFileReferencesRequest, GetFileReferencesResponse>("bicep/getFileReferences", request, cancellationToken);

    /// <inheritdoc/>
    public Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<GetMetadataRequest, GetMetadataResponse>("bicep/getMetadata", request, cancellationToken);

    /// <inheritdoc/>
    public async Task<GetSnapshotResponse> GetSnapshot(GetSnapshotRequest request, CancellationToken cancellationToken)
    {
        await EnsureMinimumVersion("0.36.1", nameof(GetSnapshot), cancellationToken).ConfigureAwait(false);
        return await jsonRpcClient.SendRequest<GetSnapshotRequest, GetSnapshotResponse>("bicep/getSnapshot", request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<string> GetVersion(CancellationToken cancellationToken)
    {
        // This method is called frequently for version checks - cache the result
        if (cachedVersion is null)
        {
            var response = await jsonRpcClient.SendRequest<VersionRequest, VersionResponse>("bicep/version", new(), cancellationToken).ConfigureAwait(false);
            // No locks needed here, since replacing a reference is atomic
            cachedVersion = response.Version;
        }

        return cachedVersion;
    }

    private async Task EnsureMinimumVersion(string requiredVersion, string operationName, CancellationToken cancellationToken)
    {
        var actualVersion = await GetVersion(cancellationToken).ConfigureAwait(false);
        if (Version.Parse(actualVersion) < Version.Parse(requiredVersion))
        {
            throw new InvalidOperationException($"Operation '{operationName}' requires Bicep CLI version '{requiredVersion}' or later, whereas '{actualVersion}' is currently installed.");
        }
    }

    public void Dispose()
    {
        onDisposeCts.Cancel();
        jsonRpcClient.Dispose();
        TryKillProcess(cliProcess);
    }

    private static void TryKillProcess(Process process)
    {
        if (process.HasExited)
        {
            return;
        }

        try
        {
            process.Kill();
            // wait for 10 seconds for the process to exit
            process.WaitForExit(10000);
        }
        catch (InvalidOperationException)
        {
        }
    }
}
