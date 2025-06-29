// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ExtensionConfigAssignmentSyntax : StatementSyntax, ITopLevelDeclarationSyntax
    {
        public ExtensionConfigAssignmentSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase specificationString, SyntaxBase withClause)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ExtensionKeyword);
            AssertSyntaxType(specificationString, nameof(specificationString), typeof(StringSyntax), typeof(SkippedTriviaSyntax), typeof(IdentifierSyntax));

            this.Keyword = keyword;
            this.SpecificationString = specificationString;
            this.WithClause = withClause;
        }

        public Token Keyword { get; }

        public SyntaxBase SpecificationString { get; }

        public SyntaxBase WithClause { get; }

        public ObjectSyntax? Config => (this.WithClause as ExtensionWithClauseSyntax)?.Config as ObjectSyntax;

        public string? TryGetSymbolName() => this.SpecificationString switch
        {
            IdentifierSyntax value => value.IdentifierName,
            _ => null,
        };

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(this.SpecificationString, this.WithClause));

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitExtensionConfigAssignmentSyntax(this);
    }
}
