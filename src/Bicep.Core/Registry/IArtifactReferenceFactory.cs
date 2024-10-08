// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;

namespace Bicep.Core.Registry;

public interface IArtifactReferenceFactory
{
    ImmutableArray<string> AvailableSchemes(Uri parentModuleUri);

    ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(ArtifactType artifactType, string reference, Uri parentModuleUri);

    ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(IArtifactReferenceSyntax artifactDeclaration, Uri parentModuleUri);
}
