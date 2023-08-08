// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Syntax
{
    public class ProviderDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IForeignTemplateReference
    {
        private readonly Lazy<ImportSpecification> lazySpecification;

        public ProviderDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase specificationString, SyntaxBase withClause, SyntaxBase asClause)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword);
            AssertSyntaxType(specificationString, nameof(specificationString), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.SpecificationString = specificationString;
            this.WithClause = withClause;
            this.AsClause = asClause;

            this.lazySpecification = new(() => ImportSpecification.From(specificationString));
        }

        public Token Keyword { get; }

        public SyntaxBase SpecificationString { get; }

        public SyntaxBase WithClause { get; }

        public SyntaxBase AsClause { get; }

        public ImportSpecification Specification => lazySpecification.Value;

        public ObjectSyntax? Config => (this.WithClause as ProviderWithClauseSyntax)?.Config as ObjectSyntax;

        public IdentifierSyntax? Alias => (this.AsClause as AliasAsClauseSyntax)?.Alias;

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(this.SpecificationString, this.WithClause, this.AsClause));

        SyntaxBase IForeignTemplateReference.ReferenceSourceSyntax => SyntaxFactory.CreateStringLiteral($@"br:asilvermantestbr.azurecr.io/bicep/providers/{this.Specification.Name}:test");

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitProviderDeclarationSyntax(this);

        public StringSyntax? TryGetPath() => SyntaxFactory.CreateStringLiteral($@"br:asilvermantestbr.azurecr.io/bicep/providers/{this.Specification.Name}:test");
    }
}
