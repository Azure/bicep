// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;

namespace Bicep.Core.Artifacts;

/// <summary>
/// A module or extension as the compiler, the language server and local deploy use it. It is backed by files, which are
/// read on demand.
/// </summary>
public abstract class Artifact
{
    protected Artifact(ArtifactReference reference)
    {
        this.Reference = reference;
    }

    public ArtifactReference Reference { get; }
}
