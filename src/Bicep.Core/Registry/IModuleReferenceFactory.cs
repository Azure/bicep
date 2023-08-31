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

    bool TryGetModuleReference(string reference, Uri parentModuleUri, [NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

    bool TryGetModuleReference(IForeignArtifactReference module, Uri parentModuleUri, [NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);
}
