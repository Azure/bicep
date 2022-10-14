// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem;

/// <summary>
/// Represents a user-defined type.
/// </summary>
public class DeclaredType : TypeSymbol
{
    public DeclaredType(DeclaredTypeSymbol symbol, TypeSymbolValidationFlags validationFlags) : base(symbol.Name)
    {
        TypeSymbol = symbol;
        ValidationFlags = validationFlags;
    }

    public DeclaredTypeSymbol TypeSymbol { get; }

    public override TypeKind TypeKind => TypeSymbol.Type.TypeKind;

    public override TypeSymbolValidationFlags ValidationFlags { get; }
}
