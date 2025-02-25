// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem.Providers;

namespace Bicep.Core.Syntax
{
    public class ExtensionDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        public ExtensionDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase specificationString, SyntaxBase withClause, SyntaxBase asClause)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ImportKeyword, LanguageConstants.ExtensionKeyword);
            AssertSyntaxType(specificationString, nameof(specificationString), typeof(StringSyntax), typeof(SkippedTriviaSyntax), typeof(IdentifierSyntax));

            this.Keyword = keyword;
            this.SpecificationString = specificationString;
            this.WithClause = withClause;
            this.AsClause = asClause;
        }

        public Token Keyword { get; }

        public SyntaxBase SpecificationString { get; }

        public SyntaxBase WithClause { get; }

        public SyntaxBase AsClause { get; }

        public ObjectSyntax? Config => (this.WithClause as ExtensionWithClauseSyntax)?.Config as ObjectSyntax;

        public IdentifierSyntax? Alias => (this.AsClause as AliasAsClauseSyntax)?.Alias;

        public string? TryGetSymbolName() => (this.Alias, this.SpecificationString) switch
        {
            (not null, _) => this.Alias.IdentifierName,
            (null, IdentifierSyntax value) => value.IdentifierName,
            _ => null,
        };

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(this.SpecificationString, this.WithClause, this.AsClause));

        public SyntaxBase SourceSyntax => SpecificationString;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitExtensionDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Extension;

        // if the extension specification is inlined return a value otherwise return null
        public SyntaxBase? Path => this.SpecificationString as StringSyntax;
    }
}
