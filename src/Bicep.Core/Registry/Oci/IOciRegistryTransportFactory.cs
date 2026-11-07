// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Oci;

public interface IOciRegistryTransportFactory
{
    IOciRegistryTransport GetTransport(OciArtifactReference reference);

    IOciRegistryTransport GetTransport(string registry, bool useOrasSdk);

    /// <summary>
    /// Creates a session for the reference's registry, using the cloud and transport from its <see cref="OciArtifactContext"/>.
    /// </summary>
    IRegistrySession CreateSession(OciArtifactReference reference);
}
