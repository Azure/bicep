// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ProviderAsClauseSyntax : SyntaxBase
    {
        public ProviderAsClauseSyntax(Token keyword, SyntaxBase alias)
        {
            AssertTokenType(keyword, nameof(keyword), TokenType.AsKeyword);
            AssertSyntaxType(alias, nameof(alias), typeof(IdentifierSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Alias = alias;
        }

        public Token Keyword { get; }

        public SyntaxBase Alias { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Alias);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitProviderAsClauseSyntax(this);
    }
}
