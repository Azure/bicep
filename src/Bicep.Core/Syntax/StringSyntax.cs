using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class StringSyntax : SyntaxBase
    {
        public StringSyntax(Token stringToken)
        {
            this.StringToken = stringToken;
        }

        public Token StringToken { get; }

        public string Value => Lexer.GetStringValue(this.StringToken);

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitStringSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(StringToken, StringToken);
    }
}