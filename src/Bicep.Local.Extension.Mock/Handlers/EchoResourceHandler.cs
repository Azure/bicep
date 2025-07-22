// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Json;
using Bicep.Core.Syntax;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types.Attributes;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Extension.Mock.Handlers;

public class EchoResourceHandler : IResourceHandler<EchoResource>
{
    public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<HandlerResponse> Preview(HandlerRequest<EchoResource> request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public async Task<HandlerResponse> CreateOrUpdate(HandlerRequest<EchoResource> request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        return HandlerResponse.Success(
            resourceType: request.Type,
            properties: JsonNode.Parse(JsonSerializer.Serialize(request.Resource))!.AsObject(),
            identifiers: [],
            apiVersion: request.ApiVersion);
    }
}
