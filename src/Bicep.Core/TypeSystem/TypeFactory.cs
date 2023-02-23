// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem;

public static class TypeFactory
{
    private record struct IntegerAttributes(long? MinValue, long? MaxValue, TypeSymbolValidationFlags ValidationFlags);
    private record struct IntegerLiteralAttributes(long Value, TypeSymbolValidationFlags ValidationFlags);

    private static ConcurrentDictionary<IntegerAttributes, IntegerType> IntegerTypePool = new();
    private static ConcurrentDictionary<IntegerLiteralAttributes, IntegerLiteralType> IntegerLiteralTypePool = new();

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

    public static TypeSymbol CreateArrayType(ITypeReference itemType, TypeSymbolValidationFlags validationFlags = TypeSymbolValidationFlags.Default, long? minLength = null, long? maxLength = null)
    {
        if (ReferenceEquals(itemType, LanguageConstants.Any) && !minLength.HasValue && !maxLength.HasValue)
        {
            return LanguageConstants.Array;
        }

        return new TypedArrayType(itemType, validationFlags, minLength, maxLength);
    }
}
