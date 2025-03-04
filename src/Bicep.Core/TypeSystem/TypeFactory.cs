// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

public static class TypeFactory
{
    private static readonly BooleanType UnrefinedBool = new(default);
    private static readonly IntegerType UnrefinedInt = new(default, default, default);
    private static readonly StringType UnrefinedString = new(default, default, default, default);

    public static TypeSymbol CreateBooleanType(TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => validationFlags == UnrefinedBool.ValidationFlags ? UnrefinedBool : new BooleanType(validationFlags);

    public static BooleanLiteralType CreateBooleanLiteralType(bool value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(value, validationFlags);

    public static TypeSymbol CreateIntegerType(long? minValue = null, long? maxValue = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value == maxValue.Value)
        {
            return CreateIntegerLiteralType(minValue.Value, validationFlags);
        }

        if (minValue != UnrefinedInt.MinValue || maxValue != UnrefinedInt.MaxValue || validationFlags != UnrefinedInt.ValidationFlags)
        {
            return new IntegerType(minValue, maxValue, validationFlags);
        }

        return UnrefinedInt;
    }

    public static IntegerLiteralType CreateIntegerLiteralType(long value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(value, validationFlags);

    public static ArrayType CreateArrayType(long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => CreateArrayType(LanguageConstants.Any, minLength, maxLength, validationFlags);

    public static ArrayType CreateStringArrayType(long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => CreateArrayType(LanguageConstants.String, minLength, maxLength, validationFlags);


    public static ArrayType CreateArrayType(ITypeReference itemType, long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (maxLength.HasValue && maxLength.Value == 0)
        {
            return new TupleType([], validationFlags);
        }

        if (ReferenceEquals(itemType, LanguageConstants.Any))
        {
            return new ArrayType(validationFlags, minLength, maxLength);
        }

        return new TypedArrayType(itemType, validationFlags, minLength, maxLength);
    }

    public static TypeSymbol CreateStringType(
        long? minLength = null,
        long? maxLength = null,
        string? pattern = null,
        TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (maxLength.HasValue && maxLength.Value == 0)
        {
            return CreateStringLiteralType(string.Empty, validationFlags);
        }

        if (minLength != UnrefinedString.MinLength ||
            maxLength != UnrefinedString.MaxLength ||
            pattern != UnrefinedString.Pattern ||
            validationFlags != UnrefinedString.ValidationFlags)
        {
            return new StringType(minLength, maxLength, pattern, validationFlags);
        }

        return UnrefinedString;
    }

    public static StringLiteralType CreateStringLiteralType(string value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(value, validationFlags);
}
