// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class PropertyAccessSyntax : ExpressionSyntax
    {
        public PropertyAccessSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax propertyName)
        {
            AssertTokenType(dot, nameof(dot), TokenType.Dot);

            this.BaseExpression = baseExpression;
            this.Dot = dot;
            this.PropertyName = propertyName;
        }

        public SyntaxBase BaseExpression { get; }

        public Token Dot { get; }

        public IdentifierSyntax PropertyName { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitPropertyAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, PropertyName);
    }
}
