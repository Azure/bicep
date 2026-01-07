// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ExtendsDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        public ExtendsDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase path)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ExtendsKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Path = path;

        }

        public Token Keyword { get; }

        public SyntaxBase Path { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitExtendsDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Module;

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Path);

        SyntaxBase IArtifactReferenceSyntax.SourceSyntax => Path;
    }
}
