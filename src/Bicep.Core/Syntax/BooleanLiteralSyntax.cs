// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class BooleanLiteralSyntax(Token literal, bool value) : ExpressionSyntax
    {
        public bool Value { get; } = value;

        public Token Literal { get; } = literal;

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitBooleanLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}
