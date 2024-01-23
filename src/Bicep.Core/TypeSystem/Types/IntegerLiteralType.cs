// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;

namespace Bicep.Core.TypeSystem.Types;

public class IntegerLiteralType : TypeSymbol
{
    internal IntegerLiteralType(long value, TypeSymbolValidationFlags validationFlags)
        : base(value.ToString(CultureInfo.InvariantCulture))
    {
        Value = value;
        ValidationFlags = validationFlags;
    }

    public override TypeKind TypeKind => TypeKind.IntegerLiteral;

    public long Value { get; }

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public override bool Equals(object? other) => other is IntegerLiteralType otherIntLiteral &&
        Value == otherIntLiteral.Value &&
        ValidationFlags == otherIntLiteral.ValidationFlags;

    public override int GetHashCode() => HashCode.Combine(TypeKind, Value, ValidationFlags);
}
