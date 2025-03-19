// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ArrayTypeMemberSyntax : TypeSyntax
{
    public ArrayTypeMemberSyntax(SyntaxBase value)
    {
        this.Value = value;
    }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayTypeMemberSyntax(this);

    public override TextSpan Span => Value.Span;
}
