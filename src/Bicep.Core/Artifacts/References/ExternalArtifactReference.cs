// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.References;

/// <summary>
/// A reference to an artifact stored in a remote registry. It must be restored to the local cache before it can be loaded.
/// </summary>
/// <remarks>
/// The <see cref="ExternalArtifactContext"/> is part of the identity: implementations must include it in <see cref="object.Equals(object?)"/>.
/// </remarks>
public abstract class ExternalArtifactReference : ArtifactReference
{
    protected ExternalArtifactReference(ExternalArtifactContext context, string scheme, ArtifactType type)
        : base(scheme, type)
    {
        this.Context = context;
    }

    /// <summary>
    /// Gets the settings used to reach the artifact in its registry and in the local cache.
    /// </summary>
    public ExternalArtifactContext Context { get; }
}
