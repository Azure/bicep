// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class NoneTypeLiteralSyntax : TypeSyntax
    {
        public NoneTypeLiteralSyntax(Token noneKeyword)
        {
            AssertTokenType(noneKeyword, nameof(noneKeyword), TokenType.NoneKeyword);

            this.NoneKeyword = noneKeyword;
        }

        public Token NoneKeyword { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitNoneTypeLiteralSyntax(this);

        public override TextSpan Span => this.NoneKeyword.Span;
    }
}
