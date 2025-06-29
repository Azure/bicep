// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class AliasAsClauseSyntax : SyntaxBase
{
    public AliasAsClauseSyntax(Token keyword, IdentifierSyntax alias)
    {
        AssertKeyword(keyword, nameof(keyword), LanguageConstants.AsKeyword);
        AssertSyntaxType(alias, nameof(alias), typeof(IdentifierSyntax), typeof(SkippedTriviaSyntax));

        this.Keyword = keyword;
        this.Alias = alias;
    }

    public Token Keyword { get; }

    public IdentifierSyntax Alias { get; }

    public override TextSpan Span => TextSpan.Between(this.Keyword, this.Alias);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitAliasAsClauseSyntax(this);
}
