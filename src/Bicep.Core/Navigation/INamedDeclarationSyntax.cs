// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    /// <summary>
    /// Represents a named syntax declaration.
    /// </summary>
    /// <remarks>This is used to distinguish a declaration from syntax that references the declaration.</remarks>
    public interface INamedDeclarationSyntax : IDeclarationSyntax
    {
        IdentifierSyntax Name { get; }
    }
}
