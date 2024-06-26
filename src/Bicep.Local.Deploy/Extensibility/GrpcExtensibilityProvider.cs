// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
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
using ExtensibilityV2 = Azure.Deployments.Engine.Host.Azure.ExtensibilityV2.Contract;
using Rpc = Bicep.Local.Extension.Rpc;
using Google.Protobuf.Collections;

namespace Bicep.Local.Deploy.Extensibility;

public class GrpcExtensibilityProvider : LocalExtensibilityProviderV2
{
    private readonly BicepExtension.BicepExtensionClient client;
    private readonly Process process;
    private static readonly ConcurrentDictionary<Uri, LocalExtensibilityProviderV2> startedProviders = new();

    private GrpcExtensibilityProvider(BicepExtension.BicepExtensionClient client, Process process)
    {
        this.client = client;
        this.process = process;
    }

    public static async Task<LocalExtensibilityProviderV2> Start(Uri pathToBinary)
    {
        // TODO: Acquire lock?
        if (!startedProviders.TryGetValue(pathToBinary, out var localExtensibilityProvider))
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

                localExtensibilityProvider = new GrpcExtensibilityProvider(client, process);
                startedProviders[pathToBinary] = localExtensibilityProvider;
            }
            catch (Exception ex)
            {
                await TerminateProcess(process);
                throw new InvalidOperationException($"Failed to connect to provider {pathToBinary.LocalPath}", ex);
            }
        }

        return localExtensibilityProvider;
    }

    public override async Task<ExtensibilityV2.Models.ResourceResponseBody> CreateOrUpdateResourceAsync(ExtensibilityV2.Models.ResourceRequestBody request, CancellationToken cancellationToken)
        => Convert(await client.CreateOrUpdateAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<ExtensibilityV2.Models.ResourceResponseBody> DeleteResourceAsync(ExtensibilityV2.Models.ResourceReferenceRequestBody request, CancellationToken cancellationToken)
        => Convert(await client.DeleteAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<ExtensibilityV2.Models.ResourceResponseBody> GetResourceAsync(ExtensibilityV2.Models.ResourceReferenceRequestBody request, CancellationToken cancellationToken)
        => Convert(await client.GetAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<ExtensibilityV2.Models.ResourceResponseBody> PreviewResourceCreateOrUpdateAsync(ExtensibilityV2.Models.ResourceRequestBody request, CancellationToken cancellationToken)
        => Convert(await client.PreviewAsync(Convert(request), cancellationToken: cancellationToken));

    private static Rpc.ResourceReferenceRequestBody Convert(ExtensibilityV2.Models.ResourceReferenceRequestBody request)
        => new()
        {
            ApiVersion = request.ApiVersion,
            Config = request.Config.ToJson(),
            Identifiers = request.Identifiers.ToJson(),
            Type = request.Type
        };

    private static Rpc.ResourceRequestBody Convert(ExtensibilityV2.Models.ResourceRequestBody request)
        => new()
        {
            ApiVersion = request.ApiVersion,
            Config = request.Config.ToJson(),
            Properties = request.Properties.ToJson(),
            Type = request.Type
        };

    private static ExtensibilityV2.Models.ErrorPayload Convert(Rpc.ErrorPayload error)
        => new("", "", "", Convert(error.Details), error.InnerError != null ? JObject.Parse(error.InnerError): null);

    private static ExtensibilityV2.Models.ErrorDetail[]? Convert(RepeatedField<Rpc.ErrorDetail>? details)
    {
        if (details is not { })
        {
            return default;
        }

        var errorDetails = new ExtensibilityV2.Models.ErrorDetail[details.Count];

        for (var i = 0; i < details.Count; i++)
        {
            if (details[i] is { })
            {
                errorDetails[i] = Convert(details[i]);
            }
        }
        return errorDetails;
    }

    private static ExtensibilityV2.Models.ErrorDetail Convert(Rpc.ErrorDetail detail)
        => new(detail.Code, detail.Message, detail.Target);

    private static ExtensibilityV2.Models.ResourceResponseBody Convert(Rpc.ResourceResponse response)
    {
        switch (response.ResultCase)
        {
            case ResourceResponse.ResultOneofCase.Response:
                return new ExtensibilityV2.Models.ResourceResponseBody(error: null, response.Response.Identifiers, response.Response.Type, response.Response.Status, response.Response.Properties.ToJToken());
            case ResourceResponse.ResultOneofCase.Error:
                return new ExtensibilityV2.Models.ResourceResponseBody(Convert(response.Error), null, null, null, null);
            default:
                throw new ArgumentException("Unexpected result.");
        }
    }

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
