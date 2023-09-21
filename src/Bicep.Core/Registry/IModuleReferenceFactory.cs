// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;

namespace Bicep.Core.Registry;

public interface IModuleReferenceFactory
{
    ImmutableArray<string> AvailableSchemes(Uri parentModuleUri);

    ResultWithDiagnostic<ArtifactReference> TryGetModuleReference(string reference, Uri parentModuleUri);

    ResultWithDiagnostic<ArtifactReference> TryGetModuleReference(IArtifactReferenceSyntax artifactDeclaration, Uri parentModuleUri);
}
