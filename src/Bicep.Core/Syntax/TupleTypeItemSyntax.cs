// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TupleTypeItemSyntax(IEnumerable<SyntaxBase> leadingNodes, SyntaxBase value) : DecorableSyntax(leadingNodes)
{
    public SyntaxBase Value { get; } = value;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTupleTypeItemSyntax(this);

    public override TextSpan Span => LeadingNodes.FirstOrDefault() is { } firstLeadingNode
        ? TextSpan.Between(firstLeadingNode, Value)
        : Value.Span;
}
