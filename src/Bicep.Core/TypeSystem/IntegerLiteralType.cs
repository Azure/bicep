// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

public class IntegerLiteralType : TypeSymbol
{
    public IntegerLiteralType(long value)
        : base(value.ToString())
    {
        Value = value;
    }

    public IntegerLiteralType(string typeName, long value) :
        base(typeName)
    {
        Value = value;
    }

    public override TypeKind TypeKind => TypeKind.IntegerLiteral;

    public long Value { get; }
}
