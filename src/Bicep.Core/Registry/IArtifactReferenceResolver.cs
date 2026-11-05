// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// Resolves artifact reference text to the artifact it names.
/// </summary>
public interface IArtifactReferenceResolver
{
    /// <param name="referencingUri">The URI of the file that contains the reference. Relative paths and configuration are resolved from it.</param>
    ResultWithDiagnosticBuilder<ArtifactReference> TryResolve(IOUri referencingUri, ArtifactType artifactType, string referenceText);
}
