using Bicep.Parser;

namespace Bicep.Syntax
{
    public class ObjectPropertySyntax : SyntaxBase
    {
        public ObjectPropertySyntax(IdentifierSyntax identifier, Token colon, SyntaxBase expression)
        {
            Identifier = identifier;
            Colon = colon;
            Expression = expression;
        }

        public IdentifierSyntax Identifier { get; }

        public Token Colon { get; }

        public SyntaxBase Expression { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitObjectPropertySyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Identifier, Expression);
    }
}