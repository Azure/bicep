// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class NoneLiteralSyntax : ExpressionSyntax
    {
        public NoneLiteralSyntax(Token noneKeyword)
        {
            AssertTokenType(noneKeyword, nameof(noneKeyword), TokenType.NoneKeyword);

            this.NoneKeyword = noneKeyword;
        }

        public Token NoneKeyword { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitNoneLiteralSyntax(this);

        public override TextSpan Span => this.NoneKeyword.Span;
    }
}
