// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation;

public static class IArtifactReferenceSyntaxExtensions
{
    public static StringSyntax? TryGetPath(this IArtifactReferenceSyntax syntax)
        => syntax.Path as StringSyntax;
}
