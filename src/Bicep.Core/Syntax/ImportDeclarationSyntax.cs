// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Syntax
{
    public class ImportDeclarationSyntax : StatementSyntax, ITopLevelNamedDeclarationSyntax
    {
        public ImportDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax aliasName, SyntaxBase fromKeyword, IdentifierSyntax providerName, SyntaxBase? config)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword);
            AssertSyntaxType(aliasName, nameof(aliasName), typeof(IdentifierSyntax));
            AssertSyntaxType(fromKeyword, nameof(fromKeyword), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(providerName, nameof(providerName), typeof(IdentifierSyntax));
            AssertSyntaxType(config, nameof(config), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.AliasName = aliasName;
            this.FromKeyword = fromKeyword;
            this.ProviderName = providerName;
            this.Config = config;
        }

        public Token Keyword { get; }

        public IdentifierSyntax AliasName { get; }

        public SyntaxBase FromKeyword { get; }

        public IdentifierSyntax ProviderName { get; }

        public SyntaxBase? Config { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, TextSpan.LastNonNull(ProviderName, Config));

        public IdentifierSyntax Name => AliasName;
    }
}
