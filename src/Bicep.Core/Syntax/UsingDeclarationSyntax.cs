// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;

namespace Bicep.Core.Syntax
{
    public class UsingDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        public UsingDeclarationSyntax(Token keyword, SyntaxBase path)
            : base([])
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.UsingKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Path = path;

        }

        public Token Keyword { get; }

        public SyntaxBase Path { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitUsingDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Module;

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Path);

        SyntaxBase IArtifactReferenceSyntax.SourceSyntax => Path;
    }
}
