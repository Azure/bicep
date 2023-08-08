// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class AliasAsClauseSyntax : SyntaxBase
{
    public AliasAsClauseSyntax(Token keyword, IdentifierSyntax alias)
    {
        AssertTokenType(keyword, nameof(keyword), TokenType.AsKeyword);
        AssertSyntaxType(alias, nameof(alias), typeof(IdentifierSyntax), typeof(SkippedTriviaSyntax));

        this.Keyword = keyword;
        this.Alias = alias;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Alias { get; }

    public override TextSpan Span => TextSpan.Between(this.Keyword, this.Alias);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitAliasAsClauseSyntax(this);
}
