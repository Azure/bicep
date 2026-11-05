// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Registry;

public static class ArtifactHelper
{
    /// <summary>
    /// Gets the distinct remote artifacts to restore. Local artifacts are never restored.
    /// </summary>
    public static ImmutableHashSet<ExternalArtifactReference> GetValidArtifactReferences(IEnumerable<ArtifactResolutionInfo> artifacts)
        => [.. artifacts
            .Select(t => t.Reference)
            .OfType<ExternalArtifactReference>()];
}
