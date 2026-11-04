// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;

namespace Bicep.Core.Registry;

/// <summary>
/// A reference to an artifact stored in a remote registry. It must be restored to the local cache before it can be loaded.
/// </summary>
public abstract class ExternalArtifactReference : ArtifactReference
{
    protected ExternalArtifactReference(IFeatureProvider featureProvider, IBicepConfiguration configuration, string scheme, ArtifactType type)
        : base(featureProvider, configuration, scheme, type)
    {
    }
}
