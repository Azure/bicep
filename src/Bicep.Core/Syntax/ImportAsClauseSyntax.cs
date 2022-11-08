// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Syntax
{
    public class ImportAsClauseSyntax : SyntaxBase
    {
        public ImportAsClauseSyntax(Token keyword, SyntaxBase alias)
        {
            AssertTokenType(keyword, nameof(keyword), TokenType.AsKeyword);
            AssertSyntaxType(alias, nameof(alias), typeof(IdentifierSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Alias = alias;
        }

        public Token Keyword { get; }

        public SyntaxBase Alias { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Alias);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportAsClauseSyntax(this);
    }
}
