// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types;

public class StringType : TypeSymbol
{
    internal StringType(long? minLength, long? maxLength, string? pattern, TypeSymbolValidationFlags validationFlags)
        : base(LanguageConstants.TypeNameString)
    {
        ValidationFlags = validationFlags;
        MinLength = minLength;
        MaxLength = maxLength;
        Pattern = pattern;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public long? MinLength { get; }

    public long? MaxLength { get; }

    public string? Pattern { get; }

    public override bool Equals(object? other) => other is StringType otherString &&
        ValidationFlags == otherString.ValidationFlags &&
        MinLength == otherString.MinLength &&
        MaxLength == otherString.MaxLength &&
        Pattern == otherString.Pattern;

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinLength, MaxLength, Pattern);
}
