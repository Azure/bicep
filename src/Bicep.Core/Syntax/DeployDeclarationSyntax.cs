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
        public DeployDeclarationSyntax(Token keyword, SyntaxBase path, SyntaxBase body)
            : base([])
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.DeployKeyword);
            AssertSyntaxType(path, nameof(path), typeof(StringSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(body, nameof(body), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Path = path;
            this.Body = body;
        }

        public Token Keyword { get; }

        public SyntaxBase Path { get; }

        public SyntaxBase Body { get; }

        public SyntaxBase SourceSyntax => this.Path;

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Body);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitDeployDeclarationSyntax(this);

        public ArtifactType GetArtifactType() => ArtifactType.Module;
    }
}
