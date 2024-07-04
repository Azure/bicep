// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text.Json.Nodes;
using Bicep.Local.Extension.Rpc;
using Google.Protobuf.Collections;
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

    private GrpcBuiltInLocalExtension(BicepExtension.BicepExtensionClient client, Process process)
    {
        this.client = client;
        this.process = process;
    }

    public static async Task<LocalExtensibilityHost> Start(Uri pathToBinary)
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

            return new GrpcBuiltInLocalExtension(client, process);
        }
        catch (Exception ex)
        {
            await TerminateProcess(process);
            throw new InvalidOperationException($"Failed to connect to provider {pathToBinary.LocalPath}", ex);
        }
    }

    public override async Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ExtensibilityV2.ResourceSpecification request, CancellationToken cancellationToken)
        => Convert(await client.CreateOrUpdateAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<LocalExtensibilityOperationResponse> Delete(ExtensibilityV2.ResourceReference request, CancellationToken cancellationToken)
        => Convert(await client.DeleteAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<LocalExtensibilityOperationResponse> Get(ExtensibilityV2.ResourceReference request, CancellationToken cancellationToken)
        => Convert(await client.GetAsync(Convert(request), cancellationToken: cancellationToken));

    public override async Task<LocalExtensibilityOperationResponse> Preview(ExtensibilityV2.ResourceSpecification request, CancellationToken cancellationToken)
        => Convert(await client.PreviewAsync(Convert(request), cancellationToken: cancellationToken));

    private static Rpc.ResourceReference Convert(ExtensibilityV2.ResourceReference request)
        => new()
        {
            ApiVersion = request.ApiVersion,
            Config = request.Config.ToJson(),
            Identifiers = request.Identifiers.ToJson(),
            Type = request.Type
        };

    private static Rpc.ResourceSpecification Convert(ExtensibilityV2.ResourceSpecification request)
        => new()
        {
            ApiVersion = request.ApiVersion,
            Config = request.Config.ToJson(),
            Properties = request.Properties.ToJson(),
            Type = request.Type
        };

    private static ExtensibilityV2.ErrorData Convert(Rpc.ErrorData errorData)
        => new(new ExtensibilityV2.Error(errorData.Error.Code, errorData.Error.Message, JsonPointer.Empty, Convert(errorData.Error.Details), errorData.Error.InnerError is null ? null : JsonObject.Parse(errorData.Error.InnerError)?.AsObject() ?? throw new UnreachableException()));

    private static ExtensibilityV2.ErrorDetail[]? Convert(RepeatedField<Rpc.ErrorDetail>? details)
        => details is not null ? details.Select(Convert).ToArray() : null;

    private static ExtensibilityV2.ErrorDetail Convert(Rpc.ErrorDetail detail)
        => new(detail.Code, detail.Message, JsonPointer.Empty);

    private static LocalExtensibilityOperationResponse Convert(Rpc.LocalExtensibilityOperationResponse response)
        => new(
            new ExtensibilityV2.Resource(response.Resource.Type, response.Resource.ApiVersion, ToJsonObject(response.Resource.Identifiers, "Parsing response identifiers failed. Please ensure is non-null or empty and is a valid JSON object."), ToJsonObject(response.Resource.Properties, "Parsing response properties failed. Please ensure is non-null or empty and is ensure is a valid JSON object."), response.Resource.Status),
            Convert(response.ErrorData));

    private static JsonObject ToJsonObject(string json, string errorMessage)
        => JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);

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
