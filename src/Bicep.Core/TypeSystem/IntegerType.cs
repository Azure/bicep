// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Text;

namespace Bicep.Core.TypeSystem;

public class IntegerType : TypeSymbol
{
    public IntegerType(long? minValue, long? maxValue, TypeSymbolValidationFlags validationFlags)
        : this(FormatName(minValue, maxValue), minValue, maxValue, validationFlags) {}

    public IntegerType(string typeName, long? minValue, long? maxValue, TypeSymbolValidationFlags validationFlags) :
        base(typeName)
    {
        ValidationFlags = validationFlags;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public long? MinValue { get; }

    public long? MaxValue { get; }

    public override string FormatNameForCompoundTypes() => MinValue.HasValue && MaxValue.HasValue ? WrapTypeName() : Name;

    public override bool Equals(object? other) =>
        other is IntegerType otherInt
        ? ValidationFlags == otherInt.ValidationFlags && MinValue == otherInt.MinValue && MaxValue == otherInt.MaxValue
        : false;

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinValue, MaxValue);

    private static string FormatName(long? minValue, long? maxValue)
    {
        StringBuilder nameBuilder = new();

        if (minValue.HasValue)
        {
            nameBuilder.Append(">= ").Append(minValue.Value);
        }

        if (maxValue.HasValue)
        {
            if (nameBuilder.Length > 0)
            {
                nameBuilder.Append(" && ");
            }

            nameBuilder.Append("<= ").Append(maxValue.Value);
        }

        return nameBuilder.Length == 0 ? LanguageConstants.TypeNameInt : nameBuilder.ToString();
    }
}
