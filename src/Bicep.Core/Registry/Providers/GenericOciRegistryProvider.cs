// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Providers;

public class GenericOciRegistryProvider : IRegistryProvider
{
    private readonly OrasOciRegistryTransport orasTransport;

    public GenericOciRegistryProvider(OrasOciRegistryTransport orasTransport)
    {
        this.orasTransport = orasTransport;
    }

    public string Name => WellKnownRegistryProviders.Generic;

    public int Priority => 0;

    public bool CanHandle(string registry) => true;

    public IOciRegistryTransport GetTransport(string registry) => orasTransport;

    public IRegistrySession CreateSession(RegistryRef reference, RegistryProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(context);

        return new OrasRegistrySession(reference.Host, reference.Repository, context.CredentialChain);
    }
}
