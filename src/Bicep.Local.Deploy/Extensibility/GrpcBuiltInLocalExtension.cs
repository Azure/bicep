// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using Azure.Deployments.Extensibility.Core.V2.Json;
using Bicep.Local.Extension.Rpc;
using Google.Protobuf.Collections;
using Grpc.Net.Client;
using Json.Pointer;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using ExtensibilityV2 = Azure.Deployments.Extensibility.Core.V2.Models;
using Rpc = Bicep.Local.Extension.Rpc;

namespace Bicep.Local.Deploy.Extensibility;

public class GrpcBuiltInLocalExtension : LocalExtensibilityHost
{
    private readonly BicepExtension.BicepExtensionClient client;
    private readonly Process process;

    private GrpcBuiltInLocalExtension(BicepExtension.BicepExtensionClient client, Process process, Uri binaryPath)
    {
        this.client = client;
        this.process = process;
        this.BinaryPath = binaryPath;
    }

    public static async Task<LocalExtensibilityHost> Start(Uri pathToBinary)
    {
        string processArgs;
        Func<GrpcChannel> channelBuilder;

        if (Socket.OSSupportsUnixDomainSockets)
        {
            var socketName = $"{Guid.NewGuid()}.tmp";
            var socketPath = Path.Combine(Path.GetTempPath(), socketName);

            if (File.Exists(socketPath))
            {
                File.Delete(socketPath);
            }

            processArgs = $"--socket {socketPath}";
            channelBuilder = () => GrpcChannelHelper.CreateDomainSocketChannel(socketPath);
        }
        else
        {
            var pipeName = $"{Guid.NewGuid()}.tmp";

            processArgs = $"--pipe {pipeName}";
            channelBuilder = () => GrpcChannelHelper.CreateNamedPipeChannel(pipeName);
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = pathToBinary.LocalPath,
                Arguments = processArgs,
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

            var client = new BicepExtension.BicepExtensionClient(channelBuilder());

            await GrpcChannelHelper.WaitForConnectionAsync(client, cts.Token);

            return new GrpcBuiltInLocalExtension(client, process, pathToBinary);
        }
        catch (Exception ex)
        {
            await TerminateProcess(process);
            throw new InvalidOperationException($"Failed to connect to extension {pathToBinary.LocalPath}", ex);
        }
    }

    public override Uri BinaryPath { get; }

    public override async Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ExtensibilityV2.ResourceSpecification request, CancellationToken cancellationToken)
        => Convert(await client.CreateOrUpdateAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<LocalExtensibilityOperationResponse> Delete(ExtensibilityV2.ResourceReference request, CancellationToken cancellationToken)
        => Convert(await client.DeleteAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<LocalExtensibilityOperationResponse> Get(ExtensibilityV2.ResourceReference request, CancellationToken cancellationToken)
        => Convert(await client.GetAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<LocalExtensibilityOperationResponse> Preview(ExtensibilityV2.ResourceSpecification request, CancellationToken cancellationToken)
        => Convert(await client.PreviewAsync(Convert(request), cancellationToken: cancellationToken));

    private static Rpc.ResourceReference Convert(ExtensibilityV2.ResourceReference request)
    {
        Rpc.ResourceReference output = new()
        {
            Type = request.Type,
            Identifiers = request.Identifiers.ToJsonString(),
        };

        if (request.ApiVersion is { })
        {
            output.ApiVersion = request.ApiVersion;
        }
        if (request.Config is { })
        {
            output.Config = request.Config.ToJsonString();
        }

        return output;
    }

    private static Rpc.ResourceSpecification Convert(ExtensibilityV2.ResourceSpecification request)
    {
        Rpc.ResourceSpecification output = new()
        {
            Type = request.Type,
            Properties = request.Properties.ToJsonString(),
        };

        if (request.ApiVersion is { })
        {
            output.ApiVersion = request.ApiVersion;
        }
        if (request.Config is { })
        {
            output.Config = request.Config.ToJsonString();
        }

        return output;
    }

    private static ExtensibilityV2.ErrorData Convert(Rpc.ErrorData errorData)
        => new(new ExtensibilityV2.Error(errorData.Error.Code, errorData.Error.Message, JsonPointer.Empty, Convert(errorData.Error.Details), ConvertInnerError(errorData.Error.InnerError)));

    private static ExtensibilityV2.ErrorDetail[]? Convert(RepeatedField<Rpc.ErrorDetail>? details)
        => details is not null ? details.Select(Convert).ToArray() : null;

    private static ExtensibilityV2.ErrorDetail Convert(Rpc.ErrorDetail detail)
        => new(detail.Code, detail.Message, JsonPointer.Empty);

    private static LocalExtensibilityOperationResponse Convert(Rpc.LocalExtensibilityOperationResponse response)
        => new(
            response.Resource is { } ? new(response.Resource.Type, response.Resource.ApiVersion, ToJsonObject(response.Resource.Identifiers, "Parsing response identifiers failed. Please ensure is non-null or empty and is a valid JSON object."), ToJsonObject(response.Resource.Properties, "Parsing response properties failed. Please ensure is non-null or empty and is ensure is a valid JSON object."), response.Resource.Status) : null,
            response.ErrorData is { } ? Convert(response.ErrorData) : null);

    private static JsonObject? ConvertInnerError(string innerError)
        => string.IsNullOrEmpty(innerError) ? null : ToJsonObject(innerError, "Parsing innerError failed. Please ensure is non-null or empty and is a valid JSON object.");

    private static JsonObject ToJsonObject(string json, string errorMessage)
        => JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);

    public override async ValueTask DisposeAsync()
    {
        await TerminateProcess(process);
    }

    private static async Task TerminateProcess(Process process)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        process.Kill();
        await process.WaitForExitAsync(cts.Token);
    }
}
