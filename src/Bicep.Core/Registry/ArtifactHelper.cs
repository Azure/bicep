// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Registry;

public static class ArtifactHelper
{
    public static ImmutableHashSet<ArtifactReference> GetValidArtifactReferences(IEnumerable<ArtifactResolutionInfo> artifacts)
        => [.. artifacts
            .Select(t => t.Reference)
            .WhereNotNull()];
}
