// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci;

public interface IOciArtifactReference : IOciArtifactAddressComponents
{
    /// <summary>
    /// Gets the fully qualified artifact reference, which includes the scheme.
    /// </summary>
    /// <example>
    /// br:mcr.microsoft.com/bicep/avm/ptn/aca-lza/hosting-environment:0.1.1
    /// </example>
    string FullyQualifiedReference { get; }
}
