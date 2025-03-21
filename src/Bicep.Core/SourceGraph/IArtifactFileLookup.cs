// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.SourceGraph
{
    public interface IArtifactFileLookup
    {
        ResultWithDiagnosticBuilder<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference);

        ImmutableDictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> ArtifactLookup { get; }

        ImmutableDictionary<ISourceFile, ImmutableHashSet<ImplicitExtension>> ImplicitExtensions { get; }
    }
}
