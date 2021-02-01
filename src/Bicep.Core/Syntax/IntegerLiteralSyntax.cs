// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class IntegerLiteralSyntax : ExpressionSyntax
    {
        public IntegerLiteralSyntax(Token literal, long value)
        {
            Literal = literal;
            Value = value;
        }

        public Token Literal { get; }

        public long Value { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitIntegerLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}
