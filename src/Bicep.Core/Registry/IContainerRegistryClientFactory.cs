// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Oci;
using System;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Creates ACR clients.
    /// </summary>
    /// <remarks>This exists because we need to inject mock clients in integration tests and because the real client constructor requires parameters.</remarks>
    public interface IContainerRegistryClientFactory
    {
        IOciRegistryContentClient CreateAuthenticatedBlobClient(RootConfiguration configuration, Uri registryUri, string repository);

        IOciRegistryContentClient CreateAnonymousBlobClient(RootConfiguration configuration, Uri registryUri, string repository);
    }
}
