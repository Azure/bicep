// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Syntax
{
    public class ImportDeclarationSyntax : StatementSyntax, ITopLevelDeclarationSyntax
    {
        public ImportDeclarationSyntax(IEnumerable<SyntaxBase> leadingNodes, Token keyword, SyntaxBase specification, ImportWithClauseSyntax? withClause, ImportAsClauseSyntax? asClause)
            : base(leadingNodes)
        {
            this.Keyword = keyword;
            this.Specification = specification;
            this.WithClause = withClause;
            this.AsClause = asClause;
        }

        public Token Keyword { get; }

        public SyntaxBase Specification { get; }

        public ImportWithClauseSyntax? WithClause { get; }

        public ImportAsClauseSyntax? AsClause { get; }

        public ObjectSyntax? Config => this.WithClause?.Config as ObjectSyntax;

        public IdentifierSyntax? Alias => this.AsClause?.Alias as IdentifierSyntax;

        public override TextSpan Span => TextSpan.Between(this.Keyword, TextSpan.LastNonNull(this.Specification, this.WithClause, this.AsClause));

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportDeclarationSyntax(this);
    }
}
