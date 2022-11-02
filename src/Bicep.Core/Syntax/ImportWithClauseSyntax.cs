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
    public class ImportWithClauseSyntax : SyntaxBase
    {
        public ImportWithClauseSyntax(Token keyword, SyntaxBase config)
        {
            AssertTokenType(keyword, nameof(keyword), TokenType.WithKeyword);
            AssertSyntaxType(config, nameof(config), typeof(ObjectSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Config = config;
        }

        public Token Keyword { get; }

        public SyntaxBase Config { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Config);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportWithClauseSyntax(this);
    }
}
