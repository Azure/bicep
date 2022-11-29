// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;

namespace Bicep.Core.TypeSystem;

public class IntegerLiteralType : TypeSymbol
{
    public IntegerLiteralType(long value)
        : base(value.ToString(CultureInfo.InvariantCulture))
    {
        Value = value;
    }

    public IntegerLiteralType(string typeName, long value) :
        base(typeName)
    {
        Value = value;
    }

    public override TypeKind TypeKind => TypeKind.IntegerLiteral;

    public long Value { get; }

    public override bool Equals(object? other) =>
        other is IntegerLiteralType otherIntLiteral ? otherIntLiteral == this : false;

    public override int GetHashCode() => (GetType(), Value).GetHashCode();

    public static bool operator ==(IntegerLiteralType? a, IntegerLiteralType? b) => a?.Value == b?.Value;

    public static bool operator !=(IntegerLiteralType? a, IntegerLiteralType? b) => a?.Value != b?.Value;
}
