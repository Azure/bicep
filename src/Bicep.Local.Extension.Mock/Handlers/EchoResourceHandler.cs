// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Json;
using Bicep.Local.Extension.Protocol;

namespace Bicep.Local.Extension.Mock.Handlers;

public record EchoRequest(
    string Payload);

public record EchoResponse(
    string Payload);

public class EchoResourceHandler : IResourceHandler
{
    public string ResourceType => "echo";

    public Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public async Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var requestBody = JsonSerializer.Deserialize(request.Resource.Properties, SerializationContext.Default.EchoRequest)
            ?? throw new InvalidOperationException("Failed to deserialize request body");

        var responseBody = new EchoResponse(requestBody.Payload);
        var response = new ExtensibleResourceData(
            request.Resource.Type,
            JsonNode.Parse(JsonSerializer.Serialize(responseBody, SerializationContext.Default.EchoResponse))!.AsObject());

        return new ExtensibilityOperationResponse(response, null, null);
    }
}
