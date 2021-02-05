// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    /// <summary>
    /// Represents a top-level syntax declaration.
    /// </summary>
    /// <remarks>This is used to identify a program syntax declaration.</remarks>
    public interface ITopLevelDeclarationSyntax
    {
        Token Keyword { get; }
    }
}