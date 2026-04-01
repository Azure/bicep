// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Oci;

public interface IOciRegistryTransportFactory
{
    IOciRegistryTransport GetTransport(OciArtifactReference reference);

    IOciRegistryTransport GetTransport(string registry);

    IRegistrySession CreateSession(RegistryRef reference, RegistryProviderContext context);
}
