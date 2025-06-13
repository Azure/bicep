// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Extensibility.Core.V2.Models;

namespace Bicep.Local.Deploy.Extensibility;

public interface ILocalExtension : IAsyncDisposable
{
    Task<LocalExtensionOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken);

    Task<LocalExtensionOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken);

    Task<LocalExtensionOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken);

    Task<LocalExtensionOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken);
}
