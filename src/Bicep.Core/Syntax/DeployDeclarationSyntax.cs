// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;

namespace Bicep.Core.Syntax
{
    public class DeployDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax, IArtifactReferenceSyntax
    {
        public DeployDeclarationSyntax(Token keyword, IdentifierSyntax name, SyntaxBase path, SyntaxBase withClause)
            : base([])
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.DeployKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(withClause, nameof(withClause), typeof(WithClauseSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Name = name;
            this.Path = path;
            this.WithClause = withClause;
        }

        public Token Keyword { get; }

        public IdentifierSyntax Name { get; }

        public SyntaxBase Path { get; }

        public SyntaxBase WithClause { get; }

        public SyntaxBase SourceSyntax => this.Path;

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.WithClause);

        public override void Accept(ISyntaxVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public ArtifactType GetArtifactType() => ArtifactType.Module;
    }
}
