// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Bicep.Core.TypeSystem;

public class StringType : TypeSymbol
{
    public StringType(long? minLength, long? maxLength, TypeSymbolValidationFlags validationFlags)
        : base(FormatName(minLength, maxLength))
    {
        ValidationFlags = validationFlags;
        MinLength = minLength;
        MaxLength = maxLength;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public long? MinLength { get; }

    public long? MaxLength { get; }

    public override bool Equals(object? other) =>
        other is StringType otherInt
        ? ValidationFlags == otherInt.ValidationFlags && MinLength == otherInt.MinLength && MaxLength == otherInt.MaxLength
        : false;

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinLength, MaxLength);

    private static string FormatName(long? minLength, long? maxLength)
    {
        StringBuilder nameBuilder = new(LanguageConstants.TypeNameString);

        if (minLength.HasValue || maxLength.HasValue)
        {
            nameBuilder.Append(" {");

            if (minLength.HasValue)
            {
                nameBuilder.Append('@').Append(LanguageConstants.ParameterMinLengthPropertyName).Append('(').Append(minLength.Value).Append(')');
            }

            if (maxLength.HasValue)
            {
                if (minLength.HasValue)
                {
                    nameBuilder.Append(", ");
                }

                nameBuilder.Append('@').Append(LanguageConstants.ParameterMaxLengthPropertyName).Append('(').Append(maxLength.Value).Append(')');
            }

            nameBuilder.Append('}');
        }

        return nameBuilder.Length == 0 ? LanguageConstants.TypeNameInt : nameBuilder.ToString();
    }
}
