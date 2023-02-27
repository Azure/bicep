// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace Bicep.Core.TypeSystem;

public static class TypeFactory
{
    private record struct IntegerAttributes(long? MinValue, long? MaxValue, TypeSymbolValidationFlags ValidationFlags);
    private record struct IntegerLiteralAttributes(long Value, TypeSymbolValidationFlags ValidationFlags);
    private record struct ArrayAttributes(long? MinLength, long? MaxLength, TypeSymbolValidationFlags ValidationFlags);
    private record struct StringAttributes(long? MinLength, long? MaxLength, TypeSymbolValidationFlags ValidationFlags);
    private record struct StringLiteralAttributes(string Value, TypeSymbolValidationFlags ValidationFlags);

    private static ConcurrentDictionary<IntegerAttributes, IntegerType> IntegerTypePool = new();
    private static ConcurrentDictionary<IntegerLiteralAttributes, IntegerLiteralType> IntegerLiteralTypePool = new();
    private static ConcurrentDictionary<ArrayAttributes, ArrayType> ArrayTypePool = new();
    private static ConcurrentDictionary<StringAttributes, StringType> StringTypePool = new();
    private static ConcurrentDictionary<StringLiteralAttributes, StringLiteralType> StringLiteralTypePool = new();

    public static TypeSymbol CreateIntegerType(long? minValue = null, long? maxValue = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value == maxValue.Value)
        {
            return CreateIntegerLiteralType(minValue.Value, validationFlags);
        }

        return IntegerTypePool.GetOrAdd(new(minValue, maxValue, validationFlags), BuildIntegerType);
    }

    private static IntegerType BuildIntegerType(IntegerAttributes attributes)
        => new(attributes.MinValue, attributes.MaxValue, attributes.ValidationFlags);

    public static IntegerLiteralType CreateIntegerLiteralType(long value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => IntegerLiteralTypePool.GetOrAdd(new(value, validationFlags), BuildIntegerLiteralType);

    private static IntegerLiteralType BuildIntegerLiteralType(IntegerLiteralAttributes attributes)
        => new(attributes.Value, attributes.ValidationFlags);

    public static ArrayType CreateArrayType(long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => CreateArrayType(LanguageConstants.Any, minLength, maxLength, validationFlags);

    public static ArrayType CreateArrayType(ITypeReference itemType, long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
    {
        if (ReferenceEquals(itemType, LanguageConstants.Any))
        {
            return ArrayTypePool.GetOrAdd(new(minLength, maxLength, validationFlags), BuildArrayType);
        }

        return new TypedArrayType(itemType, validationFlags, minLength, maxLength);
    }

    private static ArrayType BuildArrayType(ArrayAttributes attributes)
        => new(attributes.ValidationFlags, attributes.MinLength, attributes.MaxLength);

    public static StringType CreateStringType(long? minLength = null, long? maxLength = null, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => StringTypePool.GetOrAdd(new(minLength, maxLength, validationFlags), BuildStringType);

    private static StringType BuildStringType(StringAttributes attributes)
        => new(attributes.MinLength, attributes.MaxLength, attributes.ValidationFlags);

    public static StringLiteralType CreateStringLiteralType(string value, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default)
        => StringLiteralTypePool.GetOrAdd(new(value, validationFlags), BuildStringLiteralType);

    private static StringLiteralType BuildStringLiteralType(StringLiteralAttributes attributes)
        => new(attributes.Value, attributes.ValidationFlags);
}
