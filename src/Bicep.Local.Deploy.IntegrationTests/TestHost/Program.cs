// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text.Json;
using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Rpc;
using Grpc.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var processPath = Environment.ProcessPath ?? throw new InvalidOperationException("The test host requires an apphost.");
if (args is ["--hold-output", var readyPath])
{
    using var output = Console.OpenStandardOutput();
    using var error = Console.OpenStandardError();
    await output.WriteAsync("Inherited stdout\n"u8.ToArray());
    await error.WriteAsync("Inherited stderr\n"u8.ToArray());
    await File.WriteAllTextAsync($"{readyPath}.tmp", Environment.ProcessId.ToString());
    File.Move($"{readyPath}.tmp", readyPath);
    await Task.Delay(Timeout.InfiniteTimeSpan);
    return;
}

var hostName = Path.GetFileNameWithoutExtension(processPath);
var plaintext = hostName == "plaintext-host";
var socket = GetArgument("--socket");
var pipe = GetArgument("--pipe");

var endpointPath = Path.Combine(AppContext.BaseDirectory, $"{hostName}.endpoint.json");
await File.WriteAllTextAsync(
    $"{endpointPath}.tmp",
    JsonSerializer.Serialize(new { Socket = socket, Pipe = pipe, ProcessId = Environment.ProcessId, Arguments = args }));
File.Move($"{endpointPath}.tmp", endpointPath);

if (hostName is "inherited-output-host" or "exit-with-inherited-output-host")
{
    using var holder = Process.Start(new ProcessStartInfo
    {
        FileName = processPath,
        UseShellExecute = false,
        RedirectStandardInput = true,
        ArgumentList = { "--hold-output", Path.Combine(AppContext.BaseDirectory, $"{hostName}.output-holder") },
    }) ?? throw new InvalidOperationException("Failed to start the inherited-output fixture.");
    holder.StandardInput.Close();
}

if (hostName is "exit-host" or "exit-with-inherited-output-host")
{
    return;
}

if (hostName == "stall-host")
{
    await Task.Delay(Timeout.InfiniteTimeSpan);
    return;
}

var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.Services.AddGrpc();
builder.Services.AddSingleton(new TestHostState(plaintext));

if (plaintext)
{
    // Deliberately bypass the SDK host so this peer stays unauthenticated after the SDK is fixed.
    builder.WebHost.ConfigureKestrel(options =>
    {
        if (socket is { })
        {
            options.ListenUnixSocket(socket, listener => listener.Protocols = HttpProtocols.Http2);
        }
        else if (pipe is { })
        {
            options.ListenNamedPipe(pipe, listener => listener.Protocols = HttpProtocols.Http2);
        }
        else
        {
            throw new InvalidOperationException("A test endpoint is required.");
        }
    });
}
else
{
    var previousContext = SynchronizationContext.Current;
    try
    {
        if (hostName == "sync-context-host")
        {
            SynchronizationContext.SetSynchronizationContext(new RejectingSynchronizationContext());
        }

        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, $"{hostName}.bootstrap-ready"), "");
        if (hostName is "sync-host" or "sync-context-host")
        {
            var configuredBuilder = builder.AddBicepExtensionHost(args);
            if (!ReferenceEquals(configuredBuilder, builder))
            {
                throw new InvalidOperationException("The synchronous startup API must return the original builder.");
            }
        }
        else
        {
            await builder.AddBicepExtensionHostAsync(args);
        }
    }
    catch (Exception exception) when (exception is EndOfStreamException or InvalidOperationException or OperationCanceledException)
    {
        var rejectedPath = Path.Combine(AppContext.BaseDirectory, $"{hostName}.bootstrap-rejected");
        File.WriteAllText($"{rejectedPath}.tmp", $"{exception.GetType().Name}\n{exception}");
        File.Move($"{rejectedPath}.tmp", rejectedPath);
        Environment.ExitCode = 1;
        return;
    }
    finally
    {
        SynchronizationContext.SetSynchronizationContext(previousContext);
    }
}
await using var app = builder.Build();
app.MapGrpcService<TestExtension>();
await app.RunAsync();

string? GetArgument(string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

public sealed record TestHostState(bool Plaintext);

public sealed class RejectingSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback callback, object? state)
        => throw new InvalidOperationException("Synchronous host setup must not require the caller's synchronization context.");
}

public sealed class TestExtension(TestHostState state) : BicepExtension.BicepExtensionBase
{
    public override Task<Empty> Ping(Empty request, ServerCallContext context)
        => Task.FromResult(new Empty());

    public override Task<TypeFilesResponse> GetTypeFiles(Empty request, ServerCallContext context)
        => Task.FromResult(new TypeFilesResponse { IndexFile = """{"resources":{}}""" });

    public override Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, ServerCallContext context)
        => Task.FromResult(CreateResponse(request.Type));

    public override Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, ServerCallContext context)
        => Task.FromResult(CreateResponse(request.Type));

    public override Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, ServerCallContext context)
        => Task.FromResult(CreateResponse(request.Type));

    public override Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, ServerCallContext context)
        => Task.FromResult(CreateResponse(request.Type));

    private LocalExtensibilityOperationResponse CreateResponse(string type)
    {
        var value = state.Plaintext ? "FORGED_TEST_OUTPUT" : "LEGITIMATE_TEST_OUTPUT";
        return new()
        {
            Resource = new()
            {
                Type = type,
                ApiVersion = "v1",
                Identifiers = """{"name":"test-resource"}""",
                Properties = JsonSerializer.Serialize(new { uri = "test-input", body = value, value }),
                Status = "Succeeded",
            },
        };
    }
}
