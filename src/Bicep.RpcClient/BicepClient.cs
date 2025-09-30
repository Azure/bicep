// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Bicep.RpcClient.JsonRpc;
using Bicep.RpcClient.Models;

namespace Bicep.RpcClient;

internal class BicepClient : IBicepClient
{
    private readonly NamedPipeServerStream pipeStream;
    private readonly Process cliProcess;
    private readonly JsonRpcClient jsonRpcClient;
    private readonly Task backgroundTask;
    private readonly CancellationTokenSource cts;

    private BicepClient(NamedPipeServerStream pipeStream, Process cliProcess, JsonRpcClient jsonRpcClient, CancellationTokenSource cts)
    {
        this.pipeStream = pipeStream;
        this.cliProcess = cliProcess;
        this.jsonRpcClient = jsonRpcClient;
        this.backgroundTask = CreateBackgroundTask(pipeStream, cliProcess, jsonRpcClient, cts.Token);
        this.cts = cts;
    }

    /// <summary>
    /// Initializes the Bicep CLI by starting the process and establishing a JSON-RPC connection.
    /// </summary>
    public static async Task<IBicepClient> Initialize(string bicepCliPath, CancellationToken cancellationToken)
    {
        if (!File.Exists(bicepCliPath))
        {
            throw new FileNotFoundException($"The specified Bicep CLI path does not exist: {bicepCliPath}");
        }

        var pipeName = Guid.NewGuid().ToString();
        var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

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

        await pipeStream.WaitForConnectionAsync(cancellationToken);
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var client = new JsonRpcClient(pipeStream, pipeStream);

        return new BicepClient(pipeStream, cliProcess, client, cts);
    }

    private static Task CreateBackgroundTask(NamedPipeServerStream pipeStream, Process cliProcess, JsonRpcClient jsonRpcClient, CancellationToken cancellationToken)
        => Task.Run(async () =>
        {
            try
            {
                await jsonRpcClient.ReadLoop(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when disposing
            }
            finally
            {
                pipeStream.Dispose();
                cliProcess.Dispose();
            }
        }, cancellationToken);

    /// <inheritdoc/>
    public Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<CompileRequest, CompileResponse>("bicep/compile", request, cancellationToken);

    /// <inheritdoc/>
    public Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken)
        => jsonRpcClient.SendRequest<CompileParamsRequest, CompileParamsResponse>("bicep/compileParams", request, cancellationToken);

    /// <inheritdoc/>
    public async Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken)
    {
        await EnsureMinimumVersion("0.37.1", nameof(Format), cancellationToken);
        return await jsonRpcClient.SendRequest<FormatRequest, FormatResponse>("bicep/format", request, cancellationToken);
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
        await EnsureMinimumVersion("0.36.1", nameof(GetSnapshot), cancellationToken);
        return await jsonRpcClient.SendRequest<GetSnapshotRequest, GetSnapshotResponse>("bicep/getSnapshot", request, cancellationToken);        
    }

    /// <inheritdoc/>
    public async Task<string> GetVersion(CancellationToken cancellationToken)
        => (await jsonRpcClient.SendRequest<VersionRequest, VersionResponse>("bicep/version", new(), cancellationToken)).Version;

    private async Task EnsureMinimumVersion(string requiredVersion, string operationName, CancellationToken cancellationToken)
    {
        var actualVersion = await GetVersion(cancellationToken);
        if (Version.Parse(actualVersion) < Version.Parse(requiredVersion))
        {
            throw new InvalidOperationException($"Operation '{operationName}' requires Bicep CLI version '{requiredVersion}' or later, whereas '{actualVersion}' is currently installed.");
        }
    }

    public void Dispose()
    {
        cts.Cancel();
        if (!cliProcess.HasExited)
        {
            cliProcess.WaitForExit();
        }
    }
}
