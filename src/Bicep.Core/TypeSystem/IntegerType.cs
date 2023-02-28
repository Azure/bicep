// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

public class IntegerType : TypeSymbol
{
    public IntegerType(long? minValue, long? maxValue, TypeSymbolValidationFlags validationFlags)
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
}
