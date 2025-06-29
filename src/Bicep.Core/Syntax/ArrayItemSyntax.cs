// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ArrayItemSyntax : ExpressionSyntax
    {
        public ArrayItemSyntax(SyntaxBase value)
        {
            this.Value = value;
        }

        public SyntaxBase Value { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayItemSyntax(this);

        public override TextSpan Span => this.Value.Span;
    }
}
