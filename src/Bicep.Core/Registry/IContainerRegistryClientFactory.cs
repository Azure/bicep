// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Creates ACR clients.
    /// </summary>
    /// <remarks>This exists because we need to inject mock clients in integration tests and because the real client constructor requires parameters.</remarks>
    public interface IContainerRegistryClientFactory
    {
        //asdfg combine
        ContainerRegistryContentClient CreateAuthenticatedBlobClient(RootConfiguration configuration, Uri registryUri, string repository);
        ContainerRegistryContentClient CreateAnonymousBlobClient(RootConfiguration configuration, Uri registryUri, string repository);

        ContainerRegistryClient CreateContainerRegistryClient(RootConfiguration configuration, Uri registryUri, bool anonymous);

        //asdfg
        Task<HttpClient> CreateHttpClientAsdfgAsync(RootConfiguration configuration, bool anonymous); //asdfg
    }
}
