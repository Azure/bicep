// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class BooleanLiteralSyntax : SyntaxBase
    {
        public BooleanLiteralSyntax(Token literal, bool value)
        {
            Literal = literal;
            Value = value;
        }

        public bool Value { get; }

        public Token Literal { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitBooleanLiteralSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Literal, Literal);
    }
}
