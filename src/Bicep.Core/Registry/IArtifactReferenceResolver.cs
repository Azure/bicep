// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Registry;

/// <summary>
/// Resolves artifact reference text to the artifact it names.
/// </summary>
public interface IArtifactReferenceResolver
{
    ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(BicepSourceFile referencingFile, ArtifactType artifactType, string referenceText);

}
