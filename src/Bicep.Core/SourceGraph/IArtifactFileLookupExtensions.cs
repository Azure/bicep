// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Registry;

namespace Bicep.Core.SourceGraph;

public static class IArtifactFileLookupExtensions
{
    /// <summary>
    /// Gets the artifact reference that was resolved for <paramref name="syntax"/> when the source file grouping was built.
    /// Returns null if the syntax was not resolved or its reference is invalid.
    /// </summary>
    public static ArtifactReference? TryGetArtifactReference(this IArtifactFileLookup lookup, IArtifactReferenceSyntax syntax)
        => lookup.ArtifactLookup.TryGetValue(syntax, out var resolutionInfo) ? resolutionInfo.Reference : null;
}
