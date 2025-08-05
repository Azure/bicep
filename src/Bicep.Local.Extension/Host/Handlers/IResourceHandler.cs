// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.Handlers;

public interface IResourceHandler
{
    public string? Type { get; }

    public string? ApiVersion { get; }

    Task<Rpc.LocalExtensibilityOperationResponse> CreateOrUpdate(Rpc.ResourceSpecification request, CancellationToken cancellationToken);

    Task<Rpc.LocalExtensibilityOperationResponse> Preview(Rpc.ResourceSpecification request, CancellationToken cancellationToken);

    Task<Rpc.LocalExtensibilityOperationResponse> Get(Rpc.ResourceReference request, CancellationToken cancellationToken);

    Task<Rpc.LocalExtensibilityOperationResponse> Delete(Rpc.ResourceReference request, CancellationToken cancellationToken);
}
