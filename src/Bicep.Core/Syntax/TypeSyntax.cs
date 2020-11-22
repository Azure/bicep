// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class TypeSyntax : SyntaxBase
    {
        public TypeSyntax(Token identifier)
        {
            AssertTokenType(identifier, nameof(identifier), TokenType.Identifier);
            Assert(string.IsNullOrEmpty(identifier.Text) == false, "Identifier must not be null or empty.");

            this.Identifier = identifier;
        }

        public Token Identifier { get; }

        public string TypeName => this.Identifier.Text;

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.VisitTypeSyntax(this);
        }

        public override TextSpan Span => TextSpan.Between(this.Identifier, this.Identifier);
    }
}
