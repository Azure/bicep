using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    //public class ResourceDeclSyntax : SyntaxBase
    //{
    //    public ResourceDeclSyntax(Token resourceKeyword, IdentifierSyntax provider, StringSyntax type, IdentifierSyntax identifier, Token colon, SyntaxBase expression, Token semicolon)
    //    {
    //        ResourceKeyword = resourceKeyword;
    //        Provider = provider;
    //        Type = type;
    //        Identifier = identifier;
    //        Colon = colon;
    //        Expression = expression;
    //        Semicolon = semicolon;
    //    }

    //    public Token ResourceKeyword { get; }

    //    public IdentifierSyntax Provider { get; }

    //    public StringSyntax Type { get; }

    //    public IdentifierSyntax Identifier { get; }

    //    public Token Colon { get; }

    //    public SyntaxBase Expression { get; }

    //    public Token Semicolon { get; }

    //    public override void Accept(SyntaxVisitor visitor)
    //        => visitor.VisitResourceDeclSyntax(this);

    //    public override TextSpan Span
    //        => TextSpan.Between(ResourceKeyword, Semicolon);
    //}
}