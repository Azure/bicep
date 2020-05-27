using Bicep.Parser;

namespace Bicep.Syntax
{
    public class StringSyntax : SyntaxBase
    {
        public StringSyntax(Token stringToken)
        {
            StringToken = stringToken;
        }

        public Token StringToken { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitStringSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(StringToken, StringToken);
    }
}