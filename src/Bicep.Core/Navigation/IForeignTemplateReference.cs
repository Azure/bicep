// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation;

public interface IForeignArtifactReference
{
    public SyntaxBase ReferenceSourceSyntax { get; }

    public StringSyntax? TryGetPath();
}
