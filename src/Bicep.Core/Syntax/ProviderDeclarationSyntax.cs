// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;

namespace Bicep.Core.Syntax
{
    public class ProviderDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        private readonly Lazy<ImportSpecification> lazySpecification;

        public ProviderDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase specificationString, SyntaxBase withClause, SyntaxBase asClause)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword, LanguageConstants.ProviderKeyword);
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

        SyntaxBase IArtifactReferenceSyntax.SourceSyntax => SpecificationString;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitProviderDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Provider;

        public SyntaxBase? Path => this.Specification.BicepRegistryAddress is { } registryAddress ? SyntaxFactory.CreateStringLiteral(registryAddress) : null;
    }
}
