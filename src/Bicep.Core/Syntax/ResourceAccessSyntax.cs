// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ResourceAccessSyntax : ExpressionSyntax, ISymbolReference
    {
        public ResourceAccessSyntax(SyntaxBase baseExpression, Token colon, IdentifierSyntax resourceName)
        {
            AssertTokenType(colon, nameof(colon), TokenType.Colon);

            this.BaseExpression = baseExpression;
            this.Colon = colon;
            this.ResourceName = resourceName;
        }

        public SyntaxBase BaseExpression { get; }

        public Token Colon { get; }

        public IdentifierSyntax ResourceName { get; }

        IdentifierSyntax ISymbolReference.Name => ResourceName;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitResourceAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, ResourceName);
    }
}
