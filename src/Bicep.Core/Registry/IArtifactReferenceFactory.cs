// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Registry;

public interface IArtifactReferenceFactory
{
    ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string reference);

    ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference(BicepSourceFile referencingFile, IArtifactReferenceSyntax artifactDeclaration);
}
