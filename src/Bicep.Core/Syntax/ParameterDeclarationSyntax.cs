using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParameterDeclarationSyntax : StatementSyntax
    {
        public ParameterDeclarationSyntax(Token parameterKeyword, IdentifierSyntax name, IdentifierSyntax type)
        {
            this.ParameterKeyword = parameterKeyword;
            this.Name = name;
            this.Type = type;
        }

        public ParameterDeclarationSyntax(Token parameterKeyword, IdentifierSyntax name, IdentifierSyntax type, Token assignment, SyntaxBase defaultValue)
            : this(parameterKeyword, name, type)
        {
            this.Assignment = assignment;
            this.DefaultValue = defaultValue;
        }

        public Token ParameterKeyword { get; }
        
        public IdentifierSyntax Name { get; }

        public IdentifierSyntax Type { get; }

        public Token? Assignment { get; }

        public SyntaxBase? DefaultValue { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitParameterDeclarationSyntax(this);

        public override TextSpan Span
            => this.DefaultValue == null
                ? TextSpan.Between(this.ParameterKeyword, this.Type)
                : TextSpan.Between(this.ParameterKeyword, this.DefaultValue);
    }
}