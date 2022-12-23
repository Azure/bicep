// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class PropertyAccessSyntax : ExpressionSyntax
    {
        public PropertyAccessSyntax(SyntaxBase baseExpression, Token dot, Token? safeAccessMarker, IdentifierSyntax propertyName)
        {
            AssertTokenType(dot, nameof(dot), TokenType.Dot);
            AssertTokenType(safeAccessMarker, nameof(safeAccessMarker), TokenType.Question);

            this.BaseExpression = baseExpression;
            this.Dot = dot;
            this.SafeAccessMarker = safeAccessMarker;
            this.PropertyName = propertyName;
        }

        public SyntaxBase BaseExpression { get; }

        public Token Dot { get; }

        public Token? SafeAccessMarker { get; }

        public IdentifierSyntax PropertyName { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitPropertyAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, PropertyName);
    }
}
