// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Extension.Rpc;
using Grpc.Net.Client;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using ExtensibilityOperationRequest = Azure.Deployments.Extensibility.Messages.ExtensibilityOperationRequest;
using ExtensibilityOperationResponse = Azure.Deployments.Extensibility.Messages.ExtensibilityOperationResponse;
using RpcOperationRequest = Bicep.Extension.Rpc.ExtensibilityOperationRequest;
using RpcOperationResponse = Bicep.Extension.Rpc.ExtensibilityOperationResponse;

namespace Azure.Bicep.LocalDeploy.Extensibility;

public class GrpcExtensibilityProvider : LocalExtensibilityProvider
{
    private readonly BicepExtension.BicepExtensionClient client;
    private readonly Process process;

    private GrpcExtensibilityProvider(BicepExtension.BicepExtensionClient client, Process process)
    {
        this.client = client;
        this.process = process;
    }

    public static async Task<GrpcExtensibilityProvider> Start(Uri pathToBinary)
    {
        var socketName = $"{Guid.NewGuid()}.tmp";
        var socketPath = Path.Combine(Path.GetTempPath(), socketName);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = pathToBinary.LocalPath,
                Arguments = $"--socket {socketPath}",
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

            var channel = GrpcChannelHelper.CreateChannel(socketPath);
            var client = new BicepExtension.BicepExtensionClient(channel);

            return new(client, process);
        } catch (Exception ex) {
            await TerminateProcess(process);
            throw new InvalidOperationException($"Failed to connect to provider {pathToBinary.LocalPath}", ex);
        }
    }

    public async override Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.DeleteAsync(Convert(request), cancellationToken: cancellationToken));
    }

    public async override Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.GetAsync(Convert(request), cancellationToken: cancellationToken));
    }

    public async override Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.PreviewSaveAsync(Convert(request), cancellationToken: cancellationToken));
    }

    public async override Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.SaveAsync(Convert(request), cancellationToken: cancellationToken));
    }

    private static RpcOperationRequest Convert(ExtensibilityOperationRequest request)
        => request.ToJson().FromJson<RpcOperationRequest>();

    private static ExtensibilityOperationResponse Convert(RpcOperationResponse request)
        => request.ToJson().FromJson<ExtensibilityOperationResponse>();

    public override async ValueTask DisposeAsync()
    {
        await TerminateProcess(process);
    }

    private static async Task TerminateProcess(Process process)
    {
        try {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            await process.WaitForExitAsync(cts.Token);
        } finally {
            process.Kill();
        }
    }
}