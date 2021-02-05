// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    public interface INamedDeclarationSyntax
    {
        IdentifierSyntax Name { get; }
    }
}