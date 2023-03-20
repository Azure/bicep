// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem;

public class BooleanLiteralType : TypeSymbol
{
    internal BooleanLiteralType(bool value, TypeSymbolValidationFlags validationFlags)
        : base(value ? "true" : "false")
    {
        Value = value;
        ValidationFlags = validationFlags;
    }

    public override TypeKind TypeKind => TypeKind.BooleanLiteral;

    public bool Value { get; }

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public override bool Equals(object? other) => other is BooleanLiteralType otherBoolLiteral &&
        Value == otherBoolLiteral.Value &&
        ValidationFlags == otherBoolLiteral.ValidationFlags;

    public override int GetHashCode() => HashCode.Combine(TypeKind, Value, ValidationFlags);
}
