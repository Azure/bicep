// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public interface ISymbolContext
    {
        ITypeManager TypeManager { get; }

        IReadOnlyDictionary<SyntaxBase, Symbol> Bindings { get; }

        Compilation Compilation { get; }
    }
}