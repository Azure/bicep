using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParameterDeclSyntax : StatementSyntax
    {
        public ParameterDeclSyntax(Token parameterKeyword, IdentifierSyntax name, IdentifierSyntax type)//, Token semicolon)
        {
            this.ParameterKeyword = parameterKeyword;
            this.Name = name;
            this.Type = type;
        }

        public ParameterDeclSyntax(Token parameterKeyword, IdentifierSyntax name, IdentifierSyntax type, Token colon, SyntaxBase defaultValue)
            : this(parameterKeyword, name, type)
        {
            this.Colon = colon;
            this.DefaultValue = defaultValue;
        }

        public Token ParameterKeyword { get; }
        
        public IdentifierSyntax Name { get; }

        public IdentifierSyntax Type { get; }

        public Token? Colon { get; }

        public SyntaxBase? DefaultValue { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitInputDeclSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(this.ParameterKeyword, this.Type); //Semicolon);
    }
}