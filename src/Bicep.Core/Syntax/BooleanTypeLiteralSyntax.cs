// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class BooleanTypeLiteralSyntax : TypeSyntax
{
    public BooleanTypeLiteralSyntax(Token literal, bool value)
    {
        Literal = literal;
        Value = value;
    }

    public bool Value { get; }

    public Token Literal { get; }

    public override void Accept(ISyntaxVisitor visitor)
        => visitor.VisitBooleanTypeLiteralSyntax(this);

    public override TextSpan Span
        => TextSpan.Between(Literal, Literal);
}
