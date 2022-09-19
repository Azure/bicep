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
        public ImportDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase providerName, SyntaxBase asKeyword, IdentifierSyntax aliasName, SyntaxBase config)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword);
            AssertSyntaxType(providerName, nameof(providerName), typeof(IdentifierSyntax), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
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

        public SyntaxBase ProviderName { get; }

        public SyntaxBase AsKeyword { get; }

        public IdentifierSyntax AliasName { get; }

        public SyntaxBase Config { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportDeclarationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeadingNodes.FirstOrDefault() ?? this.Keyword, TextSpan.LastNonNull(AliasName, Config));

        public IdentifierSyntax Name => AliasName;

        public string? TryGetProviderName()
            => ProviderName switch {
                IdentifierSyntax identifier when identifier.IsValid => identifier.IdentifierName,
                StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is {} stringValue => stringValue.Split('@').First(),
                _ => null,
            };

        public string? TryGetProviderVersion()
        {
            if (ProviderName is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is {} stringValue &&
                stringValue.IndexOf('@') > -1)
            {
                return stringValue.Substring(stringValue.IndexOf('@'));
            }

            return null;
        }
    }
}
