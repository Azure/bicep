// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class NullTypeLiteralSyntax : TypeSyntax
    {
        public NullTypeLiteralSyntax(Token nullKeyword)
        {
            AssertTokenType(nullKeyword, nameof(nullKeyword), TokenType.NullKeyword);

            this.NullKeyword = nullKeyword;
        }

        public Token NullKeyword { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitNullTypeLiteralSyntax(this);

        public override TextSpan Span => this.NullKeyword.Span;
    }
}
