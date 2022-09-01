// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ImportDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ImportDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax providerName, SyntaxBase asKeyword, IdentifierSyntax aliasName, SyntaxBase config)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword);
            AssertSyntaxType(providerName, nameof(providerName), typeof(IdentifierSyntax));
            AssertSyntaxType(asKeyword, nameof(asKeyword), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(aliasName, nameof(aliasName), typeof(IdentifierSyntax));
            AssertSyntaxType(config, nameof(config), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.ProviderName = providerName;
            this.AsKeyword = asKeyword;
            this.AliasName = aliasName;
            this.Config = config;
        }

        public Token Keyword { get; }

        public IdentifierSyntax ProviderName { get; }

        public SyntaxBase AsKeyword { get; }

        public IdentifierSyntax AliasName { get; }

        public SyntaxBase Config { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, TextSpan.LastNonNull(AliasName, Config));

        public IdentifierSyntax Name => AliasName;
    }
}
