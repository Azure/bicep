// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.RegistryClient;
using System;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Creates ACR clients.
    /// </summary>
    /// <remarks>This exists because we need to inject mock clients in integration tests and because the real client constructor requires parameters.</remarks>
    public interface IContainerRegistryClientFactory
    { 
        BicepRegistryBlobClient CreateBlobClient(Uri registryUri, string repository, TokenCredential credential);
    }
}
