using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class VarDeclSyntax : SyntaxBase
    {
        public VarDeclSyntax(Token variableKeyword, IdentifierSyntax identifier, Token colon, SyntaxBase expression, Token semicolon)
        {
            VariableKeyword = variableKeyword;
            Identifier = identifier;
            Colon = colon;
            Expression = expression;
            Semicolon = semicolon;
        }

        public Token VariableKeyword { get; }

        public IdentifierSyntax Identifier { get; }

        public Token Colon { get; }

        public SyntaxBase Expression { get; }

        public Token Semicolon { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitVarDeclSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(VariableKeyword, Semicolon);
    }
}