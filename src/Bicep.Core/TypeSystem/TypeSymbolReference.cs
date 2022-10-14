// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem;

public class TypeSymbolReference : DeferredTypeReference
{
    public TypeSymbolReference(DeclaredTypeSymbol typeReferenced, Func<TypeSymbol> typeGetterFunc)
        : base(typeGetterFunc)
    {
        TypeReferenced = typeReferenced;
    }

    public DeclaredTypeSymbol TypeReferenced { get; }
}

