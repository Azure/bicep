// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Json;
using Bicep.Local.Extension.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Extension.Mock.Handlers;

public record EchoRequest(
    string Payload);

public record EchoResponse(
    string Payload);

public class EchoResourceHandler : IResourceHandler
{
    public string ResourceType => "echo";

    public Task<LocalExtensionOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<LocalExtensionOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<LocalExtensionOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public async Task<LocalExtensionOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var requestBody = JsonSerializer.Deserialize(request.Properties, SerializationContext.Default.EchoRequest)
            ?? throw new InvalidOperationException("Failed to deserialize request body");

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var responseBody = new EchoResponse(requestBody.Payload);
        return new LocalExtensionOperationResponse(
            Resource: new Resource(request.Type, request.ApiVersion, "Succeeded", identifiers, null, JsonNode.Parse(JsonSerializer.Serialize(responseBody, SerializationContext.Default.EchoResponse))!.AsObject()),
            ErrorData: null);
    }
}
