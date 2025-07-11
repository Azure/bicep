// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class UsingDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        public UsingDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase path)
            : base(leadingNodes)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.UsingKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax), typeof(NoneLiteralSyntax));

            this.Keyword = keyword;
            this.Path = path;

        }

        public UsingDeclarationSyntax(Token keyword, SyntaxBase path)
            : this([], keyword, path)
        {
        }

        public Token Keyword { get; }

        public SyntaxBase Path { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitUsingDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Module;

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Path);

        SyntaxBase IArtifactReferenceSyntax.SourceSyntax => Path;
    }
}
