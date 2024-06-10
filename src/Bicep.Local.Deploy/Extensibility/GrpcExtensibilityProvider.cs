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
using Bicep.Local.Extension.Rpc;
using Grpc.Net.Client;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using Data = Azure.Deployments.Extensibility.Data;
using Messages = Azure.Deployments.Extensibility.Messages;
using Rpc = Bicep.Local.Extension.Rpc;

namespace Bicep.Local.Deploy.Extensibility;

public class GrpcExtensibilityProvider : LocalExtensibilityProvider
{
    private readonly BicepExtension.BicepExtensionClient client;
    private readonly Process process;

    private GrpcExtensibilityProvider(BicepExtension.BicepExtensionClient client, Process process)
    {
        this.client = client;
        this.process = process;
    }

    public static async Task<LocalExtensibilityProvider> Start(Uri pathToBinary)
    {
        var socketName = $"{Guid.NewGuid()}.tmp";
        var socketPath = Path.Combine(Path.GetTempPath(), socketName);

        if (File.Exists(socketPath))
        {
            File.Delete(socketPath);
        }

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

        try
        {
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

            await GrpcChannelHelper.WaitForConnectionAsync(client, cts.Token);

            return new GrpcExtensibilityProvider(client, process);
        }
        catch (Exception ex)
        {
            await TerminateProcess(process);
            throw new InvalidOperationException($"Failed to connect to provider {pathToBinary.LocalPath}", ex);
        }
    }

    public async override Task<Messages.ExtensibilityOperationResponse> Delete(Messages.ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.DeleteAsync(Convert(request), cancellationToken: cancellationToken));
    }

    public async override Task<Messages.ExtensibilityOperationResponse> Get(Messages.ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.GetAsync(Convert(request), cancellationToken: cancellationToken));
    }

    public async override Task<Messages.ExtensibilityOperationResponse> PreviewSave(Messages.ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.PreviewSaveAsync(Convert(request), cancellationToken: cancellationToken));
    }

    public async override Task<Messages.ExtensibilityOperationResponse> Save(Messages.ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        return Convert(await client.SaveAsync(Convert(request), cancellationToken: cancellationToken));
    }

    private static Rpc.ExtensibilityOperationRequest Convert(Messages.ExtensibilityOperationRequest request)
        => new()
        {
            Import = new Rpc.ExtensibleImportData
            {
                Provider = request.Import.Provider,
                Version = request.Import.Version,
                Config = request.Import.Config?.ToJson(),
            },
            Resource = new Rpc.ExtensibleResourceData
            {
                Type = request.Resource.Type,
                Properties = request.Resource.Properties?.ToJson(),
            },
        };

    private static Messages.ExtensibilityOperationResponse Convert(Rpc.ExtensibilityOperationResponse response)
        => new(
            response.Resource is { } resource ? new(resource.Type, resource.Properties?.FromJson<JObject>()) : null,
            response.ResourceMetadata is { } metadata ? new(metadata.ReadOnlyProperties.ToArray(), metadata.ImmutableProperties.ToArray(), metadata.DynamicProperties.ToArray()) : null,
            response.Errors is { } errors ? errors.Select(error => new Data.ExtensibilityError(error.Code, error.Message, error.Target)).ToArray() : null);

    public override async ValueTask DisposeAsync()
    {
        await TerminateProcess(process);
    }

    private static async Task TerminateProcess(Process process)
    {
        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            await process.WaitForExitAsync(cts.Token);
        }
        finally
        {
            process.Kill();
        }
    }
}
