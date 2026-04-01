// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Azure;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Providers;

public class GenericOciRegistryProvider : IRegistryProvider
{
    private readonly AzureContainerRegistryManager azureTransport;

    public GenericOciRegistryProvider(AzureContainerRegistryManager azureTransport)
    {
        this.azureTransport = azureTransport;
    }

    public string Name => WellKnownRegistryProviders.Generic;

    public int Priority => 0;

    public bool CanHandle(string registry) => true;

    // The legacy GetTransport path (used when OciEnabled=false) delegates to AzureContainerRegistryManager
    // for backward compatibility. The session-based path (OciEnabled=true) uses ORAS via CreateSession.
    public IOciRegistryTransport GetTransport(string registry) => azureTransport;

    public IRegistrySession CreateSession(RegistryRef reference, RegistryProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(context);

        return new OrasRegistrySession(reference.Host, reference.Repository, context.CredentialChain);
    }
}
