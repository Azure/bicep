// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Artifacts.Restore;

public static class RestoreExtensions
{
    /// <summary>
    /// Gets the distinct remote artifacts to restore. Local artifacts are never restored.
    /// </summary>
    public static ImmutableHashSet<RemoteArtifactReference> GetRemoteArtifactReferences(this IEnumerable<ArtifactResolutionInfo> artifacts)
        => [.. artifacts
            .Select(t => t.Reference)
            .OfType<RemoteArtifactReference>()];
}
