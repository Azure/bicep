// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace Bicep.Core.TypeSystem;

public static class TypeFactory
{
    private record struct IntegerAttributes(long? MinValue, long? MaxValue, TypeSymbolValidationFlags ValidationFlags);
    private record struct IntegerLiteralAttributes(long Value, TypeSymbolValidationFlags ValidationFlags);
    private record struct ArrayAttributes(long? MinLength, long? MaxLength, TypeSymbolValidationFlags ValidationFlags);

    private static ConcurrentDictionary<IntegerAttributes, IntegerType> IntegerTypePool = new();
    private static ConcurrentDictionary<IntegerLiteralAttributes, IntegerLiteralType> IntegerLiteralTypePool = new();
    private static ConcurrentDictionary<ArrayAttributes, ArrayType> ArrayTypePool = new();

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

    public static ArrayType CreateArrayType(TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default, long? minLength = null, long? maxLength = null)
        => CreateArrayType(LanguageConstants.Any, validationFlags, minLength, maxLength);

    public static ArrayType CreateArrayType(ITypeReference itemType, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default, long? minLength = null, long? maxLength = null)
    {
        if (ReferenceEquals(itemType, LanguageConstants.Any))
        {
            return ArrayTypePool.GetOrAdd(new(minLength, maxLength, validationFlags), BuildArrayType);
        }

        return new TypedArrayType(itemType, validationFlags, minLength, maxLength);
    }

    private static ArrayType BuildArrayType(ArrayAttributes attributes)
        => new(attributes.ValidationFlags, attributes.MinLength, attributes.MaxLength);
}
