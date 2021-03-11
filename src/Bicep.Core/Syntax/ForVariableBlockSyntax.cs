// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ForVariableBlockSyntax : SyntaxBase
    {
        public ForVariableBlockSyntax(Token openParen, LocalVariableSyntax itemVariable, SyntaxBase comma, LocalVariableSyntax indexVariable, SyntaxBase closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            AssertSyntaxType(comma, nameof(comma), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(comma as Token, nameof(comma), TokenType.Comma);
            AssertSyntaxType(closeParen, nameof(closeParen), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(closeParen as Token, nameof(closeParen), TokenType.RightParen);

            this.OpenParen = openParen;
            this.ItemVariable = itemVariable;
            this.Comma = comma;
            this.IndexVariable = indexVariable;
            this.CloseParen = closeParen;
        }

        public Token OpenParen { get; }

        public LocalVariableSyntax ItemVariable { get; }

        public SyntaxBase Comma { get; }

        public LocalVariableSyntax IndexVariable { get; }

        public SyntaxBase CloseParen { get; }

        public override TextSpan Span => TextSpan.Between(this.OpenParen, this.CloseParen);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitForVariableBlockSyntax(this);
    }
}
