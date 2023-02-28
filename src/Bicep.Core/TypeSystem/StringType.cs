// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

public class StringType : TypeSymbol
{
    public StringType(long? minLength, long? maxLength, TypeSymbolValidationFlags validationFlags)
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
}
