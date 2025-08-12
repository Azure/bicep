// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Extensibility.Core.V2.Json;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Deploy.Types;
using Grpc.Net.Client;
using Json.Pointer;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using ExtensibilityV2 = Azure.Deployments.Extensibility.Core.V2.Models;

namespace Bicep.Local.Deploy.Extensibility;

internal class GrpcLocalExtension(
    Rpc.BicepExtension.BicepExtensionClient client,
    Process process,
    GrpcChannel channel,
    IOUri binaryUri) : ILocalExtension
{
    private static void WriteTrace(IOUri binaryUri, Func<string> getMessage)
        => Trace.WriteLine($"[{binaryUri}] {getMessage()}");

    public static async Task<ILocalExtension> Start(IOUri binaryUri)
    {
        string processArgs;
        Func<GrpcChannel> channelBuilder;
        GrpcChannel? channel = null;

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
                FileName = binaryUri.GetLocalFilePath(),
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
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    WriteTrace(binaryUri, () => $"stdout: {e.Data}");
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    WriteTrace(binaryUri, () => $"stderr: {e.Data}");
                }
            };

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            channel = channelBuilder();
            var client = new Rpc.BicepExtension.BicepExtensionClient(channel);

            await GrpcChannelHelper.WaitForConnectionAsync(client, cts.Token);

            return new GrpcLocalExtension(client, process, channel, binaryUri);
        }
        catch (Exception ex)
        {
            await TerminateProcess(binaryUri, process, channel);
            throw new InvalidOperationException($"Failed to connect to extension {binaryUri}", ex);
        }
    }

    public async Task<LocalExtensionOperationResponse> CreateOrUpdate(ExtensibilityV2.ResourceSpecification request, CancellationToken cancellationToken)
    {
        WriteTrace(binaryUri, () => $"{nameof(CreateOrUpdate)} gRPC request: {JsonSerializer.Serialize(request, JsonDefaults.SerializerContext.ResourceSpecification)}");

        var response = Convert(await client.CreateOrUpdateAsync(Convert(request), cancellationToken: cancellationToken));

        WriteTrace(binaryUri, () => $"{nameof(CreateOrUpdate)} gRPC response: {JsonSerializer.Serialize(response, LocalExtensionOperationResponseJsonDefaults.SerializerContext.LocalExtensionOperationResponse)}");

        return response;
    }

    public async Task<LocalExtensionOperationResponse> Delete(ExtensibilityV2.ResourceReference request, CancellationToken cancellationToken)
    {
        WriteTrace(binaryUri, () => $"{nameof(Delete)} gRPC request: {JsonSerializer.Serialize(request, JsonDefaults.SerializerContext.ResourceReference)}");

        var response = Convert(await client.DeleteAsync(Convert(request), cancellationToken: cancellationToken));

        WriteTrace(binaryUri, () => $"{nameof(Delete)} gRPC response: {JsonSerializer.Serialize(response, LocalExtensionOperationResponseJsonDefaults.SerializerContext.LocalExtensionOperationResponse)}");

        return response;
    }

    public async Task<LocalExtensionOperationResponse> Get(ExtensibilityV2.ResourceReference request, CancellationToken cancellationToken)
    {
        WriteTrace(binaryUri, () => $"{nameof(Get)} gRPC request: {JsonSerializer.Serialize(request, JsonDefaults.SerializerContext.ResourceReference)}");

        var response = Convert(await client.GetAsync(Convert(request), cancellationToken: cancellationToken));

        WriteTrace(binaryUri, () => $"{nameof(Get)} gRPC response: {JsonSerializer.Serialize(response, LocalExtensionOperationResponseJsonDefaults.SerializerContext.LocalExtensionOperationResponse)}");

        return response;
    }

    public async Task<LocalExtensionOperationResponse> Preview(ExtensibilityV2.ResourceSpecification request, CancellationToken cancellationToken)
    {
        WriteTrace(binaryUri, () => $"{nameof(Preview)} gRPC request: {JsonSerializer.Serialize(request, JsonDefaults.SerializerContext.ResourceSpecification)}");

        var response = Convert(await client.PreviewAsync(Convert(request), cancellationToken: cancellationToken));

        WriteTrace(binaryUri, () => $"{nameof(Preview)} gRPC response: {JsonSerializer.Serialize(response, LocalExtensionOperationResponseJsonDefaults.SerializerContext.LocalExtensionOperationResponse)}");

        return response;
    }

    public async Task<TypeFiles> GetTypeFiles(CancellationToken cancellationToken)
    {
        WriteTrace(binaryUri, () => $"{nameof(GetTypeFiles)} gRPC request: <empty>");

        var response = await client.GetTypeFilesAsync(new(), cancellationToken: cancellationToken);

        WriteTrace(binaryUri, () => $"{nameof(GetTypeFiles)} gRPC response: {response.ToJson()}");

        return new(
            response.IndexFile,
            response.TypeFiles.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

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

    private static ExtensibilityV2.ErrorDetail[]? Convert(IEnumerable<Rpc.ErrorDetail>? details)
        => details is not null ? details.Select(Convert).ToArray() : null;

    private static ExtensibilityV2.ErrorDetail Convert(Rpc.ErrorDetail detail)
        => new(detail.Code, detail.Message, JsonPointer.Empty);

    private static LocalExtensionOperationResponse Convert(Rpc.LocalExtensibilityOperationResponse response)
        => new(
            response.Resource is { } ? new(response.Resource.Type, response.Resource.ApiVersion, ToJsonObject(response.Resource.Identifiers, "Parsing response identifiers failed. Please ensure is non-null or empty and is a valid JSON object."), ToJsonObject(response.Resource.Properties, "Parsing response properties failed. Please ensure is non-null or empty and is ensure is a valid JSON object."), response.Resource.Status) : null,
            response.ErrorData is { } ? Convert(response.ErrorData) : null);

    private static JsonObject? ConvertInnerError(string innerError)
        => string.IsNullOrEmpty(innerError) ? null : ToJsonObject(innerError, "Parsing innerError failed. Please ensure is non-null or empty and is a valid JSON object.");

    private static JsonObject ToJsonObject(string json, string errorMessage)
        => JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);

    public async ValueTask DisposeAsync()
    {
        await TerminateProcess(binaryUri, process, channel);
    }

    private static async Task TerminateProcess(IOUri binaryUri, Process process, GrpcChannel? channel)
    {
        try
        {
            if (!process.HasExited)
            {
                // let's try and force-kill the process until we have a better option (e.g. sending a SIGTERM, or adding a Close event to the gRPC contract)
                process.Kill();

                // wait for a maximum of 15s for shutdown to occur - otherwise, give up and detatch, in case the process has hung
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await process.WaitForExitAsync(cts.Token);
            }
        }
        catch (Exception ex)
        {
            WriteTrace(binaryUri, () => $"Failed to terminate process: {ex}");
            // ignore exceptions - this is best-effort, and we want to avoid an exception from
            // process.Kill() bubbling up and masking the original exception that was thrown
        }
        finally
        {
            channel?.Dispose();
            process.Dispose();
        }
    }
}
