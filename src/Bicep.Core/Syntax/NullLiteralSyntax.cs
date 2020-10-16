// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class NullLiteralSyntax : SyntaxBase
    {
        public NullLiteralSyntax(Token nullKeyword)
        {
            AssertTokenType(nullKeyword, nameof(nullKeyword), TokenType.NullKeyword);

            this.NullKeyword = nullKeyword;
        }

        public Token NullKeyword { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitNullLiteralSyntax(this);

        public override TextSpan Span => this.NullKeyword.Span;
    }
}
