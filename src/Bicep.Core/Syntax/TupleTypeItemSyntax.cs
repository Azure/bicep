// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TupleTypeItemSyntax : DecorableSyntax
{
    public TupleTypeItemSyntax(IEnumerable<SyntaxBase> leadingNodes, SyntaxBase value) : base(leadingNodes)
    {
        this.Value = value;
    }

    public SyntaxBase Value { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTupleTypeItemSyntax(this);

    public override TextSpan Span => LeadingNodes.FirstOrDefault() is { } firstLeadingNode
        ? TextSpan.Between(firstLeadingNode, Value)
        : Value.Span;
}
