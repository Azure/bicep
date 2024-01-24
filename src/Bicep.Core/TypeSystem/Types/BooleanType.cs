// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types;

public class BooleanType : TypeSymbol
{
    internal BooleanType(TypeSymbolValidationFlags validationFlags)
        : base(LanguageConstants.TypeNameBool)
    {
        ValidationFlags = validationFlags;
    }

    public override TypeKind TypeKind => TypeKind.Primitive;

    public override TypeSymbolValidationFlags ValidationFlags { get; }

    public override bool Equals(object? other) => other is BooleanType otherBool && ValidationFlags == otherBool.ValidationFlags;

    public override int GetHashCode() => HashCode.Combine(TypeKind, ValidationFlags);
}
