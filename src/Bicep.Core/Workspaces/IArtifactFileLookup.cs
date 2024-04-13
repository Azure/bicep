// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.Workspaces;

public interface IArtifactFileLookup
{
    ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference);

    ImmutableDictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> ArtifactLookup { get; }

    ImmutableDictionary<ISourceFile, ImmutableHashSet<ImplicitProvider>> ImplicitProviders { get; }
}
