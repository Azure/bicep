// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class UnionTypeMemberSyntax : TypeSyntax
{
    public UnionTypeMemberSyntax(SyntaxBase value)
    {
        Value = value;
    }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitUnionTypeMemberSyntax(this);

    public override TextSpan Span => this.Value.Span;
}
