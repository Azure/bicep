// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class PropertyAccessSyntax : AccessExpressionSyntax
    {
        public PropertyAccessSyntax(SyntaxBase baseExpression, Token dot, Token? safeAccessMarker, IdentifierSyntax propertyName)
            : base(baseExpression, safeAccessMarker)
        {
            AssertTokenType(dot, nameof(dot), TokenType.Dot);

            this.Dot = dot;
            this.PropertyName = propertyName;
        }

        public Token Dot { get; }

        public IdentifierSyntax PropertyName { get; }

        public override SyntaxBase IndexExpression => PropertyName;

        public override PropertyAccessSyntax AsSafeAccess() => SafeAccessMarker is null
            ? new(BaseExpression, Dot, SyntaxFactory.QuestionToken, PropertyName)
            : this;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitPropertyAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, PropertyName);
    }
}
