// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class NumericLiteralSyntax : ExpressionSyntax
    {
        public NumericLiteralSyntax(Token literal, int value)
        {
            Literal = literal;
            Value = value;
        }

        public Token Literal { get; }

        public int Value { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitNumericLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}
