// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Core.TypeSystem.Types;

public class StringType : TypeSymbol
{
    internal StringType(long? minLength, long? maxLength, Regex? pattern, TypeSymbolValidationFlags validationFlags)
        : base(LanguageConstants.TypeNameString)
    {
        MinLength = minLength;
        MaxLength = maxLength;
        Pattern = pattern;
        ValidationFlags = validationFlags;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public long? MinLength { get; }

    public long? MaxLength { get; }

    public Regex? Pattern { get; }

    public override bool Equals(object? other) => other is StringType otherString &&
        ValidationFlags == otherString.ValidationFlags &&
        MinLength == otherString.MinLength &&
        MaxLength == otherString.MaxLength &&
        Pattern?.ToString() == Pattern?.ToString();

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags, MinLength, MaxLength, Pattern?.ToString());
}
