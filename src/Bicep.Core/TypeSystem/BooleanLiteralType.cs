// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

public class BooleanLiteralType : TypeSymbol
{
    public BooleanLiteralType(bool value)
        : base(value.ToString())
    {
        Value = value;
    }

    public BooleanLiteralType(string typeName, bool value) :
        base(typeName)
    {
        Value = value;
    }

    public override TypeKind TypeKind => TypeKind.BooleanLiteral;

    public bool Value { get; }
}
