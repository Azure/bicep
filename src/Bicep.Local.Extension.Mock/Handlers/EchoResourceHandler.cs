// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.Mock.Handlers;

public class EchoResourceHandler : TypedResourceHandler<EchoResource, EchoResourceIdentifiers>
{
    protected override async Task<ResourceResponse> CreateOrUpdate(ResourceRequest request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return GetResponse(request);
    }

    protected override EchoResourceIdentifiers GetIdentifiers(EchoResource properties)
        => new();
}
