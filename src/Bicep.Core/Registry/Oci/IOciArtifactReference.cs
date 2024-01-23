// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci;

public interface IOciArtifactReference : IArtifactAddressComponents
{
    /// <summary>
    /// Gets the fully qualified artifact reference, which includes the scheme.
    /// </summary>
    string FullyQualifiedReference { get; }
}
