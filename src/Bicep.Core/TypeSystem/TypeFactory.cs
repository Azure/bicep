// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem;

public static class TypeFactory
{
    public static BooleanType CreateBooleanType(TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(validationFlags);

    public static BooleanLiteralType CreateBooleanLiteralType(bool value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(value, validationFlags);

    public static TypeSymbol CreateIntegerType(long? minValue = null, long? maxValue = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value == maxValue.Value)
        {
            return CreateIntegerLiteralType(minValue.Value, validationFlags);
        }

        return new IntegerType(minValue, maxValue, validationFlags);
    }

    public static IntegerLiteralType CreateIntegerLiteralType(long value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(value, validationFlags);

    public static ArrayType CreateArrayType(long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => CreateArrayType(LanguageConstants.Any, minLength, maxLength, validationFlags);

    public static ArrayType CreateArrayType(ITypeReference itemType, long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (maxLength.HasValue && maxLength.Value == 0)
        {
            return new TupleType(ImmutableArray<ITypeReference>.Empty, validationFlags);
        }

        if (ReferenceEquals(itemType, LanguageConstants.Any))
        {
            return new ArrayType(validationFlags, minLength, maxLength);
        }

        return new TypedArrayType(itemType, validationFlags, minLength, maxLength);
    }

    public static TypeSymbol CreateStringType(long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (maxLength.HasValue && maxLength.Value == 0)
        {
            return CreateStringLiteralType(string.Empty, validationFlags);
        }

        return new StringType(minLength, maxLength, validationFlags);
    }

    public static StringLiteralType CreateStringLiteralType(string value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => new(value, validationFlags);
}
