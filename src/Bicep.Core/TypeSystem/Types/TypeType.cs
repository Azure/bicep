// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// The type of a symbol that may only be used as a type, not a value.
/// </summary>
public class TypeType : TypeSymbol
{
    private readonly TypeSymbol wrappedType;

    public TypeType(TypeSymbol toWrap) : base($"Type<{toWrap.Name}>")
    {
        wrappedType = toWrap;
    }

    public TypeSymbol Unwrapped => wrappedType;

    public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

    public override TypeKind TypeKind => TypeKind.TypeReference;

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => wrappedType.GetDiagnostics();
}
