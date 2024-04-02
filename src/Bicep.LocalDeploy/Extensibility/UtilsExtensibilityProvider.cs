// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Data;
using Azure.Deployments.Extensibility.Messages;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Azure.Bicep.LocalDeploy.Extensibility;

public partial class UtilsExtensibilityProvider : IExtensibilityProvider
{
    [JsonSerializable(typeof(WaitRequest))]
    [JsonSerializable(typeof(WaitResponse))]
    [JsonSerializable(typeof(AssertRequest))]
    [JsonSerializable(typeof(AssertResponse))]
    [JsonSerializable(typeof(RunScriptRequest))]
    [JsonSerializable(typeof(RunScriptResponse))]
    [JsonSerializable(typeof(LogRequest))]
    [JsonSerializable(typeof(LogResponse))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    internal partial class SerializationContext : JsonSerializerContext { }

    public record WaitRequest(
        int durationMs);

    public record WaitResponse();

    public record AssertRequest(
        string name,
        bool condition);

    public record AssertResponse();

    public record LogRequest(
        string message);

    public record LogResponse();

    [JsonConverter(typeof(JsonStringEnumConverter<ScriptType>))]
    public enum ScriptType
    {
        Bash,
        PowerShell,
    }

    public record RunScriptRequest(
        ScriptType type,
        string script);

    public record RunScriptResponse(
        int exitCode,
        string stdout,
        string stderr);

    private readonly Action<string> logFunc;

    public UtilsExtensibilityProvider(Action<string> logFunc)
    {
        this.logFunc = logFunc;
    }

    public Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        switch (request.Resource.Type)
        {
            case "Wait": {
                var body = JsonSerializer.Deserialize(request.Resource.Properties.ToJson(), SerializationContext.Default.WaitRequest)
                    ?? throw new InvalidOperationException("Failed to deserialize request body");

                await Task.Delay(body.durationMs);

                return new ExtensibilityOperationResponse(
                    new ExtensibleResourceData(request.Resource.Type, new JObject()),
                    null,
                    null);
            }
            case "Assert": {
                var body = JsonSerializer.Deserialize(request.Resource.Properties.ToJson(), SerializationContext.Default.AssertRequest)
                    ?? throw new InvalidOperationException("Failed to deserialize request body");

                if (!body.condition)
                {
                    return new ExtensibilityOperationResponse(
                        null,
                        null,
                        new[] {
                            new ExtensibilityError("AssertionFailed", $"Assertion '{body.name}' failed!", ""),
                        });
                }

                return new ExtensibilityOperationResponse(
                    new ExtensibleResourceData(request.Resource.Type, new JObject()),
                    null,
                    null);
            }
            case "Log": {
                var input = JsonSerializer.Deserialize(request.Resource.Properties.ToJson(), SerializationContext.Default.LogRequest)
                    ?? throw new InvalidOperationException("Failed to deserialize request body");

                logFunc(input.message);

                return new ExtensibilityOperationResponse(
                    new ExtensibleResourceData(request.Resource.Type, new JObject()),
                    null,
                    null);
            }
            case "Script": {
                var input = JsonSerializer.Deserialize(request.Resource.Properties.ToJson(), SerializationContext.Default.RunScriptRequest)
                    ?? throw new InvalidOperationException("Failed to deserialize request body");
                
                var scriptOutput = input.type switch {
                    ScriptType.Bash => RunBashScript(input.script),
                    ScriptType.PowerShell => RunPowerShellScript(input.script),
                    _ => throw new InvalidOperationException($"Unknown script type '{input.type}'"),
                };
                var output = JsonSerializer.Serialize(scriptOutput, SerializationContext.Default.RunScriptResponse).FromJson<JToken>();

                return new ExtensibilityOperationResponse(
                    new ExtensibleResourceData(request.Resource.Type, output),
                    null,
                    null);
            }
        }

        throw new NotImplementedException();
    }

    private static RunScriptResponse RunBashScript(string script)
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{script.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            }
        };

        proc.Start();
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        return new(proc.ExitCode, stdout, stderr);
    }

    private static RunScriptResponse RunPowerShellScript(string script)
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "pwsh",
                Arguments = $"-c \"{script.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            }
        };

        proc.Start();
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        return new(proc.ExitCode, stdout, stderr);
    }
}