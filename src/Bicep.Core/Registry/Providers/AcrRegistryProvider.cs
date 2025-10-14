// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Azure;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;
using Bicep.Core.Registry;

namespace Bicep.Core.Registry.Providers;

public class AcrRegistryProvider : IRegistryProvider
{
    private readonly AzureContainerRegistryManager azureTransport;

    public AcrRegistryProvider(AzureContainerRegistryManager azureTransport)
    {
        this.azureTransport = azureTransport;
    }

    public string Name => WellKnownRegistryProviders.Acr;

    public int Priority => 100;

    public bool CanHandle(string registry) => OciRegistryTransportFactory.IsAzureHost(registry);

    public IOciRegistryTransport GetTransport(string registry) => azureTransport;

    public IRegistrySession CreateSession(RegistryRef reference, RegistryProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(context);

        return new AcrRegistrySession(azureTransport, context.Cloud);
    }
}
