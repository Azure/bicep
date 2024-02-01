// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;

namespace Bicep.Core.Syntax
{
    public class ProviderDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        private readonly Lazy<IResourceTypesProviderSpecification> lazySpecification;

        public ProviderDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase specificationString, SyntaxBase withClause, SyntaxBase asClause)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword, LanguageConstants.ProviderKeyword);
            AssertSyntaxType(specificationString, nameof(specificationString), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.SpecificationString = specificationString;
            this.WithClause = withClause;
            this.AsClause = asClause;

            this.lazySpecification = new(() => ProviderSpecificationFactory.FromSyntax(specificationString));
        }

        public Token Keyword { get; }

        public SyntaxBase SpecificationString { get; }

        public SyntaxBase WithClause { get; }

        public SyntaxBase AsClause { get; }

        public IResourceTypesProviderSpecification Specification => lazySpecification.Value;

        public ObjectSyntax? Config => (this.WithClause as ProviderWithClauseSyntax)?.Config as ObjectSyntax;

        public IdentifierSyntax? Alias => (this.AsClause as AliasAsClauseSyntax)?.Alias;

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(this.SpecificationString, this.WithClause, this.AsClause));

        SyntaxBase IArtifactReferenceSyntax.SourceSyntax => SpecificationString;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitProviderDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Provider;

        // if the provider specification is inlined return a value otherwise return null
        public SyntaxBase? Path => this.Specification is InlinedResourceTypesProviderSpecification spec ? SyntaxFactory.CreateStringLiteral(spec.UnexpandedArtifactAddress) : null;

        public ResultWithDiagnostic<string> ResolveArtifactPath(RootConfiguration config)
        {
            if (!config.ProvidersConfig.TryGetProviderSource(this.Specification.NamespaceIdentifier).IsSuccess(out var providerSource, out var errorBuilder))
            {
                return new(errorBuilder);
            }
            return new($"br:{providerSource.Source}:{providerSource.Version}");
        }
    }
}
