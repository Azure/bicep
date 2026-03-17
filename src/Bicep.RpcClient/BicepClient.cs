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
    private readonly CancellationTokenSource cts;
    private string? cachedVersion;

    private BicepClient(Action onComplete, Process cliProcess, JsonRpcClient jsonRpcClient, CancellationTokenSource cts)
    {
        this.cliProcess = cliProcess;
        this.jsonRpcClient = jsonRpcClient;
        this.backgroundTask = jsonRpcClient.Listen(onComplete: onComplete, cts.Token);
        this.cts = cts;
    }

    /// <summary>
    /// Initializes the Bicep CLI by starting the process and establishing a JSON-RPC connection.
    /// </summary>
    public static Task<IBicepClient> Initialize(string bicepCliPath, TimeSpan connectionTimeout, CancellationToken cancellationToken)
        => Initialize(bicepCliPath, useStdio: false, connectionTimeout, cancellationToken);

    /// <summary>
    /// Initializes the Bicep CLI by starting the process and establishing a JSON-RPC connection.
    /// </summary>
    public static async Task<IBicepClient> Initialize(string bicepCliPath, bool useStdio, TimeSpan connectionTimeout, CancellationToken cancellationToken)
    {
        if (!File.Exists(bicepCliPath))
        {
            throw new FileNotFoundException($"The specified Bicep CLI path does not exist: '{bicepCliPath}'.");
        }

        if (useStdio)
        {
            return InitializeWithStdio(bicepCliPath, cancellationToken);
        }

        var pipeName = Guid.NewGuid().ToString();
        var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, System.IO.Pipes.PipeOptions.Asynchronous);

        var psi = new ProcessStartInfo
        {
            FileName = bicepCliPath,
            Arguments = $"jsonrpc --pipe {pipeName}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        var cliProcess = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start Bicep CLI process");

        try
        {
            await WaitForPipeConnection(pipeStream, connectionTimeout, cancellationToken).ConfigureAwait(false);

            var clientCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var client = new JsonRpcClient(PipeReader.Create(pipeStream), PipeWriter.Create(pipeStream));

            return new BicepClient(pipeStream.Dispose, cliProcess, client, clientCts);
        }
        catch
        {
            cliProcess.Kill();
            throw;
        }
    }

    private static IBicepClient InitializeWithStdio(string bicepCliPath, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = bicepCliPath,
            Arguments = "jsonrpc --stdio",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        var cliProcess = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start Bicep CLI process");

        var clientCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var reader = PipeReader.Create(cliProcess.StandardOutput.BaseStream);
        var writer = PipeWriter.Create(cliProcess.StandardInput.BaseStream);
        var client = new JsonRpcClient(reader, writer);

        return new BicepClient(onComplete: () => { }, cliProcess, client, clientCts);
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
        cts.Cancel();
        jsonRpcClient.Dispose();
        try
        {
            cliProcess.Kill();
            // wait for 10 seconds for the process to exit
            cliProcess.WaitForExit(10000);
        }
        catch (InvalidOperationException)
        {
        }
    }
}
