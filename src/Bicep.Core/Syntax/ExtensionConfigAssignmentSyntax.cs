// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ExtensionConfigAssignmentSyntax : StatementSyntax, ITopLevelDeclarationSyntax
    {
        public ExtensionConfigAssignmentSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase identifierSyntax, SyntaxBase withClause)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ExtensionConfigKeyword);
            AssertSyntaxType(identifierSyntax, nameof(identifierSyntax), typeof(IdentifierSyntax));

            this.Keyword = keyword;
            this.Alias = (IdentifierSyntax)identifierSyntax;
            this.WithClause = withClause;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Alias { get; }

        public SyntaxBase WithClause { get; }

        public ObjectSyntax? Config => (this.WithClause as ExtensionWithClauseSyntax)?.Config as ObjectSyntax;

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(this.Alias, this.WithClause));

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitExtensionConfigAssignmentSyntax(this);

        public string? TryGetAlias() => !this.Alias.IsSkipped ? this.Alias.IdentifierName : null;
    }
}
