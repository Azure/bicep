// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem;

public class StringType : TypeSymbol
{
    internal StringType(long? minLength, long? maxLength, TypeSymbolValidationFlags validationFlags)
        : base(LanguageConstants.TypeNameString)
    {
        ValidationFlags = validationFlags;
        MinLength = minLength;
        MaxLength = maxLength;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public long? MinLength { get; }

    public long? MaxLength { get; }

    public override bool Equals(object? other) => other is StringType otherString &&
        ValidationFlags == otherString.ValidationFlags &&
        MinLength == otherString.MinLength &&
        MaxLength == otherString.MaxLength;

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinLength, MaxLength);
}
