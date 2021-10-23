// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SimpleTypeSyntax : TypeSyntax
    {
        public SimpleTypeSyntax(Token identifier)
        {
            AssertTokenType(identifier, nameof(identifier), TokenType.Identifier);
            Assert(string.IsNullOrEmpty(identifier.Text) == false, "Identifier must not be null or empty.");

            this.Identifier = identifier;
        }

        public Token Identifier { get; }

        public string TypeName => this.Identifier.Text;

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.VisitSimpleTypeSyntax(this);
        }

        public override TextSpan Span => this.Identifier.Span;
    }
}
