﻿using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    /// <summary>
    /// Represents a syntax declaration.
    /// </summary>
    /// <remarks>This is used to distinguish a declaration from syntax that references the declaration.</remarks>
    public interface IDeclarationSyntax
    {
        IdentifierSyntax Name { get; }
    }
}