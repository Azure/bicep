// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Core.RegistryClients.Oci;

public interface IOciRegistryTransportFactory
{
    IOciRegistryTransport GetTransport(string registry, bool useOrasSdk);

    /// <summary>
    /// Creates a session for a repository, using the ORAS SDK or the Azure SDK.
    /// </summary>
    IRegistrySession CreateSession(IBicepCloudConfiguration cloud, string registry, string repository, bool useOrasSdk);
}
