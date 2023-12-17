// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Registry;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation;

/// <summary>
/// Objects that implement IArtifactReferenceSyntax, contain syntax that can reference a foreign artifact, the artifact address
/// is returned by `TryGetPath` and `SourceSyntax` contains the source syntax object to use for error propagation.
/// </summary>
public interface IArtifactReferenceSyntax
{
    SyntaxBase SourceSyntax { get; }

    SyntaxBase? Path { get; }

    ArtifactType GetArtifactType();
}
