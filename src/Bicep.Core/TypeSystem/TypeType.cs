// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem;

/// <summary>
/// The type of a symbol that may only be used as a type, not a value.
/// </summary>
public class TypeType : TypeSymbol
{
    private readonly TypeSymbol wrappedType;

    public TypeType(TypeSymbol toWrap) : base($"Type<{toWrap.Name}>")
    {
        this.wrappedType = toWrap;
    }

    public TypeSymbol Unwrapped => wrappedType;

    public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

    public override TypeKind TypeKind => TypeKind.TypeReference;

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => wrappedType.GetDiagnostics();
}
