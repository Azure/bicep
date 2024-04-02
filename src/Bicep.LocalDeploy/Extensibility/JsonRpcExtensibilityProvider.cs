// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Extension.Rpc;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using StreamJsonRpc;
using ExtensibilityOperationRequest = Azure.Deployments.Extensibility.Messages.ExtensibilityOperationRequest;
using ExtensibilityOperationResponse = Azure.Deployments.Extensibility.Messages.ExtensibilityOperationResponse;
using RpcOperationRequest = Bicep.Extension.Rpc.ExtensibilityOperationRequest;
using RpcOperationResponse = Bicep.Extension.Rpc.ExtensibilityOperationResponse;

namespace Azure.Bicep.LocalDeploy.Extensibility;

public class JsonRpcExtensibilityProvider : LocalExtensibilityProvider
{
    private readonly NamedPipeServerStream pipeStream;
    private readonly IExtensionRpcProtocol client;
    private readonly Process process;

    private JsonRpcExtensibilityProvider(IExtensionRpcProtocol client, NamedPipeServerStream pipeStream, Process process)
    {
        this.client = client;
        this.pipeStream = pipeStream;
        this.process = process;
    }

    public static async Task<JsonRpcExtensibilityProvider> Start(Uri pathToBinary)
    {
        var pipeName = Guid.NewGuid().ToString();
        var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = pathToBinary.LocalPath,
                Arguments = $"--pipe {pipeName}",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            },
        };

        try {
            // 30s timeout for starting up the RPC connection
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => cts.Cancel();
            process.OutputDataReceived += (sender, e) => Trace.WriteLine($"{pathToBinary} stdout: {e.Data}");
            process.ErrorDataReceived += (sender, e) => Trace.WriteLine($"{pathToBinary} stderr: {e.Data}");

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();            

            await pipeStream.WaitForConnectionAsync(cts.Token);
            var client = JsonRpc.Attach<IExtensionRpcProtocol>(ExtensionRpcServer.CreateMessageHandler(pipeStream, pipeStream));

            return new(client, pipeStream, process);
        } catch (Exception ex) {
            await TerminateProcess(process, pipeStream);
            throw new InvalidOperationException($"Failed to connect to provider {pathToBinary.LocalPath}", ex);
        }
    }

    public async override Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.Delete(Convert(request), cancellationToken));
    }

    public async override Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.Get(Convert(request), cancellationToken));
    }

    public async override Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.PreviewSave(Convert(request), cancellationToken));
    }

    public async override Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.Save(Convert(request), cancellationToken));
    }

    private static RpcOperationRequest Convert(ExtensibilityOperationRequest request)
        => request.ToJson().FromJson<RpcOperationRequest>();

    private static ExtensibilityOperationResponse Convert(RpcOperationResponse request)
        => request.ToJson().FromJson<ExtensibilityOperationResponse>();

    public override async ValueTask DisposeAsync()
    {
        await TerminateProcess(process, pipeStream);
    }

    private static async Task TerminateProcess(Process process, NamedPipeServerStream pipeStream)
    {
        try {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            pipeStream.Close();
            await process.WaitForExitAsync(cts.Token);
        } finally {
            process.Kill();
        }
    }
}