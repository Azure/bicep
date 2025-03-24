// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class WildcardImportSyntax : SyntaxBase, INamedDeclarationSyntax
{
    public WildcardImportSyntax(Token wildcard, AliasAsClauseSyntax aliasAsClause)
    {
        AssertTokenType(wildcard, nameof(wildcard), TokenType.Asterisk);

        this.Wildcard = wildcard;
        this.AliasAsClause = aliasAsClause;
    }

    public Token Wildcard { get; }

    public AliasAsClauseSyntax AliasAsClause { get; }

    public IdentifierSyntax Name => AliasAsClause.Alias;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitWildcardImportSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Wildcard, this.AliasAsClause);
}
