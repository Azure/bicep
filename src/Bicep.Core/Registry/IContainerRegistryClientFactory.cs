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
        ContainerRegistryContentClient CreateAuthenticatedBlobClient(IBicepCloudConfiguration cloud, Uri registryUri, string repository);

        ContainerRegistryContentClient CreateAnonymousBlobClient(IBicepCloudConfiguration cloud, Uri registryUri, string repository);

        ContainerRegistryClient CreateAuthenticatedContainerClient(IBicepCloudConfiguration cloud, Uri registryUri);

        ContainerRegistryClient CreateAnonymousContainerClient(IBicepCloudConfiguration cloud, Uri registryUri);
    }
}
