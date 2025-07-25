// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Creates ACR clients.
    /// </summary>
    /// <remarks>This exists because we need to inject mock clients in integration tests and because the real client constructor requires parameters.</remarks>
    public interface IContainerRegistryClientFactory
    {
        ContainerRegistryContentClient CreateAuthenticatedBlobClient(CloudConfiguration cloud, Uri registryUri, string repository);

        ContainerRegistryContentClient CreateAnonymousBlobClient(CloudConfiguration cloud, Uri registryUri, string repository);

        ContainerRegistryClient CreateAuthenticatedContainerClient(CloudConfiguration cloud, Uri registryUri);

        ContainerRegistryClient CreateAnonymousContainerClient(CloudConfiguration cloud, Uri registryUri);
    }
}
