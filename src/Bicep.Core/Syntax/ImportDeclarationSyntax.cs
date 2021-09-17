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
        public ImportDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, IdentifierSyntax providerName, Token? asKeyword, IdentifierSyntax? aliasName, SyntaxBase config)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword);
            AssertSyntaxType(providerName, nameof(providerName), typeof(IdentifierSyntax));
            AssertKeyword(asKeyword, nameof(asKeyword), LanguageConstants.AsKeyword);
            AssertSyntaxType(aliasName, nameof(aliasName), typeof(IdentifierSyntax));
            AssertSyntaxType(config, nameof(config), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax));

            this.Keyword = keyword;
            this.ProviderName = providerName;
            this.AsKeyword = asKeyword;
            this.AliasName = aliasName;
            this.Config = config;
        }

        public Token Keyword { get; }

        public IdentifierSyntax ProviderName { get; }

        public Token? AsKeyword { get; }

        public IdentifierSyntax? AliasName { get; }

        public SyntaxBase Config { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, this.Config);

        public IdentifierSyntax Name => AliasName ?? ProviderName;

        public TypeSymbol GetDeclaredType()
        {
            // TODO: make sure this works nicely with parse errors
            return new ImportType(Name.IdentifierName);
        }
    }
}
