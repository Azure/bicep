// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public interface ISymbolContext
    {
        ITypeManager TypeManager { get; }

        Compilation Compilation { get; }
    }
}
