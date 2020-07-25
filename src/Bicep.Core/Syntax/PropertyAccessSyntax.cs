using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class PropertyAccessSyntax : SyntaxBase
    {
        public PropertyAccessSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax propertyName)
        {
            this.AssertTokenType(dot, nameof(dot), TokenType.Dot);

            this.BaseExpression = baseExpression;
            this.Dot = dot;
            this.PropertyName = propertyName;
        }

        public SyntaxBase BaseExpression { get; }

        public Token Dot { get; }

        public SyntaxBase PropertyName { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitPropertyAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, PropertyName);
    }
}