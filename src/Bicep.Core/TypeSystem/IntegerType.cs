// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem;

public class IntegerType : TypeSymbol
{
    internal IntegerType(long? minValue, long? maxValue, TypeSymbolValidationFlags validationFlags)
        : base(LanguageConstants.TypeNameInt)
    {
        ValidationFlags = validationFlags;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public long? MinValue { get; }

    public long? MaxValue { get; }

    public override bool Equals(object? other) => other is IntegerType otherInt &&
        ValidationFlags == otherInt.ValidationFlags &&
        MinValue == otherInt.MinValue &&
        MaxValue == otherInt.MaxValue;

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinValue, MaxValue);
}
