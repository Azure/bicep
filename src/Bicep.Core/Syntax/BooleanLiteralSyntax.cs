// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class BooleanLiteralSyntax : ExpressionSyntax
    {
        public BooleanLiteralSyntax(Token literal, bool value)
        {
            Literal = literal;
            Value = value;
        }

        public bool Value { get; }

        public Token Literal { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitBooleanLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}
