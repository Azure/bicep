// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// The content of an artifact to publish.
/// </summary>
public abstract record ArtifactPackage
{
    private protected ArtifactPackage() { }

    public abstract ArtifactType Type { get; }
}
