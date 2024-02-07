// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class IntegerLiteralSyntax(Token literal, ulong value) : ExpressionSyntax
    {
        public Token Literal { get; } = literal;

        public ulong Value { get; } = value;

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitIntegerLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}
