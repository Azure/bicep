// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation;

/// <summary>
/// Objects that implement IArtifactReferenceSyntax, contain syntax that can reference a foregin artifact, the artifact address
/// is returned by `TryGetPath` and `SourceSyntax` contains the source syntax object to use for error propagation.
/// </summary>
public interface IArtifactReferenceSyntax
{
    public SyntaxBase SourceSyntax { get; }

    public StringSyntax? TryGetPath();
}
