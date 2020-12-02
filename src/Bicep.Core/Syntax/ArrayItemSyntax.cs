// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;

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
