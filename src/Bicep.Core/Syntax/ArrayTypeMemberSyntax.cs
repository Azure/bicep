// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ArrayTypeMemberSyntax(SyntaxBase value) : TypeSyntax
{
    public SyntaxBase Value { get; } = value;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayTypeMemberSyntax(this);

    public override TextSpan Span => Value.Span;
}
