// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Registry;

public interface IModuleReferenceFactory
{
    ImmutableArray<string> AvailableSchemes(Uri parentModuleUri);

    ResultWithDiagnostic<ArtifactReference> TryGetModuleReference(string reference, Uri parentModuleUri);

    ResultWithDiagnostic<ArtifactReference> TryGetModuleReference(IArtifactReferenceSyntax artifactDeclaration, Uri parentModuleUri);
}
