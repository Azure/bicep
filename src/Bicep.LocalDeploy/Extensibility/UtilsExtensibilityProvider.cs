// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    internal partial class SerializationContext : JsonSerializerContext { }

    public record WaitRequest(
        int durationMs);

    public record WaitResponse();

    public record AssertRequest(
        string name,
        bool condition);

    public record AssertResponse();

    public Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        switch (request.Resource.Type)
        {
            case "Wait":
                return new(new(request.Resource.Type, new JObject()), null, null);
            case "Assert":
                return new(new(request.Resource.Type, new JObject()), null, null);
        }
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
        }

        throw new NotImplementedException();
    }
}