using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParameterDefaultValueSyntax : SyntaxBase
    {
        public ParameterDefaultValueSyntax(Token defaultKeyword, SyntaxBase defaultValue)
        {
            this.AssertTokenType(defaultKeyword, nameof(defaultKeyword), TokenType.DefaultKeyword);

            this.DefaultKeyword = defaultKeyword;
            this.DefaultValue = defaultValue;
        }

        public Token DefaultKeyword { get; }

        public SyntaxBase DefaultValue { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitParameterDefaultValueSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.DefaultKeyword, this.DefaultValue);
    }
}