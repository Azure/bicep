// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ArrayItemSyntax(SyntaxBase value) : ExpressionSyntax
    {
        public SyntaxBase Value { get; } = value;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayItemSyntax(this);

        public override TextSpan Span => this.Value.Span;
    }
}
