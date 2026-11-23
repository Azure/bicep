// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Extensibility.Core.V2.Json;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Deploy.Types;
using Bicep.Local.Rpc;
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
    IOUri binaryUri,
    LocalExtensionAuthentication authentication,
    Task processExit,
    string? socketPath,
    string? socketDirectory) : ILocalExtension
{
    private bool disposed;

    private static void WriteTrace(IOUri binaryUri, Func<string> getMessage)
        => Trace.WriteLine($"[{binaryUri}] {getMessage()}");

    public static async Task<ILocalExtension> Start(IOUri binaryUri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var binaryPath = binaryUri.GetFilePath();
        using var startup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        string? socketPath = null;
        string? socketDirectory = null;
        GrpcChannel? channel = null;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = binaryPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            },
        };
        Task? processExit = null;
        var authentication = LocalExtensionAuthentication.Create();
        var exited = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            string transportArgument;
            string endpoint;
            Func<LocalExtensionAuthentication, GrpcChannel> channelBuilder;
            // While modern Windows technically supports UDS, many libraries (like Rust's Tokio) do not
            if (Socket.OSSupportsUnixDomainSockets && !OperatingSystem.IsWindows())
            {
                // CreateTempSubdirectory atomically creates a directory accessible only to the current user (0700).
                socketDirectory = Directory.CreateTempSubdirectory("bicep-").FullName;
                socketPath = Path.Combine(socketDirectory, "s.sock");
                transportArgument = "--socket";
                endpoint = socketPath;
                channelBuilder = authentication => GrpcChannelHelper.CreateDomainSocketChannel(endpoint, authentication);
            }
            else
            {
                transportArgument = "--pipe";
                endpoint = $"{Guid.NewGuid()}.tmp";
                channelBuilder = authentication => GrpcChannelHelper.CreateNamedPipeChannel(endpoint, authentication);
            }
            process.StartInfo.ArgumentList.Add(transportArgument);
            process.StartInfo.ArgumentList.Add(endpoint);

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

            process.Exited += (_, _) => exited.TrySetResult();
            process.EnableRaisingEvents = true;
            process.Start();
            processExit = exited.Task;
            using var connectionTimeout = CancellationTokenSource.CreateLinkedTokenSource(startup.Token, cancellationToken);

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            try
            {
                var bootstrap = authentication.WriteBootstrapAsync(process.StandardInput.BaseStream, connectionTimeout.Token);
                await WaitForStartup(bootstrap, processExit, connectionTimeout, binaryUri);
            }
            finally
            {
                process.StandardInput.Close();
            }

            channel = channelBuilder(authentication);
            var client = new Rpc.BicepExtension.BicepExtensionClient(channel);

            var connection = GrpcChannelHelper.WaitForConnectionAsync(client, connectionTimeout.Token);
            await WaitForStartup(connection, processExit, connectionTimeout, binaryUri);

            return new GrpcLocalExtension(client, process, channel, binaryUri, authentication, processExit, socketPath, socketDirectory);
        }
        catch (Exception ex)
        {
            var authenticationFailure = GrpcChannelHelper.IsAuthenticationFailure(ex);
            var message = ex switch
            {
                _ when processExit is null => $"Failed to start extension {binaryUri}: {ex.Message}",
                _ when process.HasExited => $"Extension {binaryUri} exited with code {process.ExitCode} before becoming ready.",
                _ when startup.IsCancellationRequested => $"Extension {binaryUri} did not become ready within 30 seconds.",
                _ when authenticationFailure => $"Failed to authenticate extension {binaryUri}: {ex.Message}",
                _ => $"Failed to initialize extension {binaryUri}: {ex.Message}",
            };
            if (authenticationFailure)
            {
                message += " Bicep requires an authenticated extension host. If this extension was built with an older SDK, ask its author to upgrade to an extension SDK that supports authenticated launches, rebuild and republish the extension. Then update your extension reference and restore it.";
            }

            await TerminateProcess(binaryUri, process, channel, authentication, processExit, socketPath, socketDirectory).ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException("Local extension launch was cancelled.", ex, cancellationToken);
            }

            throw new InvalidOperationException(message, ex);
        }
    }

    private static async Task WaitForStartup(Task operation, Task exited, CancellationTokenSource startup, IOUri binaryUri)
    {
        // Cancellation may finish the wait before the operation; observe any later failure.
        _ = operation.ContinueWith(
            faulted => WriteTrace(binaryUri, () => $"Extension operation failed: {faulted.Exception}"),
            CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
        var completed = await Task.WhenAny(operation, exited).WaitAsync(startup.Token).ConfigureAwait(false);
        if (exited.IsCompleted)
        {
            await startup.CancelAsync().ConfigureAwait(false);
            throw new OperationCanceledException("The local extension process exited.");
        }
        await completed.ConfigureAwait(false);
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
        if (disposed)
        {
            return;
        }

        disposed = true;
        // The OS exit signal cannot depend on the disposal caller's synchronization context.
#pragma warning disable VSTHRD003
        await TerminateProcess(binaryUri, process, channel, authentication, processExit, socketPath, socketDirectory).ConfigureAwait(false);
#pragma warning restore VSTHRD003
    }

    internal static async Task TerminateProcess(IOUri binaryUri, Process process, GrpcChannel? channel,
        LocalExtensionAuthentication authentication, Task? processExit, string? socketPath, string? socketDirectory,
        Action? terminateProcess = null, TimeSpan? exitTimeout = null)
    {
        if (channel is not null)
        {
            DisposeResources(binaryUri, channel);
        }
        try
        {
            if (processExit is not null)
            {
                if (!process.HasExited)
                {
                    terminateProcess ??= process.Kill;
                    terminateProcess();
                }
                await processExit.WaitAsync(exitTimeout ?? TimeSpan.FromSeconds(15)).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception
            or TimeoutException or AggregateException)
        {
            WriteTrace(binaryUri, () => $"Failed to terminate process; releasing parent-owned launch resources: {ex}");
        }
        finally
        {
            DisposeResources(binaryUri, process, authentication);
            if (socketPath is not null && File.Exists(socketPath))
            {
                try
                {
                    File.Delete(socketPath);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    WriteTrace(binaryUri, () => $"Failed to remove extension socket: {ex.Message}");
                }
            }
            if (socketDirectory is not null)
            {
                try
                {
                    Directory.Delete(socketDirectory, recursive: false);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    WriteTrace(binaryUri, () => $"Failed to remove extension socket directory: {ex.Message}");
                }
            }
        }
    }

    internal static void DisposeResources(IOUri binaryUri, params IDisposable[] resources)
    {
        foreach (var resource in resources)
        {
            try
            {
                resource.Dispose();
            }
            catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception
                or CryptographicException or IOException or UnauthorizedAccessException)
            {
                WriteTrace(binaryUri, () => $"Failed to dispose {resource.GetType().Name}: {ex}");
            }
        }
    }

}
