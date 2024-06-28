// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Engine.Host.Azure.ExtensibilityV2.Contract.Models;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Messages;

namespace Bicep.Local.Deploy.Extensibility;

public abstract class LocalExtensibilityProvider : IAsyncDisposable
{
    public abstract Task<ResourceResponseBody> Delete(ResourceReferenceRequestBody request, CancellationToken cancellationToken);

    public abstract Task<ResourceResponseBody> Get(ResourceReferenceRequestBody request, CancellationToken cancellationToken);

    public abstract Task<ResourceResponseBody> Preview(ResourceRequestBody request, CancellationToken cancellationToken);

    public abstract Task<ResourceResponseBody> CreateOrUpdate(ResourceRequestBody request, CancellationToken cancellationToken);

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
