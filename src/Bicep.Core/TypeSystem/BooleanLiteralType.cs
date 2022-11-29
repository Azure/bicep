// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

public class BooleanLiteralType : TypeSymbol
{
    public BooleanLiteralType(bool value)
        : base(value ? "true" : "false")
    {
        Value = value;
    }

    public BooleanLiteralType(string typeName, bool value) :
        base(typeName)
    {
        Value = value;
    }

    public override TypeKind TypeKind => TypeKind.BooleanLiteral;

    public bool Value { get; }

    public override bool Equals(object? other) =>
        other is BooleanLiteralType otherBoolLiteral ? otherBoolLiteral == this : false;

    public override int GetHashCode() => (GetType(), Value).GetHashCode();

    public static bool operator ==(BooleanLiteralType? a, BooleanLiteralType? b) => a?.Value == b?.Value;

    public static bool operator !=(BooleanLiteralType? a, BooleanLiteralType? b) => a?.Value != b?.Value;
}
