// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class IdentifierSyntax : SyntaxBase
    {
        public IdentifierSyntax(Token identifier)
        {
            AssertTokenType(identifier, nameof(identifier), TokenType.Identifier);
            Assert(string.IsNullOrEmpty(identifier.Text) == false, "Identifier must not be null or empty.");

            this.Identifier = identifier;
        }

        public Token Identifier { get; }

        public string IdentifierName => this.Identifier.Text;

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitIdentifierSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Identifier, this.Identifier);
    }
}
